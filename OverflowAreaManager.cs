using System.ComponentModel.DataAnnotations;
using System.Text;
using TP_N1_BaseDeDatos;

internal enum OverflowInsertResult
{
  OverflowFull,
  InsertOverflow,
  DirectionOverflowInvalid
}

internal sealed class OverflowAreaManager<T>(Record<T>[] Data, int OVER, int OMAX)
{
  private readonly Record<T>[] _Data = Data;
  private readonly int _OVER = OVER;
  private readonly int _OMAX = OMAX;
  private int _lastPosition = OVER;

  public Record<T>? Select(int id, int overflowStart)
  {
    var current = _Data[overflowStart];
    while (current is not null)
    {
      if (current.Id == id)
        return current;

      if (current.Direction is null)
        return null;

      current = _Data[current.Direction.Value];
    }
    return null;
  }

  private Record<T>? GetLastRecordChain(int overflowStart)
  {
    var current = _Data[overflowStart];
    while (current is not null)
    {
      if (current.Direction is null)
        return current;

      current = _Data[current.Direction.Value];
    }
    return null;
  }

  public OverflowInsertResult Insert(Record<T> record, Record<T> lastRecordBlock)
  {
    if (_lastPosition >= _OMAX)
      return OverflowInsertResult.OverflowFull;

    if (lastRecordBlock.Direction is null)
    {
      lastRecordBlock.Direction = _lastPosition;
      _Data[_lastPosition] = record;
      _lastPosition++;
      return OverflowInsertResult.InsertOverflow; ;
    }
    var lastRecordChain = GetLastRecordChain(lastRecordBlock.Direction.Value);
    if (lastRecordChain is null)
      return OverflowInsertResult.DirectionOverflowInvalid;

    lastRecordChain.Direction = _lastPosition;
    _Data[_lastPosition] = record;
    _lastPosition++;
    return OverflowInsertResult.InsertOverflow;
  }

  public override string ToString()
  {
    var sb = new StringBuilder();

    sb.AppendLine("===== OVERFLOW =====");

    int overflowStart = _OVER;

    for (int i = overflowStart; i < _lastPosition; i++)
    {
      var r = _Data[i];

      if (r is null)
        continue;

      if (r.Direction is not null)
        sb.AppendLine($"{i}: [ {r.Id}  →  {r.Direction} ]");
      else
        sb.AppendLine($"{i}: [ {r.Id} ]");
    }

    return sb.ToString();
  }
}