using Redpenguin.GoogleSheets;
using System.Collections;
using System.Collections.Generic;

namespace Redpenguin.GoogleSheets.Examples
{
  public class DemoReader : ReaderParams
  {
    public override List<(Params type, int[] colIndexes)> ReadSetup =>
        new List<(Params type, int[] colIndexes)>
        {
                (new DemoParams(),new []{0,1,2})
        };

    protected override string Sheet => "Demo";

    protected override string FromCell => "A2";

    protected override string ToCell => "C3";
  }
}