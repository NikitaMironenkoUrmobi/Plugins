using System.Collections.Generic;
using UnityEngine;

namespace Redpenguin.GoogleSheets.Scripts.Runtime.Core
{
  public abstract class SpreadSheetScriptableObject<T> : ScriptableObject, ISpreadSheets where T : ISheetData, new()
  {
    public List<T> Data = new List<T>();
    public void SetListCount(int count)
    {
      var result = count - Data.Count;
      for (var i = 0; i < result; i++)
      {
        Data.Add(new T());
      }
    }
  }
}