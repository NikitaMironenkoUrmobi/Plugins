using System;

namespace Redpenguin.GoogleSheets.Scripts.Runtime.Attributes
{
  [AttributeUsage(AttributeTargets.Class)]
  public class SheetRange : Attribute
  {
    public string SpreadSheetRange;

    public SheetRange(string spreadSheetRange)
    {
      SpreadSheetRange = spreadSheetRange;
    }
  }
}