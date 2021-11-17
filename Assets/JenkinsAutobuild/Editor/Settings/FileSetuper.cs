using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace JenkinsAutobuild.Editor.Settings
{
    public static class FileSetuper
    {
        private static string _buildPath;
        private static string _resourcesPath;
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
            $"{Application.dataPath}/JenkinsAutobuild/Resources/semver.yaml",
            $"{Application.dataPath}/JenkinsAutobuild/Resources/semver-gen"
            //"semver.yaml",
            //"semver-gen"
        };  
        
        public static void FileSetup()
        {
            _buildPath = $"{Application.dataPath}".Replace("/Assets", "");
            foreach (var folderPath in Folders.Select(folder => $"{_buildPath}/{folder}"))
            {
                CreateIfNotExist(folderPath);
                SetVisibleToGit(folderPath);
            }

            // FindPath();
            // foreach (var fileName in SemVerFiles)
            // {
            //     var newFilePath = $"{_resourcesPath}/{fileName}";
            //     if(File.Exists(newFilePath)) continue;
            //     FileUtil.CopyFileOrDirectory(newFilePath, $"{_buildPath}/{fileName}");
            // }
            foreach (var filePath in SemVerFiles)
            {
                var fileName = filePath.Split('/').Last();
                var newPath = $"{_buildPath}/{fileName}";
                if(File.Exists(newPath)) continue;
                FileUtil.CopyFileOrDirectory(filePath, newPath);
            }
            Debug.Log($"Files were created!");
        }

        private static void FindPath()
        {
            //if(_resourcesPath != string.Empty) return;
            //_resourcesPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            //_resourcesPath = Directory.GetCurrentDirectory();
            var semverYaml = Resources.Load("semver.yaml");
            _resourcesPath =  AssetDatabase.GetAssetPath(semverYaml).Replace("/semver.yaml","");
            Debug.Log(_resourcesPath);
        }

        public static string SetBuildFile(List<string> list)
        {
            //FindPath();
            var buildFile = File.ReadAllText($"{Application.dataPath}/JenkinsAutobuild/Resources/build.groovy");
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