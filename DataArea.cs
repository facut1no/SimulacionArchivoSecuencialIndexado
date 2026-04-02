using System.Runtime.CompilerServices;
using System.Text;
using TP_N1_BaseDeDatos;

internal enum StateDataArea
{
  OverflowFull,
  MainDataAreaFull,
  NormalInsert,
  OverflowInsert,
  InsertedNewBlock
}

internal record ResponseInsert(StateDataArea State, int DirectionBlock, bool FirstRecordChange = false);

internal sealed class DataArea<T>(int N, int OMAX, int PMAX)
{
  private readonly int _N = N;
  private readonly int _OMAX = OMAX;
  private readonly int _PMAX = PMAX;
  private readonly int _OVER = PMAX + 1;
  private int _CantBlocks = 0;
  private int _lastOverflowBlockDirection = PMAX + 1;

  private readonly Record<T>[] _dataArea = new Record<T>[OMAX + PMAX];

  private bool IsValidBlockDirection(int blockDirection)
  {
    return blockDirection >= 0 &&
           blockDirection % _N == 0 &&
           blockDirection < _CantBlocks * _N;
  }

  public Record<T>? Select(int id, int blockDirection)
  {
    if (!IsValidBlockDirection(blockDirection))
      throw new InvalidOperationException("La dirreccion de bloque es invalida");

    var record = SearchRecordInBlock(id, blockDirection);

    if (record is not null)
      return record;

    record = SearchRecordInOverflowArea(id, blockDirection);

    return record; // Va a se null si no esta en el overflow
  }

  private Record<T>? SearchRecordInOverflowArea(int id, int blockDirection)
  {
    var dirOverflowBlockArea = GetDirOverflowBlockArea(blockDirection);
    if (dirOverflowBlockArea is null)
      return null;

    var currentDirection = dirOverflowBlockArea;
    while (currentDirection is not null)
    {
      var record = _dataArea[currentDirection.Value];
      if (record.Id == id)
        return record;
      currentDirection = record.Direction;
    }
    return null;
  }

  private int GetLastRecordIndex(int blockDirection)
  {
    var lastValid = blockDirection;
    for (int i = blockDirection; i < blockDirection + _N && _dataArea[i] is not null; i++)
    {
      if (_dataArea[i].Id == 0)
        break;
      lastValid = i;
    }
    return lastValid;
  }

  private int? GetDirOverflowBlockArea(int blockDirection) =>
    _dataArea[GetLastRecordIndex(blockDirection)].Direction;

  private Record<T>? SearchRecordInBlock(int id, int blockDirection)
  {
    for (int i = blockDirection; i < blockDirection + _N && _dataArea[i] is not null; i++)
    {
      if (_dataArea[i].Id == id)
        return _dataArea[i];
    }
    return null;
  }

  public ResponseInsert Insert(Record<T> record, int? blockDirection = null)
  {
    if (_CantBlocks == 0 || blockDirection is null)
      return InsertCreatingNewBlock(record);

    if (!IsValidBlockDirection(blockDirection.Value))
      throw new InvalidOperationException("La dirreccion de bloque es invalida");

    var freeSpace = GetFreeSpaceRateBlock(blockDirection.Value);

    if (freeSpace > 50)
      return InsertNormal(record, blockDirection.Value);
    else if (freeSpace == 0)
      return InsertInFullBlock(record, blockDirection.Value);

    var isLastRecord = IsLastRecordBlock(record.Id, blockDirection.Value);

    if (isLastRecord && freeSpace <= 50)
      return InsertCreatingNewBlock(record);

    return InsertNormal(record, blockDirection.Value);
  }

  private ResponseInsert InsertInFullBlock(Record<T> record, int blockDirection)
  {
    var lastRecordIndex = blockDirection + _N - 1;
    var existingDirection = _dataArea[lastRecordIndex].Direction;

    var response = InsertInOverflowArea(record, existingDirection);

    if (existingDirection is null && response.State == StateDataArea.OverflowInsert)
      _dataArea[lastRecordIndex].Direction = response.DirectionBlock;

    return response;
  }

  private ResponseInsert InsertInOverflowArea(Record<T> record, int? direction)
  {
    if (IsOverflowFull())
    {
      return new ResponseInsert(StateDataArea.OverflowFull, -1);
    }
    if (direction is null)
    {
      _dataArea[_lastOverflowBlockDirection] = record;
      return new ResponseInsert(StateDataArea.OverflowInsert, _lastOverflowBlockDirection++);
    }

    // Recorro la cadena de overflow
    var currentDirection = direction;
    while (currentDirection is not null)
    {
      if (_dataArea[currentDirection.Value].Direction is null)
      {
        _dataArea[currentDirection.Value].Direction = _lastOverflowBlockDirection;
        break;
      }
      currentDirection = _dataArea[currentDirection.Value].Direction;
    }
    _dataArea[_lastOverflowBlockDirection] = record;
    return new ResponseInsert(StateDataArea.OverflowInsert, _lastOverflowBlockDirection++);
  }

  private bool IsOverflowFull() => _lastOverflowBlockDirection > _OMAX;

  private ResponseInsert InsertCreatingNewBlock(Record<T> record)
  {
    if (_CantBlocks * _N > _PMAX) // TOOD: separar en otro metodo la verificacion de espaci
      return new ResponseInsert(StateDataArea.MainDataAreaFull, -1);

    var directionBlock = _CantBlocks * _N;
    _dataArea[directionBlock] = record;
    _CantBlocks++;
    return new ResponseInsert(StateDataArea.InsertedNewBlock, directionBlock);
  }

  private ResponseInsert InsertNormal(Record<T> record, int blockDirection)
  {
    for (int i = blockDirection; i < blockDirection + _N; i++)
      if (_dataArea[i] is null)
      {
        _dataArea[i] = record;
        break;
      }
    // ordeno y verifico si cambio el primer registro para actualizar el area de indice
    var firtsRecordId = _dataArea[blockDirection];
    SortBlock(blockDirection);

    return firtsRecordId != _dataArea[blockDirection]
           ? new ResponseInsert(StateDataArea.NormalInsert, blockDirection, true) // Cambio 
           : new ResponseInsert(StateDataArea.NormalInsert, blockDirection); // NO Cambio
  }


  private void SortBlock(int blockDirection)
  {
    Array.Sort(_dataArea, blockDirection, _N, Comparer<Record<T>>.Create((x, y) =>
    {
      if (x is null && y is null) return 0;
      if (x is null) return 1;
      if (y is null) return -1;
      return x.Id.CompareTo(y.Id);
    }));
  }

  private bool IsLastRecordBlock(int id, int blockDirection)
  {
    for (int i = blockDirection; i < blockDirection + _N && _dataArea[i] is not null; i++)
      if (_dataArea[i].Id > id)
        return false;

    return true;
  }

  private int GetFreeSpaceRateBlock(int blockDirection)
  {
    // Devuelve el espacio libre en porcetaje 
    // osea 100 es un bloque sin espacio ocupados.
    var free = 0;
    for (int i = blockDirection; i < blockDirection + _N; i++)
      if (_dataArea[i] is null)
        free++;

    return (int)(free / (double)_N * 100);
  }

  public override string ToString()
  {
    var sb = new StringBuilder();

    sb.AppendLine("===== AREA PRINCIPAL =====");

    for (int b = 0; b < _CantBlocks; b++)
    {
      int start = b * _N;
      sb.Append($"Bloque {b} ({start}): ");

      for (int i = start; i < start + _N; i++)
      {
        var r = _dataArea[i];

        if (r is null || r.Id == 0)
        {
          sb.Append("[ ] ");
        }
        else
        {
          sb.Append(r.Direction is not null
              ? $"[{r.Id}|→{r.Direction}] "
              : $"[{r.Id}] ");
        }
      }

      sb.AppendLine();
    }

    sb.AppendLine("\n===== OVERFLOW =====");

    for (int i = _OVER; i < _lastOverflowBlockDirection; i++)
    {
      var r = _dataArea[i];

      if (r is null || r.Id == 0)
        continue;

      sb.AppendLine(r.Direction is not null
          ? $"{i}: [{r.Id} → {r.Direction}]"
          : $"{i}: [{r.Id}]");
    }

    return sb.ToString();
  }
}
