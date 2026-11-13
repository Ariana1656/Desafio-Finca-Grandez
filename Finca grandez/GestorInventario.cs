using System;
using System.Collections.Generic;

namespace PrototipoWebFincaGrandez
{
    public static class GestorInventario
    {
        const int I_NOMBRE = 0, I_PRESENTACION = 1, I_PRECIO = 2, I_STOCK = 3;

        static Dictionary<int, string[]> diccionarioInventario = new Dictionary<int, string[]>()
        {
            { 1, new string[] { "Blend Premium (250g)", "(250g)", "32.00", "20" } },
            { 2, new string[] { "Blend Premium (500g)", "(500g)", "52.00", "10" } },
            { 3, new string[] { "Blend Premium (1kg)", "(1kg)", "72.00", "5" } },
            { 4, new string[] { "Cafe Tradicion (250g)", "(250g)", "28.00", "15" } },
            { 5, new string[] { "Cafe Tradicion (500g)", "(500g)", "48.00", "10" } },
            { 6, new string[] { "Cafe Tradicion (1kg)", "(1kg)", "68.00", "5" } },
            { 7, new string[] { "Cafe Catuai (250g)", "(250g)", "35.00", "10" } },
            { 8, new string[] { "Cafe Catuai (500g)", "(500g)", "65.00", "5" } },
            { 9, new string[] { "Cafe Geisha (250g)", "(250g)", "45.00", "10" } }
        };

        public static int ObtenerTotalProductos() => diccionarioInventario.Count;
        public static bool ExisteProducto(int numero) => diccionarioInventario.ContainsKey(numero);
        public static int ObtenerStock(int numero) => int.Parse(diccionarioInventario[numero][I_STOCK]);
        public static double ObtenerPrecio(int numero) => double.Parse(diccionarioInventario[numero][I_PRECIO]);
        public static string ObtenerNombre(int numero) => diccionarioInventario[numero][I_NOMBRE];
        public static string ObtenerPresentacion(int numero) => diccionarioInventario[numero][I_PRESENTACION];

        public static void CambiarStock(int numero, int cambio)
        {
            int nuevo = ObtenerStock(numero) + cambio;
            diccionarioInventario[numero][I_STOCK] = nuevo.ToString();
        }

        public static int StockTotal(int numero)
        {
             if (numero == 0) return 0;                             // caso base
             return ObtenerStock(numero) + StockTotal(numero - 1);  // caso recursivo
        }

        public static void MostrarCatalogo()
        {
            Console.WriteLine("\n=========================================================");
            Console.WriteLine("           CATALOGO FINCA GRANDEZ Y STOCK");
            Console.WriteLine("=========================================================");
            Console.WriteLine("    ID    Presentacion    Precio          Stock");
            for (int numero = 1; numero <= diccionarioInventario.Count; numero++)
            {
                if (numero == 1) Console.WriteLine("\n BLEND PREMIUM");
                else if (numero == 4) Console.WriteLine("\n CAFE TRADICION");
                else if (numero == 7) Console.WriteLine("\n CAFE CATUAI");
                else if (numero == 9) Console.WriteLine("\n CAFE GEISHA");
                Console.WriteLine($"   [{numero}]  {ObtenerPresentacion(numero)}\t\tS/ {ObtenerPrecio(numero):F2}\t\t{ObtenerStock(numero)} und.");
            }
            int total = StockTotal(ObtenerTotalProductos());
            Console.WriteLine($"\n Stock total en almacen: {total} und.");
            Console.WriteLine("=========================================================\n");
        }
    }
}
