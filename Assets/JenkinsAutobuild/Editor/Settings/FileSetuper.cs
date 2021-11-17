using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace JenkinsAutobuild.Editor.Settings
{
    public static class FileSetuper
    {
        private static string _buildPath;
        // private static readonly string JenkinsBuilds = $"{BuildPath}/JenkinsBuilds";
        // private static readonly string JenkinsLog = $"{BuildPath}/JenkinsLog";
        // private static readonly string Keystore = $"{BuildPath}/Keystore";
        // private static readonly string Pipeline = $"{BuildPath}/Pipeline";
        private static readonly List<string> ReplaceOrder = new List<string>
        {
            "%GIT_URL%",
            "%UNITY_VERSION%",
            "%PROJECT_NAME%",
            "%KEY_NAME%",
            "%ALIAS%",
            "%ALIAS_PASS%",
            "%KEY_PASS%"
        }; 
        private static readonly List<string> Folders = new List<string>
        {
            "JenkinsBuilds",
            "JenkinsLog",
            "Keystore",
            //"Pipeline"
        }; 
        private static readonly List<string> SemVerFiles = new List<string>
        {
            $"{Application.dataPath}/Packages/JenkinsAutobuild/Resources/Semver/semver.yaml",
            $"{Application.dataPath}/Packages/JenkinsAutobuild/Resources/Semver/semver-gen"
        };  
        
        [MenuItem("Jenkins/File Setup")]
        public static void FileSetup()
        {
            _buildPath = $"{Application.dataPath}".Replace("/Assets", "");
            foreach (var folderPath in Folders.Select(folder => $"{_buildPath}/{folder}"))
            {
                CreateIfNotExist(folderPath);
                SetVisibleToGit(folderPath);
            }

            var files = Resources.LoadAll<TextAsset>("Semver").ToList();
            
            Debug.Log(files.Count);
            foreach (var file in SemVerFiles)
            {
                if(File.Exists(file)) continue;
                var fileName = file.Split('/').Last();
                FileUtil.CopyFileOrDirectory(file, $"{_buildPath}/{fileName}");
            }
        }

        public static string SetBuildFile(List<string> list)
        {
            //var buildFile = Resources.LoadAll<TextAsset>("BuildPipeline").First();
            var buildFile = File.ReadAllText($"{Application.dataPath}/Packages/JenkinsAutobuild/Resources/BuildPipeline/build.groovy");
            var str = buildFile;
            for (int i = 0; i < ReplaceOrder.Count; i++)
            {
                str = str.Replace(ReplaceOrder[i], list[i]);
            }

            return str;
        }

        private static void CreateIfNotExist(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        private static void SetVisibleToGit(string path)
        {
            var fileName = $"{path}/.gitVisible";
            if(!File.Exists(fileName))
                File.Create(fileName);
        }
    }
}