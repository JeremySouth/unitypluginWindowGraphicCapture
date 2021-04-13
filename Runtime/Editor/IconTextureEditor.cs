using UnityEngine;
using UnityEditor;

namespace WindowGraphicCapture
{
    [CustomEditor(typeof(IconTexture))]
    public class UwcIconTextureEditor : Editor
    {
        IconTexture texture
        {
            get { return target as IconTexture; }
        }

        public override void OnInspectorGUI()
        {
            var windowTexture = (WindowTexture)EditorGUILayout.ObjectField("Window Texture", texture.windowTexture, typeof(WindowTexture), true);
            if (texture.windowTexture != windowTexture)
            {
                texture.windowTexture = windowTexture;
            }
        }
    }
}
