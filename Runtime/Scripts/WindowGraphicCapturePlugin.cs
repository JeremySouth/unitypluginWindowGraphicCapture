using UnityEngine;
using System;
using System.Runtime.InteropServices;

namespace WindowGraphicCapture
{
    public enum DebugMode
    {
        None = 0,
        File = 1,
        UnityLog = 2,
    }
    public enum CaptureMode
    {
        None = -1,
        PrintWindow = 0,
        BitBlt = 1,
    }
    public enum CapturePriority
    {
        Auto = -1,
        High = 0,
        Middle = 1,
        Low = 2,
    }
    public enum MessageType
    {
        None = -1,
        WindowAdded = 0,
        WindowRemoved = 1,
        WindowCaptured = 2,
        WindowSizeChanged = 3,
        IconCaptured = 4,
        CursorCaptured = 5,
        Error = 1000,
        TextureNullError = 1001,
        TextureSizeError = 1002,
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct Message
    {
        [MarshalAs(UnmanagedType.I4)]
        public MessageType type;
        [MarshalAs(UnmanagedType.I4)]
        public int windowId;
        [MarshalAs(UnmanagedType.I8)]
        public IntPtr userData;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Point
    {
        [MarshalAs(UnmanagedType.I4)]
        public int x;
        [MarshalAs(UnmanagedType.I4)]
        public int y;
    }

    public class WindowGraphicCapturePlugin
    {
        public const string pluginName = "libWindowGraphicCapture_x64";
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void DebugLogDelegate(string str);

        [DllImport(pluginName, EntryPoint = "CreateModule")] public static extern void CreateModule();
        [DllImport(pluginName, EntryPoint = "DestroyModule")] public static extern void DestroyModule();
        [DllImport(pluginName, EntryPoint = "IsActiveModule")] public static extern bool IsActiveModule();
        [DllImport(pluginName, EntryPoint = "Update")] public static extern void Update();
        [DllImport(pluginName, EntryPoint = "SetDebugMode")] public static extern void SetDebugMode(DebugMode mode);
        [DllImport(pluginName, EntryPoint = "SetLogFunc")] public static extern void SetLogFunc(DebugLogDelegate func);
        [DllImport(pluginName, EntryPoint = "SetErrorFunc")] public static extern void SetErrorFunc(DebugLogDelegate func);
        [DllImport(pluginName, EntryPoint = "GetRenderEventFunc")] public static extern IntPtr GetRenderEventFunc();
        [DllImport(pluginName, EntryPoint = "TriggerGpuUpload")] public static extern void TriggerGpuUpload();
        [DllImport(pluginName, EntryPoint = "GetMessageCount")] private static extern int GetMessageCount();
        [DllImport(pluginName, EntryPoint = "GetMessages")] private static extern IntPtr GetMessages_Internal();
        [DllImport(pluginName, EntryPoint = "ClearMessages")] private static extern void ClearMessages();
        [DllImport(pluginName, EntryPoint = "CheckWindowExistence")] public static extern bool CheckWindowExistence(int id);
        [DllImport(pluginName, EntryPoint = "GetWindowHandle")] public static extern IntPtr GetWindowHandle(int id);
        [DllImport(pluginName, EntryPoint = "GetWindowParentId")] public static extern int GetWindowParentId(int id);
        [DllImport(pluginName, EntryPoint = "GetWindowOwnerHandle")] public static extern IntPtr GetWindowOwnerHandle(int id);
        [DllImport(pluginName, EntryPoint = "GetWindowParentHandle")] public static extern IntPtr GetWindowParentHandle(int id);
        [DllImport(pluginName, EntryPoint = "GetWindowInstance")] public static extern IntPtr GetWindowInstance(int id);
        [DllImport(pluginName, EntryPoint = "GetWindowProcessId")] public static extern int GetWindowProcessId(int id);
        [DllImport(pluginName, EntryPoint = "GetWindowThreadId")] public static extern int GetWindowThreadId(int id);
        [DllImport(pluginName, EntryPoint = "RequestUpdateWindowTitle")] public static extern void RequestUpdateWindowTitle(int id);
        [DllImport(pluginName, EntryPoint = "RequestCaptureWindow")] public static extern void RequestCaptureWindow(int id, CapturePriority priority);
        [DllImport(pluginName, EntryPoint = "RequestCaptureIcon")] public static extern void RequestCaptureIcon(int id);
        [DllImport(pluginName, EntryPoint = "GetWindowX")] public static extern int GetWindowX(int id);
        [DllImport(pluginName, EntryPoint = "GetWindowY")] public static extern int GetWindowY(int id);
        [DllImport(pluginName, EntryPoint = "GetWindowWidth")] public static extern int GetWindowWidth(int id);
        [DllImport(pluginName, EntryPoint = "GetWindowHeight")] public static extern int GetWindowHeight(int id);
        [DllImport(pluginName, EntryPoint = "GetWindowZOrder")] public static extern int GetWindowZOrder(int id);
        [DllImport(pluginName, EntryPoint = "GetWindowBuffer")] public static extern IntPtr GetWindowBuffer(int id);
        [DllImport(pluginName, EntryPoint = "GetWindowTextureWidth")] public static extern int GetWindowTextureWidth(int id);
        [DllImport(pluginName, EntryPoint = "GetWindowTextureHeight")] public static extern int GetWindowTextureHeight(int id);
        [DllImport(pluginName, EntryPoint = "GetWindowTextureOffsetX")] public static extern int GetWindowTextureOffsetX(int id);
        [DllImport(pluginName, EntryPoint = "GetWindowTextureOffsetY")] public static extern int GetWindowTextureOffsetY(int id);
        [DllImport(pluginName, EntryPoint = "GetWindowIconWidth")] public static extern int GetWindowIconWidth(int id);
        [DllImport(pluginName, EntryPoint = "GetWindowIconHeight")] public static extern int GetWindowIconHeight(int id);
        [DllImport(pluginName, EntryPoint = "GetWindowTitleLength")] private static extern int GetWindowTitleLength(int id);
        [DllImport(pluginName, EntryPoint = "GetWindowTitle", CharSet = CharSet.Unicode)]  private static extern IntPtr GetWindowTitle_Internal(int id);
        [DllImport(pluginName, EntryPoint = "GetWindowClassNameLength")]  private static extern int GetWindowClassNameLength(int id);
        [DllImport(pluginName, EntryPoint = "GetWindowClassName", CharSet = CharSet.Ansi)] private static extern IntPtr GetWindowClassName_Internal(int id);
        [DllImport(pluginName, EntryPoint = "GetWindowTexturePtr")] public static extern IntPtr GetWindowTexturePtr(int id);
        [DllImport(pluginName, EntryPoint = "SetWindowTexturePtr")] public static extern void SetWindowTexturePtr(int id, IntPtr texturePtr);
        [DllImport(pluginName, EntryPoint = "GetWindowIconTexturePtr")] public static extern IntPtr GetWindowIconTexturePtr(int id);
        [DllImport(pluginName, EntryPoint = "SetWindowIconTexturePtr")] public static extern void SetWindowIconTexturePtr(int id, IntPtr texturePtr);
        [DllImport(pluginName, EntryPoint = "GetWindowCaptureMode")] public static extern CaptureMode GetWindowCaptureMode(int id);
        [DllImport(pluginName, EntryPoint = "SetWindowCaptureMode")] public static extern void SetWindowCaptureMode(int id, CaptureMode mode);
        [DllImport(pluginName, EntryPoint = "GetWindowCursorDraw")] public static extern bool GetWindowCursorDraw(int id);
        [DllImport(pluginName, EntryPoint = "SetWindowCursorDraw")] public static extern void SetWindowCursorDraw(int id, bool draw);
        [DllImport(pluginName, EntryPoint = "IsWindows")] public static extern bool IsWindows(int id);
        [DllImport(pluginName, EntryPoint = "IsWindowsVisible")] public static extern bool IsWindowsVisible(int id);
        [DllImport(pluginName, EntryPoint = "IsAltTabWindows")] public static extern bool IsAltTabWindows(int id);
        [DllImport(pluginName, EntryPoint = "IsDesktops")] public static extern bool IsDesktops(int id);
        [DllImport(pluginName, EntryPoint = "IsWindowsEnabled")] public static extern bool IsWindowsEnabled(int id);
        [DllImport(pluginName, EntryPoint = "IsWindowsUnicode")] public static extern bool IsWindowsUnicode(int id);
        [DllImport(pluginName, EntryPoint = "IsWindowsZoomed")] public static extern bool IsWindowsZoomed(int id);
        [DllImport(pluginName, EntryPoint = "IsWindowsIconic")] public static extern bool IsWindowsIconic(int id);
        [DllImport(pluginName, EntryPoint = "IsWindowsHungUp")] public static extern bool IsWindowsHungUp(int id);
        [DllImport(pluginName, EntryPoint = "IsWindowsTouchable")] public static extern bool IsWindowsTouchable(int id);
        [DllImport(pluginName, EntryPoint = "IsWindowsApplicationFrameWindow")] public static extern bool IsWindowsApplicationsFrameWindow(int id);
        [DllImport(pluginName, EntryPoint = "IsWindowsUWP")] public static extern bool IsWindowsUWP(int id);
        [DllImport(pluginName, EntryPoint = "IsWindowsBackground")] public static extern bool IsWindowsBackground(int id);
        [DllImport(pluginName, EntryPoint = "GetWindowPixel")] public static extern Color32 GetWindowPixel(int id, int x, int y);
        [DllImport(pluginName, EntryPoint = "GetWindowPixels")] private static extern bool GetWindowPixels_Internal(int id, IntPtr output, int x, int y, int width, int height);
        [DllImport(pluginName, EntryPoint = "RequestCaptureCursor")] public static extern void RequestCaptureCursor();
        [DllImport(pluginName, EntryPoint = "GetCursorPosition")] public static extern Point GetCursorPosition();
        [DllImport(pluginName, EntryPoint = "GetWindowIdFromPoint")] public static extern int GetWindowIdFromPoint(int x, int y);
        [DllImport(pluginName, EntryPoint = "GetWindowIdUnderCursor")] public static extern int GetWindowIdUnderCursor();
        [DllImport(pluginName, EntryPoint = "GetCursorX")] public static extern int GetCursorX();
        [DllImport(pluginName, EntryPoint = "GetCursorY")] public static extern int GetCursorY();
        [DllImport(pluginName, EntryPoint = "GetCursorWidth")] public static extern int GetCursorWidth();
        [DllImport(pluginName, EntryPoint = "GetCursorHeight")] public static extern int GetCursorHeight();
        [DllImport(pluginName, EntryPoint = "SetCursorTexturePtr")] public static extern void SetCursorTexturePtr(IntPtr ptr);
        [DllImport(pluginName, EntryPoint = "GetScreenX")] public static extern int GetScreenX();
        [DllImport(pluginName, EntryPoint = "GetScreenY")] public static extern int GetScreenY();
        [DllImport(pluginName, EntryPoint = "GetScreenWidth")] public static extern int GetScreenWidth();
        [DllImport(pluginName, EntryPoint = "GetScreenHeight")] public static extern int GetScreenHeight();
        public static Message[] GetMessages()
        {
            var count = GetMessageCount();
            var messages = new Message[count];

            if (count == 0) return messages;

            var ptr = GetMessages_Internal();
            var size = Marshal.SizeOf(typeof(Message));

            for (int i = 0; i < count; ++i)
            {
                var data = new IntPtr(ptr.ToInt64() + (size * i));
                messages[i] = (Message)Marshal.PtrToStructure(data, typeof(Message));
            }

            ClearMessages();

            return messages;
        }
        public static string GetWindowTitle(int id)
        {
            var len = GetWindowTitleLength(id);
            var ptr = GetWindowTitle_Internal(id);
            if (ptr != IntPtr.Zero)
            {
                return Marshal.PtrToStringUni(ptr, len);
            }
            else
            {
                return "";
            }
        }
        public static string GetWindowClassName(int id)
        {
            var len = GetWindowClassNameLength(id);
            var ptr = GetWindowClassName_Internal(id);
            if (ptr != IntPtr.Zero)
            {
                return Marshal.PtrToStringAnsi(ptr, len);
            }
            else
            {
                return "";
            }
        }
        public static Color32[] GetWindowPixels(int id, int x, int y, int width, int height)
        {
            var color = new Color32[width * height];
            GetWindowPixels(id, color, x, y, width, height);
            return color;
        }
        public static bool GetWindowPixels(int id, Color32[] colors, int x, int y, int width, int height)
        {
            if (colors.Length < width * height)
            {
                Debug.LogErrorFormat("colors is smaller than (width * height).", id, x, y, width, height);
                return false;
            }
            var handle = GCHandle.Alloc(colors, GCHandleType.Pinned);
            var ptr = handle.AddrOfPinnedObject();
            if (!GetWindowPixels_Internal(id, ptr, x, y, width, height))
            {
                Debug.LogErrorFormat("GetWindowPixels({0}, {1}, {2}, {3}, {4}) failed.", id, x, y, width, height);
                return false;
            }
            handle.Free();
            return true;
        }
    }

}