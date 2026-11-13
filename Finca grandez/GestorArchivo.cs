using System;
using System.Collections.Generic;
using System.IO;

namespace PrototipoWebFincaGrandez
{
        public static class GestorArchivo
    {
        const string RUTA_DATOS = "pedidos_datos.txt";

        public static void GuardarPedidosArchivo(Dictionary<string, string[]> diccionarioPedidos)
        {
            StreamWriter datos = new StreamWriter(RUTA_DATOS);
            foreach (var ped in diccionarioPedidos)
            {
                datos.WriteLine(ped.Key + "|" + string.Join("|", ped.Value));
            }
            datos.Close();
        }

        public static void CargarPedidosArchivo(Dictionary<string, string[]> diccionarioPedidos, ref int siguienteIdPedido)
        {
            if (!File.Exists(RUTA_DATOS)) return;
            string[] lineas = File.ReadAllLines(RUTA_DATOS);
            for (int i = 0; i < lineas.Length; i++)
            {
                string[] partes = lineas[i].Split('|');
                if (partes.Length == 14 && !diccionarioPedidos.ContainsKey(partes[0]))
                {
                    string[] datos = new string[13];
                    for (int j = 0; j < 13; j++) datos[j] = partes[j + 1];
                    diccionarioPedidos.Add(partes[0], datos);
                }
            }
            siguienteIdPedido = diccionarioPedidos.Count + 1;
        }
    }
}
