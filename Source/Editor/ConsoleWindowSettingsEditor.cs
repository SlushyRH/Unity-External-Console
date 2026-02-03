using System;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace SRH.Editor
{
    public class ConsoleWindowSettingsEditor : EditorWindow
    {
        private const string FOLDER_PATH = "Assets/Resources/";
        private const string RESOURCES_PATH = "SRH/External Console Window/";
        
        [MenuItem("Tools/SRH/External Console Settings")]
        private static void ShowWindow()
        {
            GetWindow<ConsoleWindowSettingsEditor>("Console Window Settings");
        }

        private ConsoleWindowSettings settings;
        private UnityEditor.Editor settingsEditor;

        private void OnEnable()
        {
            settings = Resources.Load<ConsoleWindowSettings>(RESOURCES_PATH + "Console Window Settings");
            
            if (settings == null)
                CreateSettingsAsset();

            if (settings != null)
                settingsEditor = UnityEditor.Editor.CreateEditor(settings);
        }

        private void OnGUI()
        {
            if (!settings)
            {
                EditorGUILayout.LabelField("Could not find any ConsoleWindowSettings.");
                return;
            }
            
            if (settingsEditor)
                settingsEditor.OnInspectorGUI();
        }

        private void CreateSettingsAsset()
        {
            // create base resources folder
            if (!Directory.Exists(FOLDER_PATH + RESOURCES_PATH))
                Directory.CreateDirectory(FOLDER_PATH + RESOURCES_PATH);

            ConsoleWindowSettings newSettings = ScriptableObject.CreateInstance<ConsoleWindowSettings>();
            AssetDatabase.CreateAsset(newSettings, FOLDER_PATH + RESOURCES_PATH + "Console Window Settings.asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            settings = newSettings;
        }
    }
    
    [CustomEditor(typeof(ConsoleWindowSettings), true)]
    public class ConsoleWindowSettingsInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawPropertiesExcluding(serializedObject, "m_Script");
            serializedObject.ApplyModifiedProperties();
        }
    }
}
