using System;
using System.Collections.Generic;
using Redpenguin.GoogleSheets.Scripts.Runtime.Attributes;
using Redpenguin.GoogleSheets.Scripts.Runtime.Core;

namespace Redpenguin.GoogleSheets.Scripts.Runtime.Examples
{
  //[SpreadSheet("Example")] //Get values from Example sheet
  [Serializable]
  public class ExampleData : ISheetData
  {
    public string myString;         // string
    public int myInt;               // 1
    public bool myBool;             // true
    public List<int> myInts;        // [1,2,3]
    public List<string> myStrings;  // ["a","b","c"]
    public JsonExample jsonExample; // {"id":1,"myString":"string"}
  }

  [Serializable]
  public class JsonExample
  {
    public int id;
    public string myString;
  }
}