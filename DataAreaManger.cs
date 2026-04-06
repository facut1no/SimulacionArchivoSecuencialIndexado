using System.Text;
using TP_N1_BaseDeDatos;

internal enum DataInsertState
{
  InsertedInBlock,
  BlockFull,
  FirstRecordChanged,
  DataAreaFull,
  NewBlockCreated,
}

internal record DataInsertResult(
    DataInsertState State,
    int BlockDirection
);

internal sealed class DataAreaManager<T>(Record<T>[] Data, int N, int PMAX)
{
  private readonly Record<T>[] _data = Data;
  private readonly int _N = N;
  private readonly int _PMAX = PMAX;
  private int CantBlocks { get; set; } = 0;


  internal Record<T>? Select(int id, int blockDirection)
  {
    for (int i = blockDirection; i < blockDirection + _N && _data[i] is not null; i++)
    {
      if (_data[i].Id == id)
        return _data[i];
    }
    return null;
  }

  internal DataInsertResult Insert(Record<T> record, int? blockDirection)
  {
    if (blockDirection is null)
      return InsertCreatingBlock(record);

    var numberOfRecords = GetNumberOfRecordsBlock(blockDirection.Value);
    var isLast = IsLastRecordBlock(record.Id, blockDirection.Value);

    if (numberOfRecords == _N && !isLast)
      return new DataInsertResult(DataInsertState.BlockFull, blockDirection.Value);

    if (isLast)
    {
      if (numberOfRecords >= Math.Ceiling((decimal)(_N / 2d)))
        return InsertCreatingBlock(record);
      else
        return InsertNormal(record, blockDirection.Value, isLast);
    }
    return InsertNormal(record, blockDirection.Value, false);
  }

  internal void SetDirectionOverflowBlock(int blockDirection, int overflowDirection)
  {
    var lastRecord = GetLastRecordBlock(blockDirection);
    lastRecord?.Direction = overflowDirection;
  }

  internal Record<T>? GetLastRecordBlock(int blockDirection)
  {
    Record<T>? last = null;
    for (int i = blockDirection; i < blockDirection + _N && _data[i] is not null; i++)
    {
      last = _data[i];
    }
    return last;
  }

  private DataInsertResult InsertNormal(Record<T> record, int blockDirection, bool isLast)
  {
    for (int i = blockDirection; i < blockDirection + _N; i++)
    {
      if (_data[i] is null)
      {
        _data[i] = record;
        break;
      }
    }
    if (!isLast)
    {
      var firstId = GetIdFirstRecord(blockDirection);
      SortBlock(blockDirection);
      if (firstId!.Value != GetIdFirstRecord(blockDirection))
        return new DataInsertResult(DataInsertState.FirstRecordChanged, blockDirection);
    }
    return new DataInsertResult(DataInsertState.InsertedInBlock, blockDirection);
  }

  internal int? GetIdFirstRecord(int blockDirection)
  {
    return _data[blockDirection].Id;
  }

  private void SortBlock(int blockDirection)
  {
    Array.Sort(_data, blockDirection, _N, Comparer<Record<T>>.Create((x, y) =>
    {
      if (x is null && y is null) return 0;
      if (x is null) return 1;
      if (y is null) return -1;
      return x.Id.CompareTo(y.Id);
    }));
  }

  private bool IsLastRecordBlock(int id, int blockDirection)
  {
    for (int i = blockDirection; i < blockDirection + _N && _data[i] is not null; i++)
    {
      if (id < _data[i].Id)
        return false;
    }
    return true;
  }

  private DataInsertResult InsertCreatingBlock(Record<T> record)
  {
    var blockDirection = CantBlocks * _N;
    if (blockDirection >= _PMAX)
      return new DataInsertResult(DataInsertState.DataAreaFull, -1);

    _data[blockDirection] = record;
    CantBlocks++;

    return new DataInsertResult(DataInsertState.NewBlockCreated, blockDirection);
  }

  private int GetNumberOfRecordsBlock(int blockDirection)
  {
    var number = 0;
    for (int i = blockDirection; i < blockDirection + _N && _data[i] is not null; i++)
    {
      number++;
    }
    return number;
  }

  internal int? GerDirOverflowStart(int blockDirection)
  {
    for (int i = blockDirection; i < blockDirection + _N && _data[i] is not null; i++)
    {
      if (_data[i].Direction is not null)
        return _data[i].Direction;
    }
    return null;
  }

  public override string ToString()
  {
    var sb = new StringBuilder();

    sb.AppendLine("===== AREA PRINCIPAL =====");

    for (int b = 0; b < CantBlocks; b++)
    {
      int start = b * _N;
      sb.Append($"Bloque {b} ({start}): ");

      for (int i = start; i < start + _N; i++)
      {
        var r = _data[i];

        if (r is null)
        {
          sb.Append("[  ] ");
        }
        else
        {
          if (r.Direction is not null)
            sb.Append($"[ {r.Id} -→ {r.Direction} ] ");
          else
            sb.Append($"[ {r.Id} ] ");
        }
      }

      sb.AppendLine();
    }

    return sb.ToString();
  }

}