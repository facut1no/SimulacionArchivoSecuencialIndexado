using System.Runtime.CompilerServices;
using System.Text;
using TP_N1_BaseDeDatos;

internal sealed class Storage<T>(int size)
{
  public Record<T>[] Data { get; } = new Record<T>[size];

  public override string ToString()
  {
    var sb = new StringBuilder();
    sb.Append("[ ");
    foreach (var i in Data)
    {
      sb.Append($"{i} ");
    }
    sb.Append(']');
    return sb.ToString();
  }
}