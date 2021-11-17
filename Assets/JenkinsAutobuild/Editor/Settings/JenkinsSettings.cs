using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace JenkinsAutobuild.Editor.Settings
{
    public class JenkinsSettings : EditorWindow
    {
        [SerializeField] private string gitURL;
        [SerializeField] private string unityVersion = Application.unityVersion;
        [SerializeField] private string projectName;
        [SerializeField] private string key;
        [SerializeField] private string alias;
        [SerializeField] private string aliasPass;
        [SerializeField] private string keyPass;

        private bool IsAllFilds()
        {
            return GitCorrectCheck(gitURL)
                   || projectName == string.Empty
                   || key == string.Empty
                   || alias == string.Empty
                   || aliasPass == string.Empty
                   || keyPass == string.Empty
                   || unityVersion == string.Empty;
        }

        private static bool GitCorrectCheck(string url)
        {
            return !(url.Contains("https://github.com/") && url.Contains(".git"));
        }
        // Add menu named "My Window" to the Window menu
        [MenuItem("Jenkins/Settings")]
        private static void Init()
        {
            // Get existing open window or if none, make a new one:
            JenkinsSettings window = GetWindow<JenkinsSettings>("JenkinsSettings");
            window.maxSize = new Vector2(400f, 400f);
            window.minSize = window.maxSize;
            window.Show();
        }

        void OnGUI()
        {
            EditorGUILayout.LabelField("Settings");
            EditorGUILayout.Space(15);
            EditorGUILayout.BeginVertical();
            gitURL = EditorGUILayout.TextField("GIT URL", gitURL);
            if(GitCorrectCheck(gitURL)) EditorGUILayout.HelpBox("Incorrect  git", MessageType.Error);
            //unityVersion = EditorGUILayout.TextField("UNITY VERSION", unityVersion);
            projectName = EditorGUILayout.TextField("PROJECT NAME", projectName);
            key = EditorGUILayout.TextField("KEY NAME", key);
            alias = EditorGUILayout.TextField("ALIAS", alias);
            aliasPass = EditorGUILayout.TextField("ALIAS_PASS", aliasPass);
            keyPass = EditorGUILayout.TextField("KEY_PASS", keyPass);
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(15);
            
            if (GUILayout.Button("File Setup"))
            {
                FileSetuper.FileSetup();
            }
            EditorGUILayout.Space(15);
            EditorGUI.BeginDisabledGroup(IsAllFilds());
            if (GUILayout.Button("Copy groovy to buffer"))
            {
                GUIUtility.systemCopyBuffer = FileSetuper.SetBuildFile(new List<string> {gitURL, unityVersion, projectName, key, alias, aliasPass, keyPass});
                Debug.Log($"Groovy pipeline copied to buffer.");
            }

            EditorGUI.EndDisabledGroup();
        }

        private static void Buttons()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Button("1");
            GUILayout.Button("2", EditorStyles.miniButtonMid);
            GUILayout.Button("3", EditorStyles.miniButtonRight);
            GUILayout.Button("4", EditorStyles.miniButton);
            EditorGUILayout.EndHorizontal();
            GUILayout.Button("4");
        }

        private void Reset()
        {
            unityVersion = Application.unityVersion;
        }

        private void Save()
        {
            var data = JsonUtility.ToJson(this, false);
            EditorPrefs.SetString("JenkinsSettings", data);
        }
        private void Load()
        {
            var data = EditorPrefs.GetString("JenkinsSettings", JsonUtility.ToJson(this, false));
            JsonUtility.FromJsonOverwrite(data, this);
        }

        private void OnEnable()
        {
            Load();
        }
        private void OnDisable ()
        {
            Save();
        }
    }
}