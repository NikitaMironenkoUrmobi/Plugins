using UnityEngine;

namespace Redpenguin.GoogleSheets
{
  [CreateAssetMenu(fileName = "GoogleSheetsSettings", menuName = "Google Sheets/Settings", order = 1)]
  public class GoogleSheetsSettings : ScriptableObject
  {
    public string sheetID;
    public TextAsset clientSecrets;
  }
}