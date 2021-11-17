using System.Collections.Generic;

namespace Redpenguin.GoogleSheets
{
  public interface IContainerHelper
  {
    void RegisterContainers(List<IContainer> containers);
  }
}