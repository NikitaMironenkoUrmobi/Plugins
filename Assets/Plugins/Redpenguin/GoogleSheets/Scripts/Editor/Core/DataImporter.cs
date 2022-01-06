using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Redpenguin.GoogleSheets.Scripts.Runtime.Attributes;
using Redpenguin.GoogleSheets.Scripts.Runtime.Core;
using Redpenguin.GoogleSheets.Scripts.Runtime.Utils;
using UnityEngine;

namespace Redpenguin.GoogleSheets.Scripts.Editor.Core
{
  public class DataImporter
  {
    private const string SheetsData = "SheetsData";
    private readonly GoogleSheetsReader _sheetsReader;

    public DataImporter(GoogleSheetsReader sheetsReader)
    {
      _sheetsReader = sheetsReader;
    }

    public void LoadSheetsData()
    {
      var databaseScriptObj = Resources.LoadAll<ScriptableObject>(SheetsData);
      Debug.Log($"Count {databaseScriptObj.Length}");
      foreach (var database in databaseScriptObj)
      {
        var databaseType = database.GetType();
        var sheetValues = GetSheetValues(databaseType);
        var dataList = databaseType.GetFields().First();
        ((ISpreadSheets) database).SetListCount(sheetValues.First().Value.Count);
        SetValues(dataList, database, sheetValues);
      }
    }

    private void SetValues(FieldInfo list, ScriptableObject database,
      IReadOnlyDictionary<string, List<object>> sheetValues)
    {
      if (!(list.GetValue(database) is IList dataList)) return;
      for (var i = 0; i < dataList.Count; i++)
      {
        var dataClass = dataList[i];
        var dataClassFields = dataClass.GetType().GetFields();
        foreach (var field in dataClassFields)
        {
          var fieldName = field.Name;
          if (!sheetValues.ContainsKey(fieldName)) continue;
          if (sheetValues[fieldName].Count <= i) continue;

          var fieldData = sheetValues[fieldName][i];
          var newClass = IsJson(field.FieldType, fieldData);
          field.SetValue(dataClass, newClass ?? Convert.ChangeType(sheetValues[fieldName][i], field.FieldType));
        }
      }
    }

    private Dictionary<string, List<object>> GetSheetValues(Type databaseType)
    {
      var spreadSheetRange = databaseType.GetAttributeValue((SheetRange st) => st.SpreadSheetRange);
      return _sheetsReader.GetValuesOnRange(spreadSheetRange);
    }

    private object IsJson(Type type, object value)
    {
      try
      {
        var result = JsonConvert.DeserializeObject(value.ToString(), type);
        return result;
      }
      catch
      {
        return null;
      }
    }
  }
}