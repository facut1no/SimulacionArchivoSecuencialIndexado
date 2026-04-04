using System.IO.Pipelines;
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices;
using TP_N1_BaseDeDatos;

internal static class MenuHandler
{

  private static IndexedSequentialFile<int> CreateFile()
  {
    Console.WriteLine("Ingrese la cantidad de registros por bloque: ");
    var N = GetOption();
    if (N is null) return CreateFile();

    Console.WriteLine("Ingrese la cantidad de registros el area principal de datos: ");
    var PMAX = GetOption();
    if (PMAX is null) return CreateFile();

    Console.WriteLine("Ingrese la cantidad de registros en total (Area de datos + overflow): ");
    var OMAX = GetOption();
    if (OMAX is null) return CreateFile();

    return new IndexedSequentialFile<int>(N.Value, PMAX.Value, OMAX.Value);
  }

  private static void PrintMenu()
  {
    Console.Clear();
    Console.WriteLine("==========MENU==========");
    Console.WriteLine("1) Consultar registro.");
    Console.WriteLine("2) Agregar registro.");
    Console.WriteLine("3) Mostrar area de indices.");
    Console.WriteLine("4) Mostrar area de primaria de datos.");
    Console.WriteLine("5) Mostrar area de overflow.");
    Console.WriteLine("6) Mostrar archivo completo.");
    Console.WriteLine("7) Salir.");
    Console.Write("Opcion: ");
  }

  private static int? GetOption()
  {
    if (int.TryParse(Console.ReadLine(), out var option))
      return option;

    return null;
  }

  internal static void Show()
  {
    var file = CreateFile();
    int option;
    while (true)
    {
      PrintMenu();
      option = GetOption() ?? -1;
      Console.Clear();
      switch (option)
      {
        case 1:
          Select(ref file);
          break;
        case 2:
          Insert(ref file);
          break;
        case 3:
          ShowIndexArea(ref file);
          break;
        case 4:
          ShowDataArea(ref file);
          break;
        case 5:
          ShowOverflowArea(ref file);
          break;
        case 6:
          ShowAll(ref file);
          break;
        case 7:
          Console.WriteLine("Saliendo...");
          return;
        default:
          Console.WriteLine("Ingrese una opcion valida.");
          break;
      }
      Console.WriteLine("Presione Enter para continuar.");
      Console.ReadKey();
    }
  }

  private static void Select(ref IndexedSequentialFile<int> file)
  {
    Console.WriteLine("Ingrese el registro a buscar: ");
    var record = GetOption();
    if (record is null)
    {
      Console.WriteLine("Ingrese un registro valido.");
      return;
    }
    var recordSelect = file.Select(record.Value);
    if (recordSelect is null)
      Console.WriteLine($"No se encontro el registro {record}");
    else
      Console.WriteLine(recordSelect);
  }

  private static void Insert(ref IndexedSequentialFile<int> file)
  {
    Console.WriteLine("Ingrese el ID del registro a insertar: ");
    var record = GetOption();
    Console.WriteLine("Ingrese el dato del registro: ");
    var data = GetOption();
    if (record is null || data is null)
    {
      Console.WriteLine("Ingrese un registro valido.");
      return;
    }
    var recordInsert = new Record<int>(record.Value, data.Value);
    file.Insert(recordInsert);
  }

  private static void ShowAll(ref IndexedSequentialFile<int> file)
  {
    Console.WriteLine(file);
  }

  private static void ShowDataArea(ref IndexedSequentialFile<int> file)
  {
    Console.WriteLine(file.GetDataArea());
  }

  private static void ShowOverflowArea(ref IndexedSequentialFile<int> file)
  {
    Console.WriteLine(file.GetOverflow());
  }


  private static void ShowIndexArea(ref IndexedSequentialFile<int> file)
  {
    Console.WriteLine(file.GetIndexArea());
  }
}