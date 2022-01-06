using UnityEditor;
using UnityEngine;

namespace Redpenguin.GoogleSheets.Scripts.Editor.Utils
{
  public abstract class CustomEditorWindow : EditorWindow
  {
    private void Save()
    {
      var data = JsonUtility.ToJson(this, false);
      EditorPrefs.SetString(this.GetType().ToString(), data);
    }

    private void Load()
    {
      var data = EditorPrefs.GetString(this.GetType().ToString(), JsonUtility.ToJson(this, false));
      JsonUtility.FromJsonOverwrite(data, this);
    }

    protected virtual void OnEnable()
    {
      Load();
    }

    protected virtual void OnDisable()
    {
      Save();
    }
  }
}