

namespace TP_N1_BaseDeDatos;

internal class Program
{
    public static void Main()
    {

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
