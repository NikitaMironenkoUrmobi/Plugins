using UnityEngine;

namespace Redpenguin.GoogleSheets.Scripts.Runtime.Core
{
  public abstract class ConfigDatabaseProvider
  {
    private ConfigDatabase _configDatabase;
    public ConfigDatabase ConfigDatabase
    {
      get
      {
        if (_configDatabase != null) return _configDatabase;
        _configDatabase = Resources.Load<ConfigDatabase>(GoogleSheetsVariables.ConfigDatabasePath);
        if (_configDatabase == null)
        {
          throw new System.Exception("ConfigDatabase doesn't exist!");
        }
        return _configDatabase;
      }
    }
  }
}