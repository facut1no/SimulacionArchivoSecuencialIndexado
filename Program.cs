

namespace TP_N1_BaseDeDatos;

internal class Program
{
    public static void Main()
    {
        
        while (true)
        {
            PrintMenu();
            var option = GetOption();

            switch (option)
            {
                case 1:
                case 2:
                case 3:
                    return;
                default:
                    Console.WriteLine("Ingrese una opcion valida\n");
                    break;
            }
        }
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
         Console.WriteLine("8) Salir.");
         Console.Write("Opcion: ");
     }
     
    private static int GetOption()
    {
        if(int.TryParse(Console.ReadLine(), out var option))
            return option;
         
        return -1;
    }
}
