using System;
using System.Collections.Generic;
using System.IO;
using Redpenguin.GoogleSheets.Scripts.Runtime.Core;
using UnityEditor;
using UnityEngine;

namespace Redpenguin.GoogleSheets.Scripts.Editor.Core
{
  public class SpreadSheetScriptableObjectFactory
  {
    private const string SavePath = "Assets/Plugins/Redpenguin/GoogleSheets/Resources/SheetsData";
    private const string ConfigSavePath = "Assets/Plugins/Redpenguin/GoogleSheets/Resources/ConfigurationDatabase";

    public SpreadSheetScriptableObjectFactory()
    {
      CreateDirectories();
    }

    private static void CreateDirectories()
    {
      if (!Directory.Exists(SavePath))
      {
        Directory.CreateDirectory(SavePath);
      }

      if (!Directory.Exists(ConfigSavePath))
      {
        Directory.CreateDirectory(ConfigSavePath);
      }
    }

    public void CreateScriptableObjects(List<Type> types)
    {
      if(types.Count == 0) return;
      //DeleteAllAssets();
      foreach (var type in types)
      {
        var asset = ScriptableObject.CreateInstance(type);
        AssetDatabase.CreateAsset(asset, $"{SavePath}/{type.Name}.asset");
      }

      AssetDatabase.SaveAssets();
      AssetDatabase.Refresh();
      Debug.Log("Scriptable Objects was created!");
    }

    public void CreatConfigDatabase()
    {
      var asset = ScriptableObject.CreateInstance<ConfigDatabase>();
      AssetDatabase.CreateAsset(asset, $"{ConfigSavePath}/ConfigDatabase.asset");
      AssetDatabase.SaveAssets();
      AssetDatabase.Refresh();
      //Debug.Log("<color=green>ConfigDatabase was created</color>");
    }
    
    public void DeleteAllAssets()
    {
      var di = new DirectoryInfo(SavePath);
      foreach (var file in di.EnumerateFiles())
      {
        //Debug.Log($"{file.Name} delete");
        file.Delete(); 
      }
      //AssetDatabase.Refresh();
    }
  }
}