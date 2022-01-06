using System;

namespace Redpenguin.GoogleSheets.Scripts.Runtime.Attributes
{
  [AttributeUsage(AttributeTargets.Class)]
  public class SpreadSheet : Attribute
  {
    public string Name;
    public string Range;

    public SpreadSheet(string name, string to = "Z1000")
    {
      Name = name;
      Range = $"{Name}!A1:{to}";
    }
  }
}