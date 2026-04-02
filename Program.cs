

namespace TP_N1_BaseDeDatos;

internal class Program
{
    public static void Main()
    {
        var dataArea = new DataArea<int>(4, 50, 20);

        Console.WriteLine("=== INSERTANDO BLOQUE INICIAL ===");

        dataArea.Insert(new Record<int>(20, 1), 0);
        dataArea.Insert(new Record<int>(30, 2), 0);
        dataArea.Insert(new Record<int>(40, 3), 0);
        dataArea.Insert(new Record<int>(50, 4), 0);
        dataArea.Insert(new Record<int>(50, 4), 0);
        dataArea.Insert(new Record<int>(50, 4), 0);
        dataArea.Insert(new Record<int>(50, 4), 0);
        dataArea.Insert(new Record<int>(50, 4), 0);
        dataArea.Insert(new Record<int>(50, 4), 0);
        dataArea.Insert(new Record<int>(50, 4), 0);
        dataArea.Insert(new Record<int>(50, 4), 0);
        dataArea.Insert(new Record<int>(50, 4), 0);
        dataArea.Insert(new Record<int>(50, 4), 0);
        dataArea.Insert(new Record<int>(50, 4), 0);
        dataArea.Insert(new Record<int>(50, 4), 0);
        dataArea.Insert(new Record<int>(50, 4), 0);
        dataArea.Insert(new Record<int>(50, 4), 0);
        dataArea.Insert(new Record<int>(50, 4), 0);
        dataArea.Insert(new Record<int>(50, 4), 0);
        dataArea.Insert(new Record<int>(50, 4), 0);
        dataArea.Insert(new Record<int>(50, 4), 0);

        Console.WriteLine(dataArea);

        // 🔹 Inserción normal (con espacio)
        Console.WriteLine("\n=== INSERCIÓN NORMAL ===");
        dataArea.Insert(new Record<int>(35, 99), 0); // debería ordenar
        Console.WriteLine(dataArea);

        // 🔹 Cambio de primer registro
        Console.WriteLine("\n=== CAMBIO PRIMER REGISTRO ===");
        dataArea.Insert(new Record<int>(10, 111), 0);
        Console.WriteLine(dataArea);

        // 🔴 Forzar bloque lleno
        Console.WriteLine("\n=== BLOQUE LLENO ===");
        dataArea.Insert(new Record<int>(60, 200), 0); // depende tu lógica
        Console.WriteLine(dataArea);

        // 🔴 Inserción en el medio → OVERFLOW
        Console.WriteLine("\n=== OVERFLOW ===");
        dataArea.Insert(new Record<int>(25, 555), 0);
        dataArea.Insert(new Record<int>(27, 556), 0);
        Console.WriteLine(dataArea);

        // 🔴 Inserción al final → NUEVO BLOQUE (n/2)
        Console.WriteLine("\n=== NUEVO BLOQUE ===");
        dataArea.Insert(new Record<int>(100, 999), 0);
        Console.WriteLine(dataArea);

        // 🔍 Buscar en bloque
        Console.WriteLine("\n=== BUSCAR EN BLOQUE ===");
        var r1 = dataArea.Select(30, 0);
        Console.WriteLine(r1 != null ? r1.ToString() : "NO ENCONTRADO");

        // 🔍 Buscar en overflow
        Console.WriteLine("\n=== BUSCAR EN OVERFLOW ===");
        var r2 = dataArea.Select(25, 0);
        Console.WriteLine(r2 != null ? r2.ToString() : "NO ENCONTRADO");

        // 🔍 Buscar inexistente
        Console.WriteLine("\n=== BUSCAR INEXISTENTE ===");
        var r3 = dataArea.Select(9999, 0);
        Console.WriteLine(r3 != null ? r3.ToString() : "NO ENCONTRADO");
    }

    private static void PrintMenu()
    {
        Console.Clear();
        Console.WriteLine("==========MENU==========");
        Console.WriteLine("1) Consultar registro.");
        Console.WriteLine("2) Agregar registro.");
        Console.WriteLine("4) Mostrar todos los registros.");
        Console.WriteLine("5) Mostrar area de primaria de datos.");
        Console.WriteLine("7) Mostrar area de overflow.");
        Console.WriteLine("8) Mostrar archivo completo.");
        Console.WriteLine("9) Salir.");
        Console.Write("Opcion: ");
    }

    private static int GetOption()
    {
        if (int.TryParse(Console.ReadLine(), out var option))
            return option;

        return -1;
    }
}
