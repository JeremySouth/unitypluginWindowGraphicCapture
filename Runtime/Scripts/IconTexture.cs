using UnityEngine;

namespace WindowGraphicCapture
{
    public class IconTexture : MonoBehaviour
    {
        [SerializeField] WindowTexture _windowTexture;
        public WindowTexture windowTexture
        {
            get
            {
                return _windowTexture;
            }
            set
            {
                _windowTexture = value;
                if (_windowTexture)
                {
                    window = _windowTexture.window;
                }
            }
        }
        private Window _window = null;
        public Window window
        {
            get
            {
                return _window;
            }
            set
            {
                _window = value;

                if (_window != null)
                {
                    if (!_window.hasIconTexture)
                    {
                        _window.onIconCaptured.AddListener(OnIconCaptured);
                        _window.RequestCaptureIcon();
                    }
                    else
                    {
                        OnIconCaptured();
                    }
                }
            }
        }
        private bool isValid
        {
            get
            {
                return window != null;
            }
        }
        void Update()
        {
            if (_windowTexture != null)
            {
                if (window == null || window != _windowTexture.window)
                {
                    window = _windowTexture.window;
                }
            }
        }
        void OnIconCaptured()
        {
            if (!isValid) return;

            var renderer = GetComponent<Renderer>();
            renderer.material.mainTexture = window.iconTexture;
            window.onIconCaptured.RemoveListener(OnIconCaptured);
        }
    }
}