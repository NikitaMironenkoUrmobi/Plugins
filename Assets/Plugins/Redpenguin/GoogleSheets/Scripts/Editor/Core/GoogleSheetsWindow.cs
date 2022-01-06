using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Redpenguin.GoogleSheets.Scripts.Editor.Utils;
using Redpenguin.GoogleSheets.Scripts.Runtime.Core;
using UnityEditor;
using UnityEngine;

namespace Redpenguin.GoogleSheets.Scripts.Editor.Core
{
  public class GoogleSheetsWindow : CustomEditorWindow
  {
    public string googleSheetId;
    public string credentialPath;
    public TextAsset credential;
    public List<ScriptableObject> _sheetsData = new List<ScriptableObject>();

    private bool isShow = false;
    private GoogleSheetsReader _googleSheetsReader;
    private SerializedObject _serializedObject;
    private List<string> _list = new List<string>();
    private bool _isScriptsCreated = false;
    private TextAsset _textAsset;
    private SpreadSheetCodeFactory _codeFactory;
    private SpreadSheetScriptableObjectFactory _scriptObjFactory;
    private bool _foldout;
    private DataImporter _dataImporter;

    [MenuItem("Urmobi/Google Sheets", false, 3)]
    private static void CreateWindows()
    {
      GetWindow<GoogleSheetsWindow>("Google Sheets Provider").Show();
    }

    protected override void OnEnable()
    {
      base.OnEnable();
      SetSheetsDatesToEditorWindow();
      CreateGoogleSheetParser();
      Initialization();
      LoadCredentialAsset();
      AssemblyReloadEvents.afterAssemblyReload += OnAfterAssemblyReload;
    }

    private void LoadCredentialAsset()
    {
      if (credential != null) return;
      var asset = AssetDatabase.LoadAssetAtPath<TextAsset>(credentialPath);
      if (asset != null)
      {
        credential = asset;
      }
    }

    private void OnValidate()
    {
      if (FieldIsEmpty()) return;
      credentialPath = AssetDatabase.GetAssetPath(credential);
      CreateGoogleSheetParser();
    }

    private void Initialization()
    {
      _codeFactory ??= new SpreadSheetCodeFactory(_textAsset);
      _scriptObjFactory ??= new SpreadSheetScriptableObjectFactory();
    }

    private void CreateGoogleSheetParser()
    {
      if (FieldIsEmpty()) return;
      _googleSheetsReader ??= new GoogleSheetsReader(googleSheetId, credential.text);
      _dataImporter ??= new DataImporter(_googleSheetsReader);
    }

    private bool FieldIsEmpty()
    {
      return credential == null || googleSheetId.Equals(string.Empty);
    }

    private bool SettingCheck()
    {
      if (FieldIsEmpty())
      {
        Debug.LogError("Google Sheet ID and Credential is empty!");
        return false;
      }

      return true;
    }

    protected override void OnDisable()
    {
      AssemblyReloadEvents.afterAssemblyReload -= OnAfterAssemblyReload;
      base.OnDisable();
    }

    private void SetSheetsDatesToEditorWindow()
    {
      _sheetsData.Clear();
      Resources.LoadAll<ScriptableObject>("SheetsData")
        .ToList().ForEach(x => _sheetsData.Add(x));
    }

    private void OnGUI()
    {
      _serializedObject = new SerializedObject(this);
      _foldout = EditorGUILayout.Foldout(_foldout, "Settings");
      if (_foldout)
      {
        googleSheetId = EditorGUILayout.TextField("Google Sheet ID", googleSheetId);
        ShowProperty("credential");
      }
      ShowList();
      SetButton("Create sheets SO", CreateAdditionalScripts);
      SetButton("Load data", LoadSheetsData);
      //SetButton("OnAfterAssemblyReloadTest", CreateConfigDatabase);
      //SetButton("Json", CreateJsonTest);

      _serializedObject.ApplyModifiedProperties();
    }

    private static void CreateJsonTest()
    {
      var result = JsonConvert.SerializeObject(new List<int> {1, 2, 3});
      Debug.Log(result);
    }

    private void SetButton(string label, Action action)
    {
      if (GUILayout.Button(label))
      {
        if (!SettingCheck()) return;
        action.Invoke();
      }
    }

    private void CreateConfigDatabase()
    {
      if (!_sheetsData.Any() || _sheetsData.Contains(null)) return;
      var configDb = Resources.Load<ConfigDatabase>(GoogleSheetsVariables.ConfigDatabasePath);
      if (configDb == null)
      {
        _scriptObjFactory.CreatConfigDatabase();
        configDb = Resources.Load<ConfigDatabase>(GoogleSheetsVariables.ConfigDatabasePath);
      }

      configDb.AddContainers(_sheetsData);
    }


    private void CreateAdditionalScripts()
    {
      _scriptObjFactory.DeleteAllAssets();
      _isScriptsCreated = _codeFactory.CreateAdditionalScripts();
    }

    private async void OnAfterAssemblyReload()
    {
      if (_isScriptsCreated == false) return;
      _isScriptsCreated = false;
      _scriptObjFactory.CreateScriptableObjects(_codeFactory.GetGeneratedScriptsTypes());
      await Task.Delay(TimeSpan.FromSeconds(0.1f));
      SetSheetsDatesToEditorWindow();
      CreateConfigDatabase();
    }

    private void LoadSheetsData()
    {
      _dataImporter.LoadSheetsData();
    }

    private void ShowList()
    {
      ShowProperty("_sheetsData");
    }

    private void ShowProperty(string property)
    {
      var stringsProperty = _serializedObject.FindProperty(property);
      EditorGUILayout.PropertyField(stringsProperty, true);
    }
  }
}