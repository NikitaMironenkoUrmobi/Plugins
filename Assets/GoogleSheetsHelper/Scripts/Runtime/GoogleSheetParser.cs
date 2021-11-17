namespace Redpenguin.GoogleSheets
{
  [System.Serializable]
  public abstract class GoogleSheetParser
  {
    public abstract void ClearData();
    public abstract void LoadDatabase();
  }
}