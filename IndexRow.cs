namespace TP_N1_BaseDeDatos;

internal sealed record IndexRow(int keyMin, int blockDirection)
{
  public int KeyMin { get; set; } = keyMin;
  public int BlockDirection { get; } = blockDirection;
  public override string ToString()
  {
    return $"MIN ID: {KeyMin}, DIRRECCION BLOQUE: {BlockDirection}";
  }
}