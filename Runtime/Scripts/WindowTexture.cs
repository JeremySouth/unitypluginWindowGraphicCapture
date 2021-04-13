using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WindowGraphicCapture
{
    public class WindowTexture : MonoBehaviour
    {
        [SerializeField]
        private bool _shouldUpdateWindow = true;
        public bool shouldUpdateWindow
        {
            get
            {
                return _shouldUpdateWindow;
            }
            set
            {
                if (value && _searchTiming == WindowSearchTiming.Manual) return;
                _shouldUpdateWindow = value;
            }
        }
        [SerializeField]
        private WindowSearchTiming _searchTiming = WindowSearchTiming.OnlyWhenParameterChanged;
        public WindowSearchTiming searchTiming
        {
            get
            {
                return _searchTiming;
            }
            set
            {
                _searchTiming = value;
                shouldUpdateWindow = true;
            }
        }
        [SerializeField]
        private WindowTextureType _type = WindowTextureType.Window;
        public WindowTextureType type
        {
            get
            {
                return _type;
            }
            set
            {
                shouldUpdateWindow = true;
                _type = value;
            }
        }
        [SerializeField]
        private bool _altTabWindow = false;
        public bool altTabWindow
        {
            get
            {
                return _altTabWindow;
            }
            set
            {
                shouldUpdateWindow = true;
                _altTabWindow = value;
            }
        }
        [SerializeField]
        private bool _createChildWindows = true;
        public bool createChildWindows
        {
            get
            {
                return _createChildWindows;
            }
            set
            {
                _createChildWindows = value;

                var manager = GetComponent<WindowChildrenManager>();
                if (_createChildWindows)
                {
                    if (!manager)
                    {
                        gameObject.AddComponent<WindowChildrenManager>();
                    }
                }
                else
                {
                    if (manager)
                    {
                        Destroy(manager);
                    }
                }
            }
        }
        [SerializeField]
        private string _partialWindowTitle;
        public string partialWindowTitle
        {
            get
            {
                return _partialWindowTitle;
            }
            set
            {
                shouldUpdateWindow = true;
                _partialWindowTitle = value;
            }
        }
        [SerializeField]
        private int _desktopIndex = 0;
        public int desktopIndex
        {
            get
            {
                return _desktopIndex;
            }
            set
            {
                shouldUpdateWindow = true;
                _desktopIndex = (WindowGraphicCaptureManager.desktopCount > 0) ?
                    Mathf.Clamp(value, 0, WindowGraphicCaptureManager.desktopCount - 1) : 0;
            }
        }
        public GameObject childWindowPrefab;
        public float childWindowZDistance = WindowGraphicCaptureManager.childWindowZDistance;
        public CaptureMode captureMode = CaptureMode.PrintWindow;
        public CapturePriority capturePriority = CapturePriority.Auto;
        public WindowTextureCaptureTiming captureRequestTiming = WindowTextureCaptureTiming.OnlyWhenVisible;
        public int captureFrameRate = 30;
        public bool drawCursor = true;
        public bool updateTitle = true;
        public bool searchAnotherWindowWhenInvalid = false;

        public WindowTextureScaleControlType scaleControlType = WindowTextureScaleControlType.BaseScale;
        public float scalePer1000Pixel = 1f;

        private static HashSet<WindowTexture> _list = new HashSet<WindowTexture>();
        public static HashSet<WindowTexture> list
        {
            get { return _list; }
        }

        Window _window;
        public Window window
        {
            get
            {
                return _window;
            }
            set
            {
                if (_window == value)
                {
                    return;
                }

                if (_window != null)
                {
                    _window.onCaptured.RemoveListener(OnCaptured);
                }

                var old = _window;
                _window = value;
                _onWindowChanged.Invoke(_window, old);

                if (_window != null)
                {
                    shouldUpdateWindow = false;
                    if (_window.isDesktop || _window.isChild)
                    {
                        captureMode = _window.captureMode;
                    }
                    _window.onCaptured.AddListener(OnCaptured);
                    _window.RequestCapture(CapturePriority.High);
                }
            }
        }

        public WindowTextureManager manager { get; set; }
        public WindowTexture parent { get; set; }

        private WindowChangeEvent _onWindowChanged = new WindowChangeEvent();
        public WindowChangeEvent onWindowChanged
        {
            get { return _onWindowChanged; }
        }

        float basePixel
        {
            get { return 1000f / scalePer1000Pixel; }
        }

        public bool isValid
        {
            get
            {
                return window != null && window.isValid;
            }
        }

        Material material_;
        Renderer renderer_;
        MeshFilter meshFilter_;
        Collider collider_;
        float captureTimer_ = 0f;
        bool hasBeenCaptured_ = false;

        void Awake()
        {
            renderer_ = GetComponent<Renderer>();
            material_ = renderer_.material; // clone
            meshFilter_ = GetComponent<MeshFilter>();
            collider_ = GetComponent<Collider>();

            _list.Add(this);
        }

        void OnDestroy()
        {
            _list.Remove(this);
        }

        void Update()
        {
            UpdateSearchTiming();
            UpdateTargetWindow();

            if (!isValid)
            {
                material_.mainTexture = null;
                return;
            }

            UpdateTexture();
            UpdateRenderer();
            UpdateScale();

            if (updateTitle && isValid)
            {
                window.RequestUpdateTitle();
            }

            if (captureRequestTiming == WindowTextureCaptureTiming.EveryFrame)
            {
                RequestCapture();
            }

            captureTimer_ += Time.deltaTime;

            UpdateBasicComponents();
        }

        void OnWillRenderObject()
        {
            if (captureRequestTiming == WindowTextureCaptureTiming.OnlyWhenVisible)
            {
                RequestCapture();
            }
        }

        void UpdateTexture()
        {
            if (!isValid) return;

            window.cursorDraw = drawCursor;

            if (material_.mainTexture != window.texture)
            {
                material_.mainTexture = window.texture;
            }
        }

        void UpdateRenderer()
        {
            if (hasBeenCaptured_)
            {
                renderer_.enabled = !window.isIconic && window.isVisible;
            }
        }

        void UpdateScale()
        {
            if (!isValid || window.isChild) return;

            var scale = transform.localScale;

            switch (scaleControlType)
            {
                case WindowTextureScaleControlType.BaseScale:
                    {
                        var extents = meshFilter_.sharedMesh.bounds.extents;
                        var meshWidth = extents.x * 2f;
                        var meshHeight = extents.y * 2f;
                        var baseHeight = meshHeight * basePixel;
                        var baseWidth = meshWidth * basePixel;
                        scale.x = window.width / baseWidth;
                        scale.y = window.height / baseHeight;
                        break;
                    }
                case WindowTextureScaleControlType.FixedWidth:
                    {
                        scale.y = transform.localScale.x * window.height / window.width;
                        break;
                    }
                case WindowTextureScaleControlType.FixedHeight:
                    {
                        scale.x = transform.localScale.y * window.width / window.height;
                        break;
                    }
                case WindowTextureScaleControlType.Manual:
                    {
                        break;
                    }
            }

            transform.localScale = scale;
        }

        void UpdateSearchTiming()
        {
            if (searchTiming == WindowSearchTiming.Always)
            {
                shouldUpdateWindow = true;
            }
        }

        void UpdateTargetWindow()
        {
            if (!shouldUpdateWindow) return;

            switch (type)
            {
                case WindowTextureType.Window:
                    window = WindowGraphicCaptureManager.Find(partialWindowTitle, altTabWindow);
                    break;
                case WindowTextureType.Desktop:
                    window = WindowGraphicCaptureManager.FindDesktop(desktopIndex);
                    break;
                case WindowTextureType.Child:
                    break;
            }
        }

        void UpdateBasicComponents()
        {
            if (renderer_) renderer_.enabled = isValid;
            if (collider_) collider_.enabled = isValid;
        }

        void OnCaptured()
        {
            hasBeenCaptured_ = true;
        }

        public void RequestCapture()
        {
            if (!isValid) return;

            window.captureMode = captureMode;

            float T = 1f / captureFrameRate;
            if (captureTimer_ < T) return;

            while (captureTimer_ > T)
            {
                captureTimer_ -= T;
            }

            var priority = capturePriority;
            if (priority == CapturePriority.Auto)
            {
                priority = CapturePriority.Low;
                if (window == WindowGraphicCaptureManager.cursorWindow)
                {
                    priority = CapturePriority.High;
                }
                else if (window.zOrder < WindowGraphicCaptureManager.MiddlePriorityMaxZ)
                {
                    priority = CapturePriority.Middle;
                }
            }

            window.RequestCapture(priority);
        }

        public void RequestWindowUpdate()
        {
            shouldUpdateWindow = true;
        }

        static public RayCastResult RayCast(Vector3 from, Vector3 dir, float distance, LayerMask layerMask)
        {
            var ray = new Ray();
            ray.origin = from;
            ray.direction = dir;
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, distance, layerMask))
            {
                var collider = hit.collider;
                var texture =
                    collider.GetComponent<WindowTexture>() ??
                    collider.GetComponentInChildren<WindowTexture>();
                if (texture)
                {
                    var window = texture.window;
                    var meshFilter = texture.GetComponent<MeshFilter>();
                    if (window != null && meshFilter && meshFilter.sharedMesh)
                    {
                        var localPos = texture.transform.InverseTransformPoint(hit.point);
                        var meshScale = 2f * meshFilter.sharedMesh.bounds.extents;
                        var windowLocalX = (int)((localPos.x / meshScale.x + 0.5f) * window.width);
                        var windowLocalY = (int)((0.5f - localPos.y / meshScale.y) * window.height);
                        var desktopX = window.x + windowLocalX;
                        var desktopY = window.y + windowLocalY;
                        return new RayCastResult
                        {
                            hit = true,
                            texture = texture,
                            position = hit.point,
                            normal = hit.normal,
                            windowCoord = new Vector2(windowLocalX, windowLocalY),
                            desktopCoord = new Vector2(desktopX, desktopY),
                        };
                    }
                }
            }

            return new RayCastResult()
            {
                hit = false,
            };
        }
    }

}

