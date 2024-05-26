using UnityEditor;
using UnityEngine;

namespace TileGame
{
    [CustomEditor(typeof(SaveManager))]
    public class SaveManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            SaveManager saveManager = (SaveManager)target;

            if (GUILayout.Button("Save Game"))
            {
                saveManager.SaveGameEditor();
            }

            if (GUILayout.Button("Load Game"))
            {
                saveManager.LoadGameEditor();
            }

            if (GUILayout.Button("Reset Game"))
            {
                saveManager.ResetGameEditor();
            }
        }
    }
}