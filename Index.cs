namespace TP_N1_BaseDeDatos;

internal record Index(int keyMin, int directionBlock)
{
  public int KeyMin { get; } = keyMin;
  public int DirectionBlock { get; } = directionBlock;

  public override string ToString()
  {
    return $"MIN ID: {KeyMin}, DIRRECCION BLOQUE: {DirectionBlock}";
  }
}