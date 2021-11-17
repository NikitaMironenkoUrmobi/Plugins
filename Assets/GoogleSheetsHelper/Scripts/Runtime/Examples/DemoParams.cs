using System;
using System.Collections.Generic;
using UnityEngine;

namespace Redpenguin.GoogleSheets.Examples
{
  public class DemoParams : Params
  {
    public string id;
    public string id2;
    public string id3;
    public override void SetValues(object[] args)
    {
      if (args.Length < 3) throw new Exception($"Not enough items to create {this.GetType()}");
      var list = new List<string>();

      list.Add(id = Convert.ToString(args[0]));
      list.Add(id2 = Convert.ToString(args[1]));
      list.Add(id3 = Convert.ToString(args[2]));

      var st = "";
      foreach (var item in list)
      {
        st += $"{item} ";
      }
      Debug.Log(st);
    }
  }
}