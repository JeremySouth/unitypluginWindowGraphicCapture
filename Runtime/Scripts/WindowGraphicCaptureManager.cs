using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WindowGraphicCapture;

public class WindowGraphicCaptureManager : MonoBehaviour
{
    //setup
    public const int MiddlePriorityMaxZ = 5;
    public const float childWindowZDistance = 0.02f;

    //instance
    private static WindowGraphicCaptureManager _instance;
    public static WindowGraphicCaptureManager instance
    {
        get { return GetInstance(); }
    }
    private static WindowGraphicCaptureManager GetInstance()
    {
        if (_instance != null) return _instance;

        var manager = FindObjectOfType<WindowGraphicCaptureManager>();
        if(manager)
        {
            _instance = manager;
        }
        else
        {
            GameObject objManager = new GameObject("WindowGraphicCaptureManager");
            _instance = objManager.AddComponent<WindowGraphicCaptureManager>();
        }
        return _instance;
    }

    //debug mode
    public DebugMode debugModeFromInspector = DebugMode.File;
    private static DebugMode debugModeFromScript = DebugMode.File;
    private static bool debugModeChangedFromScript = false;
    public static DebugMode debugMode
    {
        get
        {
            return debugModeChangedFromScript ?
                debugModeFromScript :
                instance.debugModeFromInspector;
        }
        set
        {
            debugModeFromScript = value;
            debugModeChangedFromScript = true;
        }
    }
    public static event WindowGraphicCapturePlugin.DebugLogDelegate _onDebugLog = OnDebugLog;
    public static event WindowGraphicCapturePlugin.DebugLogDelegate _onDebugErr = OnDebugErr;
    [AOT.MonoPInvokeCallback(typeof(WindowGraphicCapturePlugin.DebugLogDelegate))]
    private static void OnDebugLog(string msg) { Debug.Log(msg); }
    [AOT.MonoPInvokeCallback(typeof(WindowGraphicCapturePlugin.DebugLogDelegate))]
    private static void OnDebugErr(string msg) { Debug.LogError(msg); }

    //window events
    public WindowTitlesUpdateTiming windowTitlesUpdateTiming = WindowTitlesUpdateTiming.Manual;
    private WindowEvent _onWindowAdded = new WindowEvent();
    public static WindowEvent onWindowAdded
    {
        get { return instance._onWindowAdded; }
    }

    private WindowEvent _onWindowRemoved = new WindowEvent();
    public static WindowEvent onWindowRemoved
    {
        get { return instance._onWindowRemoved; }
    }

    private WindowEvent _onDesktopAdded = new WindowEvent();
    public static WindowEvent onDesktopAdded
    {
        get { return instance._onDesktopAdded; }
    }

    private WindowEvent _onDesktopRemoved = new WindowEvent();
    public static WindowEvent onDesktopRemoved
    {
        get { return instance._onDesktopRemoved; }
    }

    private WinEvent _onCursorCaptured = new WinEvent();
    public static WinEvent onCursorCaptured
    {
        get { return instance._onCursorCaptured; }
    }

    System.IntPtr _renderEventFunc;
    private Dictionary<int, Window> _windows = new Dictionary<int, Window>();
    static public Dictionary<int, Window> windows
    {
        get { return instance._windows; }
    }

    private int _cursorWindowId = -1;
    static public Window cursorWindow
    {
        get { return Find(instance._cursorWindowId); }
    }

    private WindowCursor _cursor = new WindowCursor();
    static public WindowCursor cursor
    {
        get { return instance._cursor; }
    }
    List<int> desktops_ = new List<int>();
    static public int desktopCount
    {
        get { return instance.desktops_.Count; }
    }

    void Awake()
    {
        WindowGraphicCapturePlugin.SetDebugMode(debugMode);
        WindowGraphicCapturePlugin.CreateModule();
        _renderEventFunc = WindowGraphicCapturePlugin.GetRenderEventFunc();
    }

    void Start()
    {
        StartCoroutine(Render());
    }

    void OnApplicationQuit()
    {
        Resources.UnloadUnusedAssets();
        WindowGraphicCapturePlugin.DestroyModule();
    }

    void OnEnable()
    {
        WindowGraphicCapturePlugin.SetLogFunc(_onDebugLog);
        WindowGraphicCapturePlugin.SetErrorFunc(_onDebugErr);
    }

    void OnDisable()
    {
        WindowGraphicCapturePlugin.SetLogFunc(null);
        WindowGraphicCapturePlugin.SetErrorFunc(null);
    }

    IEnumerator Render()
    {
        for (; ; )
        {
            yield return new WaitForEndOfFrame();
            GL.IssuePluginEvent(_renderEventFunc, 0);
            WindowGraphicCapturePlugin.TriggerGpuUpload();
        }
    }

    void Update()
    {
        WindowGraphicCapturePlugin.Update();
        UpdateWindowInfo();
        UpdateMessages();
        UpdateWindowTitles();
    }

    void UpdateWindowInfo()
    {
        _cursorWindowId = WindowGraphicCapturePlugin.GetWindowIdUnderCursor();
    }

    Window AddWindow(int id)
    {
        var window = new Window(id);
        windows.Add(id, window);
        return window;
    }

    void UpdateMessages()
    {
        var messages = WindowGraphicCapturePlugin.GetMessages();

        for (int i = 0; i < messages.Length; ++i)
        {
            var message = messages[i];
            var id = message.windowId;
            switch (message.type)
            {
                case MessageType.WindowAdded:
                    {
                        var window = AddWindow(id);
                        if (window.isAlive && window.isDesktop)
                        {
                            desktops_.Add(id);
                            onDesktopAdded.Invoke(window);
                        }
                        else
                        {
                            onWindowAdded.Invoke(window);
                        }
                        break;
                    }
                case MessageType.WindowRemoved:
                    {
                        var window = Find(id);
                        if (window != null)
                        {
                            window.isAlive = false;
                            if (window.parentWindow != null)
                            {
                                window.parentWindow.onChildRemoved.Invoke(window);
                            }
                            windows.Remove(id);
                            if (window.isAlive && window.isDesktop)
                            {
                                desktops_.Remove(id);
                                onDesktopRemoved.Invoke(window);
                            }
                            else
                            {
                                onWindowRemoved.Invoke(window);
                            }
                        }
                        break;
                    }
                case MessageType.WindowCaptured:
                    {
                        var window = Find(id);
                        if (window != null)
                        {
                            window.onCaptured.Invoke();
                        }
                        break;
                    }
                case MessageType.WindowSizeChanged:
                    {
                        var window = Find(id);
                        if (window != null)
                        {
                            window.onSizeChanged.Invoke();
                        }
                        break;
                    }
                case MessageType.IconCaptured:
                    {
                        var window = Find(id);
                        if (window != null)
                        {
                            window.onIconCaptured.Invoke();
                        }
                        break;
                    }
                case MessageType.CursorCaptured:
                    {
                        cursor.onCaptured.Invoke();
                        break;
                    }
                case MessageType.TextureNullError:
                    {
                        var window = Find(id);
                        if (window != null)
                        {
                            window.ResetWindowTexture();
                        }
                        break;
                    }
                case MessageType.TextureSizeError:
                    {
                        var window = Find(id);
                        if (window != null)
                        {
                            window.ResetWindowTexture();
                        }
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }
    }

    void UpdateWindowTitles()
    {
        switch (windowTitlesUpdateTiming)
        {
            case WindowTitlesUpdateTiming.Manual:
                break;
            case WindowTitlesUpdateTiming.AlwaysAllWindows:
                UpdateAllWindowTitles();
                break;
            case WindowTitlesUpdateTiming.AlwaysAltTabWindows:
                UpdateAltTabWindowTitles();
                break;
        }
    }

    static public Window Find(int id)
    {
        Window window = null;
        windows.TryGetValue(id, out window);
        return window;
    }

    static public Window Find(string partialTitle, bool isAltTabWindow = true)
    {
        Window target = null;
        int minIndex = int.MaxValue;
        foreach (var kv in windows)
        {
            var window = kv.Value;
            if (isAltTabWindow && !window.isAltTabWindow)
            {
                continue;
            }
            int index = window.title.IndexOf(partialTitle);
            if (index == 0)
            {
                return window;
            }
            else if (index != -1 && index < minIndex)
            {
                minIndex = index;
                target = window;
            }
        }
        return target;
    }

    static public Window Find(System.IntPtr handle)
    {
        foreach (var kv in windows)
        {
            var window = kv.Value;
            if (window.handle == handle)
            {
                return window;
            }
        }
        return null;
    }

    static public Window Find(System.Func<Window, bool> func)
    {
        foreach (var kv in windows)
        {
            var window = kv.Value;
            if (func(window)) return window;
        }
        return null;
    }

    static public List<Window> FindAll(string title)
    {
        var list = new List<Window>();
        foreach (var kv in windows)
        {
            var window = kv.Value;
            if (window.title.IndexOf(title) != -1)
            {
                list.Add(window);
            }
        }
        return list;
    }

    static public Window FindParent(int id)
    {
        var parentId = WindowGraphicCapturePlugin.GetWindowParentId(id);
        if (parentId == -1) return null;

        Window parent;
        windows.TryGetValue(parentId, out parent);
        return parent;
    }

    static public Window FindDesktop(int index)
    {
        if (index < 0 || index >= desktopCount) return null;
        var id = instance.desktops_[index];
        return Find(id);
    }

    static public void UpdateAllWindowTitles()
    {
        foreach (var kv in windows)
        {
            var window = kv.Value;
            window.RequestUpdateTitle();
        }
    }

    static public void UpdateAltTabWindowTitles()
    {
        foreach (var kv in windows)
        {
            var window = kv.Value;
            if (window.isAltTabWindow)
            {
                window.RequestUpdateTitle();
            }
        }
    }
}
