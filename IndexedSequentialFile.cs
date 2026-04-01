namespace TP_N1_BaseDeDatos;

internal sealed class IndexedSequentialFile<T>(int N, int OVER, int OMAX, IndexArea indexArea, DataArea<T> dataArea)
{
  private readonly int _N = N;
  private readonly int _OVER = OVER;    
  private readonly int _PMAX = OMAX;
  private readonly int _OMAX = OMAX;

  private readonly IndexArea _indexArea = indexArea;
  private readonly DataArea<T> _dataArea = dataArea;

  public bool Insert(Record<T> record)
  {
    return false;
  }

  public Record<T>? Select(int id)
  {
    return null;
  }
}