namespace TP_N1_BaseDeDatos;

internal sealed record Record<T>(int Id = 0, T? Data = default, int Direction = -1)
{
    public int Id { get; } = Id;
    public T? Data { get; } = Data;
    public int Direction { get; } = Direction;
}