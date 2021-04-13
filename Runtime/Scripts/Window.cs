using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace WindowGraphicCapture
{
    public class Window
    {
        public Window(int id)
        {
            this.id = id;
            isAlive = true;

            onCaptured.AddListener(OnCaptured);
            onSizeChanged.AddListener(OnSizeChanged);
            onIconCaptured.AddListener(OnIconCaptured);

            CreateIconTexture();

            //parentWindow = WindowGraphicCaptureManager.FindParent(id);
            if (parentWindow != null)
            {
                parentWindow.onChildAdded.Invoke(this);
            }
        }
        public int id
        {
            get;
            private set;
        }

        public Window parentWindow
        {
            get;
            private set;
        }

        public System.IntPtr handle
        {
            get { return WindowGraphicCapturePlugin.GetWindowHandle(id); }
        }

        public System.IntPtr ownerHandle
        {
            get { return WindowGraphicCapturePlugin.GetWindowOwnerHandle(id); }
        }

        public System.IntPtr parentHandle
        {
            get { return WindowGraphicCapturePlugin.GetWindowParentHandle(id); }
        }

        public System.IntPtr instance
        {
            get { return WindowGraphicCapturePlugin.GetWindowInstance(id); }
        }

        public int processId
        {
            get { return WindowGraphicCapturePlugin.GetWindowProcessId(id); }
        }

        public int threadId
        {
            get { return WindowGraphicCapturePlugin.GetWindowThreadId(id); }
        }

        public bool isValid
        {
            get { return WindowGraphicCapturePlugin.CheckWindowExistence(id); }
        }

        public bool isAlive
        {
            get;
            set;
        }

        public bool isRoot
        {
            get { return parentWindow == null; }
        }

        public bool isChild
        {
            get { return !isRoot; }
        }

        public bool isVisible
        {
            get { return WindowGraphicCapturePlugin.IsWindowsVisible(id); }
        }

        public bool isAltTabWindow
        {
            get { return WindowGraphicCapturePlugin.IsAltTabWindows(id); }
        }

        public bool isDesktop
        {
            get { return WindowGraphicCapturePlugin.IsDesktops(id); }
        }

        public bool isEnabled
        {
            get { return WindowGraphicCapturePlugin.IsWindowsEnabled(id); }
        }

        public bool isUnicode
        {
            get { return WindowGraphicCapturePlugin.IsWindowsUnicode(id); }
        }

        public bool isZoomed
        {
            get { return WindowGraphicCapturePlugin.IsWindowsZoomed(id); }
        }

        public bool isMaximized
        {
            get { return isZoomed; }
        }

        public bool isIconic
        {
            get { return WindowGraphicCapturePlugin.IsWindowsIconic(id); }
        }

        public bool isMinimized
        {
            get { return isIconic; }
        }

        public bool isHungup
        {
            get { return WindowGraphicCapturePlugin.IsWindowsHungUp(id); }
        }

        public bool isTouchable
        {
            get { return WindowGraphicCapturePlugin.IsWindowsTouchable(id); }
        }

        public bool isApplicationFrameWindow
        {
            get { return WindowGraphicCapturePlugin.IsWindowsApplicationsFrameWindow(id); }
        }

        public bool isUWP
        {
            get { return WindowGraphicCapturePlugin.IsWindowsUWP(id); }
        }

        public bool isBackground
        {
            get { return WindowGraphicCapturePlugin.IsWindowsBackground(id); }
        }

        public string title
        {
            get { return WindowGraphicCapturePlugin.GetWindowTitle(id); }
        }

        public string className
        {
            get { return WindowGraphicCapturePlugin.GetWindowClassName(id); }
        }

        public int rawX
        {
            get { return WindowGraphicCapturePlugin.GetWindowX(id); }
        }

        public int rawY
        {
            get { return WindowGraphicCapturePlugin.GetWindowY(id); }
        }

        public int rawWidth
        {
            get { return WindowGraphicCapturePlugin.GetWindowWidth(id); }
        }

        public int rawHeight
        {
            get { return WindowGraphicCapturePlugin.GetWindowHeight(id); }
        }

        public int x
        {
            get { return rawX + WindowGraphicCapturePlugin.GetWindowTextureOffsetX(id); }
        }

        public int y
        {
            get { return rawY + WindowGraphicCapturePlugin.GetWindowTextureOffsetY(id); }
        }

        public int width
        {
            get { return WindowGraphicCapturePlugin.GetWindowTextureWidth(id); }
        }

        public int height
        {
            get { return WindowGraphicCapturePlugin.GetWindowTextureHeight(id); }
        }

        public int zOrder
        {
            get { return WindowGraphicCapturePlugin.GetWindowZOrder(id); }
        }

        public System.IntPtr buffer
        {
            get { return WindowGraphicCapturePlugin.GetWindowBuffer(id); }
        }

        public int textureOffsetX
        {
            get { return WindowGraphicCapturePlugin.GetWindowTextureOffsetX(id); }
        }

        public int textureOffsetY
        {
            get { return WindowGraphicCapturePlugin.GetWindowTextureOffsetY(id); }
        }

        public int iconWidth
        {
            get { return WindowGraphicCapturePlugin.GetWindowIconWidth(id); }
        }

        public int iconHeight
        {
            get { return WindowGraphicCapturePlugin.GetWindowIconHeight(id); }
        }

        private Texture2D _backTexture;
        private bool _willTextureSizeChange = false;
        public Texture2D texture
        {
            get;
            private set;
        }

        private Texture2D _iconTexture;
        private bool _hasIconTextureCaptured = false;
        public bool hasIconTexture
        {
            get { return _hasIconTextureCaptured; }
        }

        public Texture2D iconTexture
        {
            get { return _hasIconTextureCaptured ? iconTexture : null; }
        }

        public CaptureMode captureMode
        {
            get { return WindowGraphicCapturePlugin.GetWindowCaptureMode(id); }
            set { WindowGraphicCapturePlugin.SetWindowCaptureMode(id, value); }
        }

        public bool cursorDraw
        {
            get { return WindowGraphicCapturePlugin.GetWindowCursorDraw(id); }
            set { WindowGraphicCapturePlugin.SetWindowCursorDraw(id, value); }
        }

        private UnityEvent onCaptured_ = new UnityEvent();
        public UnityEvent onCaptured
        {
            get { return onCaptured_; }
        }

        private bool isFirstSizeChangedEvent_ = true;
        private UnityEvent onSizeChanged_ = new UnityEvent();
        public UnityEvent onSizeChanged
        {
            get { return onSizeChanged_; }
        }

        private UnityEvent onIconCaptured_ = new UnityEvent();
        public UnityEvent onIconCaptured
        {
            get { return onIconCaptured_; }
        }

        public class ChildAddedEvent : UnityEvent<Window> { }
        private ChildAddedEvent onChildAdded_ = new ChildAddedEvent();
        public ChildAddedEvent onChildAdded
        {
            get { return onChildAdded_; }
        }

        public class ChildRemovedEvent : UnityEvent<Window> { }
        private ChildRemovedEvent onChildRemoved_ = new ChildRemovedEvent();
        public ChildRemovedEvent onChildRemoved
        {
            get { return onChildRemoved_; }
        }

        public void RequestUpdateTitle()
        {
            WindowGraphicCapturePlugin.RequestUpdateWindowTitle(id);
        }

        public void RequestCaptureIcon()
        {
            WindowGraphicCapturePlugin.RequestCaptureIcon(id);
        }

        public void RequestCapture(CapturePriority priority = CapturePriority.High)
        {
            if (!texture)
            {
                CreateWindowTexture();
            }
            WindowGraphicCapturePlugin.RequestCaptureWindow(id, priority);
        }

        void OnSizeChanged()
        {
            if (isFirstSizeChangedEvent_)
            {
                isFirstSizeChangedEvent_ = false;
                return;
            }

            CreateWindowTexture();
        }

        void OnCaptured()
        {
            UpdateWindowTexture();
        }

        void OnIconCaptured()
        {
            _hasIconTextureCaptured = true;
        }

        void CreateWindowTexture(bool force = false)
        {
            var w = width;
            var h = height;
            if (w <= 0 || h <= 0) return;

            if (force || !texture || texture.width != w || texture.height != h)
            {
                if (_backTexture)
                {
                    Object.DestroyImmediate(_backTexture);
                }
                try
                {
                    _backTexture = new Texture2D(w, h, TextureFormat.BGRA32, false);
                    WindowGraphicCapturePlugin.SetWindowTexturePtr(id, _backTexture.GetNativeTexturePtr());
                    _willTextureSizeChange = true;
                }
                catch (System.Exception e)
                {
                    Debug.LogError(e.Message);
                    Debug.LogErrorFormat("Width: {0}, Height: {1}", w, h);
                }
            }
        }

        void UpdateWindowTexture()
        {
            if (_willTextureSizeChange)
            {
                if (texture)
                {
                    Object.DestroyImmediate(texture);
                }
                texture = _backTexture;
                _backTexture = null;
                _willTextureSizeChange = false;
            }
        }

        public void ResetWindowTexture()
        {
            CreateWindowTexture(true);
        }

        void CreateIconTexture()
        {
            var w = iconWidth;
            var h = iconHeight;
            if (w == 0 || h == 0) return;
            _iconTexture = new Texture2D(w, h, TextureFormat.BGRA32, false);
            _iconTexture.filterMode = FilterMode.Point;
            WindowGraphicCapturePlugin.SetWindowIconTexturePtr(id, _iconTexture.GetNativeTexturePtr());
        }

        public Color32[] GetPixels(int x, int y, int width, int height)
        {
            return WindowGraphicCapturePlugin.GetWindowPixels(id, x, y, width, height);
        }

        public bool GetPixels(Color32[] colors, int x, int y, int width, int height)
        {
            return WindowGraphicCapturePlugin.GetWindowPixels(id, colors, x, y, width, height);
        }

        public Color32 GetPixel(int x, int y)
        {
            return WindowGraphicCapturePlugin.GetWindowPixel(id, x, y);
        }
    }
}