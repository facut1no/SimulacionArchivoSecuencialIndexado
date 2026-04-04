using System.ComponentModel;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Text;
using System.Transactions;

namespace TP_N1_BaseDeDatos;

internal sealed class IndexedSequentialFile<T>
{
  private readonly IndexArea _indexArea;
  private readonly Storage<T> _storage;
  private readonly DataAreaManager<T> _dataAreaManager;
  private readonly OverflowAreaManager<T> _overflowAreaManager;

  public IndexedSequentialFile(int N, int PMAX, int OMAX)
  {
    _indexArea = new IndexArea(PMAX / N);
    _storage = new Storage<T>(OMAX);
    _dataAreaManager = new DataAreaManager<T>(_storage.Data, N, PMAX);
    _overflowAreaManager = new OverflowAreaManager<T>(_storage.Data, PMAX, OMAX);
  }


  public Record<T>? Select(int id)
  {
    var blockDirection = _indexArea.GetBlockRecordDirection(id);
    if (blockDirection is null)
      return null;

    var record = _dataAreaManager.Select(id, blockDirection.Value);
    if (record is not null)
      return record;
    var overflowStart = _dataAreaManager.GerDirOverflowStart(blockDirection.Value);
    if (overflowStart is null)
      return null;

    record = _overflowAreaManager.Select(id, overflowStart.Value);

    return record; // Si no se encontro en el overflow va a ser null
  }

  public void Insert(Record<T> record)
  {
    var blockDirection = _indexArea.GetBlockRecordDirection(record.Id);

    var resultInserdData = _dataAreaManager.Insert(record, blockDirection);

    blockDirection = resultInserdData.BlockDirection;

    switch (resultInserdData.State)
    {
      case DataInsertState.NewBlockCreated:
        _indexArea.AddIndexRow(record.Id, blockDirection!.Value);
        break;
      case DataInsertState.FirstRecordChanged:
        var firstId = _dataAreaManager.GetIdFirstRecord(blockDirection!.Value);
        _indexArea.ModifyKeyBlock(firstId!.Value, blockDirection.Value);
        break;
      case DataInsertState.BlockFull:
        var lastRecordBlock = _dataAreaManager.GetLastRecordBlock(blockDirection.Value);
        var resultOverflow = _overflowAreaManager.Insert(record, lastRecordBlock!);
        switch (resultOverflow)
        {
          case OverflowInsertResult.OverflowFull:
            Console.WriteLine("Overflow lleno, se necesita reorganizar el archivo.");
            break;
          case OverflowInsertResult.InsertOverflow:
            Console.WriteLine("Se inserto un nuevo registro en el area de overflow.");
            break;
          case OverflowInsertResult.DirectionOverflowInvalid:
            Console.WriteLine("Direccion de overflow invalida.");
            break;
        }
        break;
      case DataInsertState.DataAreaFull:
        Console.WriteLine("No se pueden crear mas bloques, se necesita reorganizar el archivo.");
        break;
      case DataInsertState.InsertedInBlock:
        Console.WriteLine("Se inserto correctamente un nuevo registro en el area de datos.");
        break;
    }
  }

  internal DataAreaManager<T> GetDataArea() => _dataAreaManager;
  internal OverflowAreaManager<T> GetOverflow() => _overflowAreaManager;
  internal IndexArea GetIndexArea() => _indexArea;
  public override string ToString()
  {
    var sb = new StringBuilder();

    sb.AppendLine(_indexArea.ToString());
    sb.AppendLine(_dataAreaManager.ToString());
    sb.AppendLine(_overflowAreaManager.ToString());
    return sb.ToString();
  }

}