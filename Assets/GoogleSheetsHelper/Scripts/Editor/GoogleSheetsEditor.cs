using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Redpenguin.GoogleSheets.Editor
{
  public class GoogleSheetsEditor
  {
    private static GoogleSheetParser googleSheetsParser;

    [MenuItem("Google Sheets Parser/Load Database", priority = 1)]
    private static void LoadDatabase()
    {
      Init();
      if (IsNeedToCreateSettings() == true) return;
      googleSheetsParser.ClearData();
      googleSheetsParser.LoadDatabase();
    }

    [MenuItem("Google Sheets Parser/Clear Data", priority = 2)]
    private static void Clear()
    {
      Init();
      if (IsNeedToCreateSettings() == true) return;
      googleSheetsParser.ClearData();
    }
    [MenuItem("Google Sheets Parser/Settings", priority = 3)]
    private static void Settings()
    {
      EditorUtility.FocusProjectWindow();
      UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>("Assets/GoogleSheetsHelper/Resources/GoogleSheetsSettings.asset");
      Selection.activeObject = obj;
    }

    private static void Init()
    {
      if (googleSheetsParser != null) return;
      var temp = Assembly.GetAssembly(typeof(GoogleSheetParser)).GetTypes().Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(GoogleSheetParser)));
      if(temp.Any())
      {
        googleSheetsParser = Activator.CreateInstance(temp.First()) as GoogleSheetParser;
      }
    }

    private static bool IsNeedToCreateSettings()
    {
      if(googleSheetsParser == null)
      {
        Debug.LogError("Create GoogleSheetParser");
      }
      return googleSheetsParser == null;
    }
  }
}