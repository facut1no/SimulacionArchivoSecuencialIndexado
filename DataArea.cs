using TP_N1_BaseDeDatos;

internal enum StateDataArea
{
  OverflowFull,
  MainDataAreaFull,
  FirstRecordBlockChanged,
}

internal sealed class DataArea<T>(int N, int OMAX, int PMAX)
{
  // TODO: Ver si se puede sacar responsabilidad.
  //       Quizas crear una clase para manejar el overflow
  //       y otra para el area de datos principal. Dificil :/
  private readonly int _N = N;
  private readonly int _OMAX = OMAX;
  private readonly int _PMAX = PMAX;
  private readonly int _OVER = PMAX + 1;
  private int _CantBlocks = 0;
  private int _lastOverflowBlockDirection = PMAX + 1;

  private readonly Record<T>[] _dataBlocks = [];

  public Record<T>? Select(int id, int blockDirection)
  {
    var record = SearchRecordInBlock(id, blockDirection);

    if (record is not null)
      return record;

    var dirRecordOverflow = GetDirRecordOverflowBlock(blockDirection);

    if (dirRecordOverflow is null)
      return null;

    var overflowRecord = SearchRecordInOverflowArea(dirRecordOverflow.Value, id);

    return overflowRecord; // Puede ser null si no se encuentra en overflow
  }

  private int? GetDirRecordOverflowBlock(int blockDirection)
  {
    throw new NotImplementedException();
  }

  private Record<T>? SearchRecordInOverflowArea(int firstDirection, int id)
  {
    var currentDirection = firstDirection;
    while (currentDirection != -1)
    {
      var record = _dataBlocks[currentDirection];
      if (record.Id == id)
        return record;
      currentDirection = record.Direction ?? -1;
    }
    return null;
  }

  private Record<T>? SearchRecordInBlock(int id, int blockDirection)
  {
    for (int i = blockDirection; i < blockDirection + _N; i++)
    {
      if (_dataBlocks[i].Id == id)
        return _dataBlocks[i];
    }
    return null;
  }

  public void Insert(Record<T> record, int blockDirection)
  {
    if (_CantBlocks == 0)
    {
      InsertCreatingBlock(record);
      return;
    }

    int percentageFreeSpaceBlock = PercentageFreeSpace(blockDirection);

    if (percentageFreeSpaceBlock < 50)
      NormalInsert(record, blockDirection);
    else if (percentageFreeSpaceBlock >= 50 && percentageFreeSpaceBlock < 100)
      InsertBlockMediumFull(record, blockDirection);
    else
      InsertBlockFull(record, blockDirection);
  }

  public Record<T>? GetIndexFirstRecordBlock(int blockDirection)
  {
    return _dataBlocks[blockDirection];
  }

  private void InsertBlockFull(Record<T> record, int blockDirection)
  {

  }

  private StateDataArea NormalInsert(Record<T> record, int blockDirection)
  {
    throw new NotImplementedException();
  }

  private int CreateBlock()
  {
    _CantBlocks++;
    var newBlockDirection = _CantBlocks * _N;
    return newBlockDirection;
  }

  private void InsertBlockMediumFull(Record<T> record, int blockDirection)
  {
    throw new NotImplementedException();
  }

  private void InsertCreatingBlock(Record<T> record)
  {
    var blockDirection = CreateBlock();
    _dataBlocks[blockDirection] = record;
  }

  private void InsertInOverflowArea(Record<T> record)
  {
    _dataBlocks[_lastOverflowBlockDirection] = record;
    _lastOverflowBlockDirection++;
  }

  private void SortBlock(int blockDirection)
  {
    Array.Sort(_dataBlocks, blockDirection, _N, Comparer<Record<T>>.Create((x, y) =>
    {
      if (x.Id == 0 && y.Id != 0) return 1;
      if (x.Id != 0 && y.Id == 0) return -1;
      return x.Id.CompareTo(y.Id);
    }));
  }

  private int PercentageFreeSpace(int blockDirection)
  {
    int emptyRegisters = 0;
    for (int i = blockDirection; i < blockDirection + _N; i++)
    {
      if (_dataBlocks[i].Id == 0)
        emptyRegisters++;
    }
    return (int)(emptyRegisters / (double)_N * 100);
  }
}