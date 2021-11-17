using Redpenguin.GoogleSheets.Tools;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Redpenguin.GoogleSheets
{
  public abstract class GoogleSheetsProvider<T> : GoogleSheetParser where T : IContainerHelper, new()
  {
    private string filename = "Database";
    private string extension = ".bytes";
    public string FileName { get => filename; set => filename = value; }
    public virtual string DatabaseSavingPath => GoogleSheetsVariables.SavePaths.TAMPLATE_SAVEPATH;

    public override void LoadDatabase()
    {
      var container = RegisterContainer(CreateDatabase());
      SaveDatabase(container);
      Debug.Log("<color=green>Google sheets data successfully loaded!</color>");
    }

    protected abstract List<IContainer> CreateDatabase();
    protected T RegisterContainer(List<IContainer> containers)
    {
      T globalDatabaseContainer = new T();
      globalDatabaseContainer.RegisterContainers(containers);
      return globalDatabaseContainer;
    }
    private void SaveDatabase(T container)
    {
#if UNITY_EDITOR
      if (File.Exists($"{DatabaseSavingPath}/{filename}{extension}"))
      {
        File.Delete($"{DatabaseSavingPath}/{filename}{extension}");
      }
      if (Directory.Exists(DatabaseSavingPath) == false)
      {
        Directory.CreateDirectory(DatabaseSavingPath);
      }
      AssetDatabase.Refresh();
      FileSerializer.WriteToBinaryFile($"{DatabaseSavingPath}/{filename}{extension}", container);
      AssetDatabase.Refresh();
#endif
    }

    //[Button()]

    public override void ClearData()
    {
#if UNITY_EDITOR
      PlayerPrefs.DeleteAll();
      Caching.ClearCache();
      if (File.Exists($"{GoogleSheetsVariables.SavePaths.DATABASE_SAVEPATH}/{filename}{extension}"))
      {
        File.Delete($"{GoogleSheetsVariables.SavePaths.DATABASE_SAVEPATH}/{filename}{extension}");
      }
      AssetDatabase.Refresh();
      Debug.Log("Clear all saves!");
#endif
    }
  }
}