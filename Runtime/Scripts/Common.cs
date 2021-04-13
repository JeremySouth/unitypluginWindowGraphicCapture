using UnityEngine;
using UnityEngine.Events;

namespace WindowGraphicCapture
{
    public enum WindowTextureType
    {
        Window = 0,
        Desktop = 1,
        Child = 2,
    }
    public enum WindowTextureCaptureTiming
    {
        EveryFrame = 0,
        OnlyWhenVisible = 1,
        Manual = 2,
    }
    public enum WindowTextureScaleControlType
    {
        BaseScale = 0,
        FixedWidth = 1,
        FixedHeight = 2,
        Manual = 3,
    }
    public enum WindowSearchTiming
    {
        Always = 0,
        Manual = 1,
        OnlyWhenParameterChanged = 2,
    }
    public enum WindowTitlesUpdateTiming
    {
        Manual = 0,
        AlwaysAllWindows = 1,
        AlwaysAltTabWindows = 2,
    }
    public class WinEvent : UnityEvent
    {
    }

    public class WindowEvent : UnityEvent<Window>
    {
    }

    public class WindowChangeEvent : UnityEvent<Window, Window>
    {
    }

    public class WindowTextureEvent : UnityEvent<WindowTexture>
    {
    }
    public struct RayCastResult
    {
        public bool hit;
        public WindowTexture texture;
        public Vector3 position;
        public Vector3 normal;
        public Vector2 windowCoord;
        public Vector2 desktopCoord;
    }
}
