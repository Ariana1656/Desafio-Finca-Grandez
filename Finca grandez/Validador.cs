using System;

namespace PrototipoWebFincaGrandez
{
    internal class Validador
    {
        // ---------- Lectura basica ----------
        public static string Leer(string mensaje)
        {
            Console.Write(mensaje);
            string texto = Console.ReadLine();
            if (texto == null)
            {
                texto = "";
            }
            return texto;
        }

        public static void LimpiarPantalla()
        {
             Console.Clear();
        }

        public static void Pausa()
        {
               Console.WriteLine("\nPresione una tecla para continuar...");
               Console.ReadKey(true);
        }

        // ---------- Revisan un texto (devuelven true/false) ----------
        public static bool SoloDigitos(string texto)
        {
            if (texto.Length == 0)
            {
                return false;
            }
            for (int i = 0; i < texto.Length; i++)
            {
                if (texto[i] < '0' || texto[i] > '9')
                {
                    return false;
                }
            }
            return true;
        }

        public static bool TieneNumero(string texto)
        {
            for (int i = 0; i < texto.Length; i++)
            {
                if (texto[i] >= '0' && texto[i] <= '9')
                {
                    return true;
                }
            }
            return false;
        }

        // ---------- Piden un dato hasta que sea valido ----------
        public static string PedirCelular(string mensaje)
        {
            while (true)
            {
                string telefono = Leer(mensaje);
                if (!SoloDigitos(telefono))
                {
                    Console.WriteLine("Error: el celular debe contener solo numeros.");
                }
                else if (telefono.Length == 9 && telefono[0] == '9')
                {
                    return telefono;
                }
                else
                {
                    Console.WriteLine("Error: el celular debe tener 9 digitos y empezar con 9.");
                }
            }
        }

        // Devuelve "S" o "N"
        public static string PedirSN(string mensaje)
        {
            while (true)
            {
                string respuesta = Leer(mensaje).Trim().ToUpper();
                if (respuesta == "S" || respuesta == "N")
                {
                    return respuesta;
                }
                Console.WriteLine("Error: responda S o N.");
            }
        }

        // Texto con un largo minimo (direccion, referencia, razon social...)
        public static string PedirTexto(string mensaje, int minimo, string mensajeError)
        {
            while (true)
            {
                string texto = Leer(mensaje).Trim();
                if (texto.Length >= minimo)
                {
                    return texto;
                }
                Console.WriteLine(mensajeError);
            }
        }

        // Entero entre minimo y maximo (distrito, opciones de menu...)
        public static int PedirEnteroEnRango(string mensaje, int minimo, int maximo)
        {
            while (true)
            {
                int numero;
                if (!int.TryParse(Leer(mensaje), out numero))
                {
                    Console.WriteLine("Error: ingrese solo numeros.");
                }
                else if (numero < minimo || numero > maximo)
                {
                    Console.WriteLine($"Error: ingrese un numero entre {minimo} y {maximo}.");
                }
                else
                {
                    return numero;
                }
            }
        }

        // Numero de exactamente "longitud" digitos (codigo de Yape, etc.)
        public static string PedirDigitos(string mensaje, int longitud, string nombre)
        {
            while (true)
            {
                string valor = Leer(mensaje).Trim();
                if (!SoloDigitos(valor))
                {
                    Console.WriteLine($"Error: el {nombre} debe contener solo numeros.");
                }
                else if (valor.Length == longitud)
                {
                    return valor;
                }
                else
                {
                    Console.WriteLine($"Error: el {nombre} debe tener exactamente {longitud} digitos.");
                }
            }
        }
    }
}
