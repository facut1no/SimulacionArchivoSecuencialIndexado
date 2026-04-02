internal sealed class FullBlockException() : InvalidOperationException("El bloque de datos esta lleno.");
internal sealed class DataAreaFullException() : InvalidOperationException("El area de datos esta llena.");
internal sealed class OverflowAreaFullException() : InvalidOperationException("El area de overflow esta llena.");