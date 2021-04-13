using UnityEngine;
using UnityEditor;

namespace WindowGraphicCapture
{
    [CustomEditor(typeof(WindowGraphicCaptureManager))]
    public class UwcManagerEditor : Editor
    {
        public WindowGraphicCaptureManager manager
        {
            get { return target as WindowGraphicCaptureManager; }
        }
        public SerializedProperty windowTitlesUpdateTiming;

        private void OnEnable()
        {
            windowTitlesUpdateTiming = serializedObject.FindProperty("windowTitlesUpdateTiming");
        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            Draw();
            serializedObject.ApplyModifiedProperties();
        }
        private void Draw()
        {
            var debugMode = (DebugMode)EditorGUILayout.EnumPopup("Debug Mode", manager.debugModeFromInspector);
            if (debugMode != manager.debugModeFromInspector)
            {
                manager.debugModeFromInspector = debugMode;
            }

            EditorGUILayout.PropertyField(windowTitlesUpdateTiming);
        }
    }

}