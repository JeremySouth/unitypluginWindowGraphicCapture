using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WindowGraphicCapture
{
    public class WindowCursorTexture : MonoBehaviour
    {
        Renderer _renderer;
        Material _material;

        WindowCursor cursor
        {
            get { return WindowGraphicCaptureManager.cursor; }
        }

        void Awake()
        {
            _renderer = GetComponent<Renderer>();
            _material = _renderer.material;
            cursor.onTextureChanged.AddListener(OnTextureChanged);
        }

        void Update()
        {
            cursor.CreateTextureIfNeeded();
            cursor.RequestCapture();
        }

        void OnTextureChanged()
        {
            _material.mainTexture = cursor.texture;
        }
    }

}

