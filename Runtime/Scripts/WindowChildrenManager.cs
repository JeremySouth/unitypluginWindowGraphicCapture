using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WindowGraphicCapture
{
    public class WindowChildrenManager : MonoBehaviour
    {
        private WindowTexture _windowTexture;
        private Dictionary<int, WindowTexture> _children = new Dictionary<int, WindowTexture>();
        void Awake()
        {
            _windowTexture = GetComponent<WindowTexture>();
            _windowTexture.onWindowChanged.AddListener(OnWindowChanged);
            OnWindowChanged(_windowTexture.window, null);
        }

        void Update()
        {
            UpdateChildren();
        }

        WindowTexture InstantiateChild()
        {
            var prefab = _windowTexture.childWindowPrefab;
            if (!prefab) return null;

            var childTexture = Instantiate(prefab, transform);
            return childTexture.GetComponent<WindowTexture>();
        }

        void OnWindowChanged(Window newWindow, Window oldWindow)
        {
            if (newWindow == oldWindow) return;

            if (oldWindow != null)
            {
                oldWindow.onChildAdded.RemoveListener(OnChildAdded);
                oldWindow.onChildRemoved.RemoveListener(OnChildRemoved);

                foreach (var kv in _children)
                {
                    var windowTexture = kv.Value;
                    Destroy(windowTexture.gameObject);
                }

                _children.Clear();
            }

            if (newWindow != null)
            {
                newWindow.onChildAdded.AddListener(OnChildAdded);
                newWindow.onChildRemoved.AddListener(OnChildRemoved);

                foreach (var pair in WindowGraphicCaptureManager.windows)
                {
                    Window window = pair.Value;
                    if (
                        !window.isAltTabWindow &&
                        window.isChild &&
                        window.parentWindow.id == newWindow.id)
                    {
                        OnChildAdded(window);
                    }
                }
            }
        }

        void OnChildAdded(Window window)
        {
            var childWindowTexture = InstantiateChild();
            if (!childWindowTexture)
            {
                Debug.LogError("childPrefab is not set or does not have UwcWindowTexture.");
                return;
            }
            childWindowTexture.window = window;
            childWindowTexture.parent = _windowTexture;
            childWindowTexture.manager = _windowTexture.manager;
            childWindowTexture.type = WindowTextureType.Child;
            childWindowTexture.captureFrameRate = _windowTexture.captureFrameRate;
            childWindowTexture.captureRequestTiming = _windowTexture.captureRequestTiming;
            childWindowTexture.drawCursor = _windowTexture.drawCursor;

            _children.Add(window.id, childWindowTexture);
        }

        void OnChildRemoved(Window window)
        {
            WindowTexture child;
            _children.TryGetValue(window.id, out child);
            if (child)
            {
                Destroy(child.gameObject);
                _children.Remove(window.id);
            }
        }

        void MoveAndScaleChildWindow(WindowTexture child)
        {
            var window = child.window;
            var parent = window.parentWindow;

            var px = parent.x;
            var py = parent.y;
            var pw = parent.width;
            var ph = parent.height;
            var cx = window.x;
            var cy = window.y;
            var cw = window.width;
            var ch = window.height;
            var dz = WindowGraphicCaptureManager.childWindowZDistance;
            var desktopX = (cw - pw) * 0.5f + (cx - px);
            var desktopY = (ch - ph) * 0.5f + (cy - py);
            var localX = desktopX / parent.width;
            var localY = -desktopY / parent.height;
            var localZ = dz * (window.zOrder - window.parentWindow.zOrder) / transform.localScale.z;
            child.transform.localPosition = new Vector3(localX, localY, localZ);

            var widthRatio = 1f * window.width / window.parentWindow.width;
            var heightRatio = 1f * window.height / window.parentWindow.height;
            child.transform.localScale = new Vector3(widthRatio, heightRatio, 1f);
        }

        void UpdateChildren()
        {
            foreach (var kv in _children)
            {
                var windowTexture = kv.Value;
                MoveAndScaleChildWindow(windowTexture);
            }
        }
    }

}
