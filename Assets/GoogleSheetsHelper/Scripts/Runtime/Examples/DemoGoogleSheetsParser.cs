using System.Collections.Generic;
using UnityEngine;
using static Redpenguin.GoogleSheets.Examples.DemoGoogleSheetsParser;

namespace Redpenguin.GoogleSheets.Examples
{
  public abstract class DemoGoogleSheetsParser : GoogleSheetsProvider<DatabaseContainer>
  {
    public override string DatabaseSavingPath => Application.dataPath;
    protected override List<IContainer> CreateDatabase()
    {
      var list = new List<IContainer>();
      List<DemoParams> demoParams = new List<DemoParams>();
      demoParams.SetParams<DemoParams, DemoReader>();
      return list;
    }

    public class DatabaseContainer : IContainerHelper
    {
      public void RegisterContainers(List<IContainer> containers)
      {
        throw new System.NotImplementedException();
      }
    }

  }
}