using Redpenguin.GoogleSheets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoogleSheetsParser : GoogleSheetsProvider<ContainerHelper>
{
  protected override List<IContainer> CreateDatabase()
  {
    Debug.Log("CreateDatabase");
    return new List<IContainer>() { };
  }
}

[System.Serializable]
public class ContainerHelper : IContainerHelper
{
  public void RegisterContainers(List<IContainer> containers)
  {
    Debug.Log("RegisterContainers");
  }
}

