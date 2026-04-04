namespace TP_N1_BaseDeDatos;

public sealed record Record<T>(int Id = 0, T? Data = default, int? Direction = null)
{
    public int Id { get; } = Id;
    public T? Data { get; } = Data;
    public int? Direction { get; set; } = Direction;

    public override string ToString()
    {
        return $"ID: {Id}, DATO: {Data}, DIRRECCION: {Direction?.ToString() ?? "NULL"}";
    }
}