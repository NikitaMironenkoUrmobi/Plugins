using System.Collections.Generic;

namespace Redpenguin.GoogleSheets
{
  public static class Parser
  {
    public static T GetBalanceParams<T, R>(this IList<object> values, R readerParams = null) where T : Params, new() where R : ReaderParams, new()
    {
      var temp = new T();
      readerParams ??= new R();
      temp.SetValues(GetValues<T>(values, readerParams));
      return temp;
    }

    private static object[] GetValues<T>(IList<object> values, ReaderParams reader)
    {
      var indexes = GetIndexes<T>(reader.ReadSetup);
      List<object> args = new List<object>();
      for (int i = 0; i < indexes.Length; i++)
      {
        args.Add(values[indexes[i]]);
      }
      return args.ToArray();
    }

    private static int[] GetIndexes<T>(List<(Params type, int[] colIndexes)> data)
    {
      var results = data.FindAll(x => x.type is T);
      if (results.Count <= 0) return null;
      return results[0].colIndexes;
    }
  }
}