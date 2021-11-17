using System.Collections.Generic;

namespace Redpenguin.GoogleSheets
{
  public abstract class ReaderParams
  {
    public string Range => $"{Sheet}!{FromCell}:{ToCell}";

    protected abstract string Sheet { get; }
    protected abstract string FromCell { get; }
    protected abstract string ToCell { get; }

    public abstract List<(Params type, int[] colIndexes)> ReadSetup { get; }
  }
}