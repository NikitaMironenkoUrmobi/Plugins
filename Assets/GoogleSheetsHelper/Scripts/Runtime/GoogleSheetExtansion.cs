using System.Collections.Generic;

namespace Redpenguin.GoogleSheets
{
  public static class GoogleSheetExtansion
  {
    //public static void SetParams<T,R>(this List<T> list) where T : Params, new() where R : ReaderParams, new()
    //{
    //    list ??= new List<T>();
    //    var values = GoogleSheetsReader.GetRows<R>();
    //    foreach (var value in values)
    //    {
    //        list.Add(value.GetBalanceParams<T, R>());
    //    }
    //}
    public static List<T> SetParams<T, R>(this List<T> list, R readerParams = null) where T : Params, new() where R : ReaderParams, new()
    {
      list ??= new List<T>();
      var values = GoogleSheetsReader.GetRows(readerParams);
      foreach (var value in values)
      {
        list.Add(value.GetBalanceParams<T, R>(readerParams));
      }
      return list;
    }
  }
}