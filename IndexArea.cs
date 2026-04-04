
using System.Linq.Expressions;
using System.Text;
using TP_N1_BaseDeDatos;

internal sealed class IndexArea(int cantBlocks)
{
  private readonly int _cantblocks = cantBlocks;
  private readonly IndexRow[] _indexs = new IndexRow[cantBlocks];
                
  public bool ModifyKeyBlock(int newKey, int blockDirection)
  {
    for (int i = 0; i < _cantblocks && _indexs[i] is not null; i++)
    {
      if (_indexs[i].BlockDirection == blockDirection)
      {
        _indexs[i].KeyMin = newKey;
        SortIndexs();
        return true;
      }
    }
    return false;
  }

  public void AddIndexRow(int key, int blockDirection)
  {
    var index = new IndexRow(key, blockDirection);
    for (int i = 0; i < _cantblocks; i++)
      if (_indexs[i] is null)
      {
        _indexs[i] = index;
        break;
      }
    SortIndexs();
  }

  public int? GetBlockRecordDirection(int key)
  {
    if (_indexs[0] is null)
      return null;

    if (key < _indexs[0].KeyMin)
      return _indexs[0].BlockDirection;

    int? blockIndex = null;

    for (int i = 0; i < _cantblocks && _indexs[i] is not null; i++)
    {
      if (key >= _indexs[i].KeyMin)
        blockIndex = _indexs[i].BlockDirection;
      else
        break;
    }

    return blockIndex;
  }

  private void SortIndexs()
  {
    Array.Sort(_indexs, Comparer<IndexRow>.Create((x, y) =>
    {
      if (x is null && y is null) return 0;
      if (x is null) return 1;
      if (y is null) return -1;
      return x.KeyMin.CompareTo(y.keyMin);
    }));
  }

  public override string ToString()
  {
    var sb = new StringBuilder();
    sb.AppendLine("=====INDICES=====");
    for (int i = 0; i < _cantblocks && _indexs[i] is not null; i++)
    {
      sb.AppendLine(_indexs[i].ToString());
    }
    return sb.ToString();
  }

}