using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WindowGraphicCapture
{
    public class WindowCursor
    {
        public WindowCursor()
        {
            onCaptured.AddListener(OnCaptured);
        }
        public int x
        {
            get { return WindowGraphicCapturePlugin.GetCursorX(); }
        }

        public int y
        {
            get { return WindowGraphicCapturePlugin.GetCursorY(); }
        }

        public int width
        {
            get { return WindowGraphicCapturePlugin.GetCursorWidth(); }
        }

        public int height
        {
            get { return WindowGraphicCapturePlugin.GetCursorHeight(); }
        }

        public Texture2D texture
        {
            get;
            private set;
        }

        WinEvent _onCaptured = new WinEvent();
        public WinEvent onCaptured
        {
            get { return _onCaptured; }
        }

        WinEvent _onTextureChanged = new WinEvent();
        public WinEvent onTextureChanged
        {
            get { return _onTextureChanged; }
        }

        public void RequestCapture()
        {
            WindowGraphicCapturePlugin.RequestCaptureCursor();
        }

        void OnCaptured()
        {
        }

        public void CreateTextureIfNeeded()
        {
            var w = width;
            var h = height;
            if (w == 0 || h == 0) return;

            if (!texture || texture.width != w || texture.height != h)
            {
                texture = new Texture2D(w, h, TextureFormat.BGRA32, false);
                texture.filterMode = FilterMode.Point;
                texture.wrapMode = TextureWrapMode.Clamp;
                WindowGraphicCapturePlugin.SetCursorTexturePtr(texture.GetNativeTexturePtr());
                onTextureChanged.Invoke();
            }
        }
    }

}