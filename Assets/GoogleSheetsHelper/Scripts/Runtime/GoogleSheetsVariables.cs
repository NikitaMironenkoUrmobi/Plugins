using UnityEngine;

namespace Redpenguin.GoogleSheets
{
  public class GoogleSheetsVariables
  {
    public static class SavePaths
    {
      public static readonly string DATABASE_SAVEPATH = $"{Application.persistentDataPath}/.database";
      public static readonly string TAMPLATE_SAVEPATH = $"{Application.dataPath}/Resources";
    }
  }
}