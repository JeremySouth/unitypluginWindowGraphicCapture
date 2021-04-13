using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WindowGraphicCapture
{
    public class WindowTextureManager : MonoBehaviour
    {
        private GameObject _windowPrefab;
        private Dictionary<int, WindowTexture> _windowTextures = new Dictionary<int, WindowTexture>();
        public Dictionary<int, WindowTexture> windowsTextures
        {
            get { return _windowTextures; }
        }
        WindowTextureEvent _onWindowTextureAdded = new WindowTextureEvent();
        public WindowTextureEvent onWindowTextureAdded
        {
            get { return _onWindowTextureAdded; }
        }
        WindowTextureEvent _onWindowTextureRemoved = new WindowTextureEvent();
        public WindowTextureEvent onWindowTextureRemoved
        {
            get { return _onWindowTextureRemoved; }
        }

        public WindowTexture AddWindowTexture(Window window)
        {
            if (!_windowPrefab)
            {
                Debug.LogError("windowPrefab is null.");
                return null;
            }

            var obj = Instantiate(_windowPrefab, transform);
            var windowTexture = obj.GetComponent<WindowTexture>();
            if(windowTexture != null)
            {
                windowTexture.window = window;
                windowTexture.manager = this;

                _windowTextures.Add(window.id, windowTexture);
                onWindowTextureAdded.Invoke(windowTexture);
            }
            return windowTexture;
        }

        public void RemoveWindowTexture(Window window)
        {
            WindowTexture windowTexture;
            _windowTextures.TryGetValue(window.id, out windowTexture);
            if (windowTexture)
            {
                onWindowTextureRemoved.Invoke(windowTexture);
                _windowTextures.Remove(window.id);
                Destroy(windowTexture.gameObject);
            }
        }
        public WindowTexture Get(int id)
        {
            WindowTexture windowTexture = null;
            _windowTextures.TryGetValue(id, out windowTexture);
            return windowTexture;
        }
    }

}
