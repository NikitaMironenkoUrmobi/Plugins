namespace Redpenguin.GoogleSheets
{
  [System.Serializable]
  public abstract class Params : SerializableGroup
  {
    public abstract void SetValues(object[] args);
  }
}