using System;
using System.Collections.Generic;
using System.IO;

namespace PrototipoWebFincaGrandez
{
    internal class Program
    {
        // =================================================================================
        // 1. VARIABLES GLOBALES (Diccionarios = similar a una base de datos)
        // =================================================================================

        // USUARIOS. Clave: nick
        // [0] Password, [1] Rol, [2] Nombre, [3] Direccion, [4] Correo
        const int U_CLAVE = 0, U_ROL = 1, U_NOMBRE = 2, U_DIRECCION = 3, U_CORREO = 4;
        static Dictionary<string, string[]> diccionarioUsuarios = new Dictionary<string, string[]>()
        {
            { "admin", new string[] { "admin", "admin", "Administrador Finca Grandez", "", "admin@fincagrandez.com" } },
            { "user", new string[] { "user", "user", "Cliente Prueba", "Av. Falsa 123", "user@test.com" } }
        };

        // INVENTARIO. Clave: id del producto
        // [0] Nombre, [1] Presentacion, [2] Precio, [3] Stock
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

        // PEDIDOS. Clave: id del pedido (ej. "PED-0001")
        // [0] Fecha, [1] Hora, [2] Usuario (nick), [3] Cliente, [4] Producto, [5] Cantidad,
        // [6] Entrega, [7] Direccion, [8] Referencia, [9] Tel. contacto, [10] Dias, [11] Total, [12] Estado
        const int P_FECHA = 0, P_HORA = 1, P_USUARIO = 2, P_CLIENTE = 3, P_PRODUCTO = 4, P_CANTIDAD = 5,
                  P_ENTREGA = 6, P_DIRECCION = 7, P_REFERENCIA = 8, P_TELEFONO = 9, P_DIAS = 10,
                  P_TOTAL = 11, P_ESTADO = 12;
        static Dictionary<string, string[]> diccionarioPedidos = new Dictionary<string, string[]>();

        // Correlativo de pedidos
        static int siguienteIdPedido = 1;

        // VENTAS POR TIPO DE CAFE (para el reporte de mas vendidos)
        static int[] unidadesCafe = new int[4];
        static double[] montosCafe = new double[4];

        // DELIVERY
        const double COSTO_DELIVERY = 15.00;
        const string DIAS_ENTREGA = "1 a 3 dias";

        // ARCHIVOS TXT
        const string RUTA_PEDIDOS = "pedidos.txt";       // reporte legible
        const string RUTA_DATOS = "pedidos_datos.txt";   // datos para recargar

        // DATOS DE FINCA GRANDEZ (para la boleta)
        const string EMISOR_NOMBRE = "CORPORACION GRANDEZ S.A.C.";
        const string EMISOR_RUC = "20602996931";
        const string EMISOR_DIRECCION = "C. JOSE ANTONIO ENCINAS 184 - PUEBLO LIBRE";
        static string[] distritosLima =
        {
            "Cercado de Lima", "Brena", "La Victoria", "Rimac", "San Luis", "Lince",
            "Jesus Maria", "Pueblo Libre", "Magdalena del Mar", "San Miguel", "San Isidro",
            "Miraflores", "Surquillo", "Barranco", "San Borja", "Santiago de Surco",
            "Chorrillos", "La Molina", "Los Olivos", "San Martin de Porres", "Independencia",
            "Comas", "Carabayllo", "Puente Piedra", "Ancon", "Santa Anita", "Ate",
            "El Agustino", "San Juan de Lurigancho", "Lurigancho", "Chaclacayo",
            "San Juan de Miraflores", "Villa El Salvador", "Villa Maria del Triunfo"
        };

        // SESION ACTUAL
        static string usuarioActual = "";
        static string rolActual = "";



        // =================================================================================
        // 2. FUNCIONES BASICAS
        // =================================================================================
        static string Leer(string mensaje)
        {
            Console.Write(mensaje);
            string texto = Console.ReadLine();
            if (texto == null)
            {
                texto = "";
            }
            return texto;
        }

        static void LimpiarPantalla()
        {
            Console.Clear();
        }

        // Evita que LimpiarPantalla borre el resultado antes de leerlo
        static void Pausa()
        {
            Console.WriteLine("\nPresione una tecla para continuar...");
            Console.ReadKey(true);
        }

        static bool SoloDigitos(string texto)
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

        static bool TieneNumero(string texto)
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

        static string PedirCelular(string mensaje)
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

        static string GenerarIdPedido()
        {
            return "PED-" + siguienteIdPedido.ToString("D4");
        }

        // ==========================================================
        // FUNCIONES DE ACCESO A LOS DICCIONARIOS
        // ==========================================================
        static int ObtenerStock(int numero)
        {
            return int.Parse(diccionarioInventario[numero][I_STOCK]);
        }

        static double ObtenerPrecio(int numero)
        {
            return double.Parse(diccionarioInventario[numero][I_PRECIO]);
        }

        static string ObtenerNombre(int numero)
        {
            return diccionarioInventario[numero][I_NOMBRE];
        }

        // cambio positivo = suma stock, negativo = resta stock
        static void CambiarStock(int numero, int cambio)
        {
            int nuevo = ObtenerStock(numero) + cambio;
            diccionarioInventario[numero][I_STOCK] = nuevo.ToString();
        }

        static double ObtenerTotalPedido(string[] pedido)
        {
            return double.Parse(pedido[P_TOTAL]);
        }

        // Devuelve el rol si el login es correcto, o "fallo" si no
        static string ValidarLogin(string usuario, string clave)
        {
            if (diccionarioUsuarios.ContainsKey(usuario) && diccionarioUsuarios[usuario][U_CLAVE] == clave)
            {
                return diccionarioUsuarios[usuario][U_ROL];
            }
            return "fallo";
        }

        static bool CorreoYaRegistrado(string correo)
        {
            foreach (var u in diccionarioUsuarios)
            {
                if (u.Value[U_CORREO] == correo)
                {
                    return true;
                }
            }
            return false;
        }

        static void RegistrarUsuario(string nick, string clave, string rol, string nombre, string direccion, string correo)
        {
            diccionarioUsuarios.Add(nick, new string[] { clave, rol, nombre, direccion, correo });
        }

        // ==========================================================
        // ARCHIVO DE PEDIDOS (pedidos.txt)
        // Cada linea: id|fecha|hora|usuario|cliente|producto|... (13 campos + id)
        // ==========================================================
        static void GuardarPedidosArchivo()
        {
            // 1. Archivo de datos (para volver a cargar los pedidos al abrir el programa)
            StreamWriter datos = new StreamWriter(RUTA_DATOS);
            foreach (var ped in diccionarioPedidos)
            {
                datos.WriteLine(ped.Key + "|" + string.Join("|", ped.Value));
            }
            datos.Close();

            // 2. Archivo legible (reporte de pedidos)
            StreamWriter escritor = new StreamWriter(RUTA_PEDIDOS);
            int completados = 0;
            escritor.WriteLine("=========================================================================");
            escritor.WriteLine("                    PEDIDOS REGISTRADOS - FINCA GRANDEZ");
            escritor.WriteLine("=========================================================================");
            escritor.WriteLine($"{"ID",-10}{"Fecha",-12}{"Hora",-7}{"Cliente",-22}{"Total",-14}Estado");
            escritor.WriteLine("-------------------------------------------------------------------------");
            foreach (var ped in diccionarioPedidos)
            {
                string[] p = ped.Value;
                string estadoTexto = "PENDIENTE";
                if (p[P_ESTADO] == "Completado")
                {
                    estadoTexto = "COMPLETADO";
                    completados++;
                }
                string total = "S/ " + ObtenerTotalPedido(p).ToString("F2");
                escritor.WriteLine($"{ped.Key,-10}{p[P_FECHA],-12}{p[P_HORA],-7}{p[P_CLIENTE],-22}{total,-14}{estadoTexto}");
                escritor.WriteLine($"   Producto:   {p[P_PRODUCTO]}");
                escritor.WriteLine($"   Entrega:    {p[P_ENTREGA]}");
                escritor.WriteLine($"   Cliente:    {p[P_CLIENTE]}");
                escritor.WriteLine($"   Usuario:    {p[P_USUARIO]}");
                escritor.WriteLine($"   Cantidad:   {p[P_CANTIDAD]}");
                escritor.WriteLine($"   Direccion:  {p[P_DIRECCION]}");
                escritor.WriteLine($"   Referencia: {p[P_REFERENCIA]}");
                escritor.WriteLine($"   Contacto:   {p[P_TELEFONO]}");
                escritor.WriteLine($"   Plazo:      {p[P_DIAS]}");
                escritor.WriteLine("-------------------------------------------------------------------------");
            }
            escritor.WriteLine($"Total de pedidos: {diccionarioPedidos.Count}");
            escritor.WriteLine($"- Completados: {completados}");
            escritor.Close();
            Console.WriteLine("Pedidos guardados en " + RUTA_PEDIDOS);
        }

        static void CargarPedidosArchivo()
        {
            if (!File.Exists(RUTA_DATOS))
            {
                return;
            }
            string[] lineas = File.ReadAllLines(RUTA_DATOS);
            for (int i = 0; i < lineas.Length; i++)
            {
                string[] partes = lineas[i].Split('|');
                // 1 id + 13 campos del pedido
                if (partes.Length == 14 && !diccionarioPedidos.ContainsKey(partes[0]))
                {
                    string[] datos = new string[13];
                    for (int j = 0; j < 13; j++)
                    {
                        datos[j] = partes[j + 1];
                    }
                    diccionarioPedidos.Add(partes[0], datos);
                }
            }
            siguienteIdPedido = diccionarioPedidos.Count + 1;
        }

        // ==========================================================
        // CATALOGO Y DISTRITOS
        // ==========================================================
        static void MostrarCatalogo()
        {
            Console.WriteLine("\n=========================================================");
            Console.WriteLine("           CATALOGO FINCA GRANDEZ Y STOCK");
            Console.WriteLine("=========================================================");
            Console.WriteLine("   ID    Presentacion    Precio         Stock");
            for (int numero = 1; numero <= diccionarioInventario.Count; numero++)
            {
                if (numero == 1)
                {
                    Console.WriteLine("\n BLEND PREMIUM");
                }
                else if (numero == 4)
                {
                    Console.WriteLine("\n CAFE TRADICION");
                }
                else if (numero == 7)
                {
                    Console.WriteLine("\n CAFE CATUAI");
                }
                else if (numero == 9)
                {
                    Console.WriteLine("\n CAFE GEISHA");
                }
                Console.WriteLine($"   [{numero}]  {diccionarioInventario[numero][I_PRESENTACION]}\t\tS/ {ObtenerPrecio(numero):F2}\t\t{ObtenerStock(numero)} und.");
            }
            Console.WriteLine("=========================================================\n");
        }

        static string ElegirDistrito()
        {
            Console.WriteLine("\nDistritos con delivery (solo Lima):");
            for (int i = 0; i < distritosLima.Length; i++)
            {
                Console.WriteLine($"{i + 1}. {distritosLima[i]}");
            }
            while (true)
            {
                string entrada = Leer("Elija su distrito (numero): ");
                int opcion;
                if (!int.TryParse(entrada, out opcion))
                {
                    Console.WriteLine("Error: ingrese solo el numero del distrito.");
                }
                else if (opcion >= 1 && opcion <= distritosLima.Length)
                {
                    return distritosLima[opcion - 1];
                }
                else
                {
                    Console.WriteLine("Error: ingrese un numero de la lista.");
                }
            }
        }

        // ==========================================================
        // PEDIDOS
        // ==========================================================
        static void BuscarPedido()
        {
            Console.WriteLine("\n--- BUSCAR PEDIDO ---");
            string idBuscado = Leer("Ingrese el ID del pedido (ej. PED-0001): ").Trim().ToUpper();

            // El admin ve cualquier pedido; el cliente solo los suyos
            if (diccionarioPedidos.ContainsKey(idBuscado)
                && (rolActual == "admin" || diccionarioPedidos[idBuscado][P_USUARIO] == usuarioActual))
            {
                string[] p = diccionarioPedidos[idBuscado];
                Console.WriteLine("\nPedido encontrado");
                Console.WriteLine($"Estado:     {p[P_ESTADO]}");
                Console.WriteLine("Detalle:");
                Console.WriteLine($"  ID:        {idBuscado}");
                Console.WriteLine($"  Fecha:     {p[P_FECHA]} {p[P_HORA]}");
                Console.WriteLine($"  Cliente:   {p[P_CLIENTE]}");
                Console.WriteLine($"  Producto:  {p[P_PRODUCTO]}");
                Console.WriteLine($"  Unidades:  {p[P_CANTIDAD]}");
                Console.WriteLine($"  Entrega:   {p[P_ENTREGA]} - {p[P_DIRECCION]}");
                if (p[P_ENTREGA] == "Delivery")
                {
                    Console.WriteLine($"  Referencia:       {p[P_REFERENCIA]}");
                    Console.WriteLine($"  Tel. contacto:    {p[P_TELEFONO]}");
                    Console.WriteLine($"  Entrega estimada: {p[P_DIAS]}");
                }
                Console.WriteLine($"  Total:     S/ {ObtenerTotalPedido(p):F2}\n");
            }
            else
            {
                Console.WriteLine("El pedido no existe.\n");
            }
        }

        // PEDIDOS DEL CLIENTE QUE INICIO SESION
        static void MostrarMisPedidos()
        {
            Console.WriteLine("\n--- MIS PEDIDOS ---");
            int encontrados = 0;
            foreach (var ped in diccionarioPedidos)
            {
                if (ped.Value[P_USUARIO] == usuarioActual)
                {
                    encontrados++;
                    Console.WriteLine($"{ped.Key} | {ped.Value[P_FECHA]} {ped.Value[P_HORA]} | {ped.Value[P_PRODUCTO]} | S/ {ObtenerTotalPedido(ped.Value):F2} | {ped.Value[P_ESTADO]}");
                }
            }
            if (encontrados == 0)
            {
                Console.WriteLine("Aun no tiene pedidos.");
            }
        }

        // TODOS LOS PEDIDOS ORDENADOS POR TOTAL (BURBUJA)
        static void MostrarTodosLosPedidos()
        {
            Console.WriteLine("\n--- TODOS LOS PEDIDOS ---");

            int cantidad = diccionarioPedidos.Count;
            if (cantidad == 0)
            {
                Console.WriteLine("No hay pedidos registrados.");
                return;
            }

            // Cada posicion guarda el id de un pedido del diccionario.
            string[] orden = new string[cantidad];
            diccionarioPedidos.Keys.CopyTo(orden, 0);

            // Burbuja: se ordenan los ids de mayor a menor total.
            for (int i = 0; i < cantidad - 1; i++)
            {
                for (int j = 0; j < cantidad - 1 - i; j++)
                {
                    if (ObtenerTotalPedido(diccionarioPedidos[orden[j]]) < ObtenerTotalPedido(diccionarioPedidos[orden[j + 1]]))
                    {
                        string auxiliar = orden[j];
                        orden[j] = orden[j + 1];
                        orden[j + 1] = auxiliar;
                    }
                }
            }

            for (int puesto = 0; puesto < cantidad; puesto++)
            {
                string id = orden[puesto];
                string[] p = diccionarioPedidos[id];
                Console.WriteLine("---------------------------------------------------------");
                Console.WriteLine($"Puesto:     {puesto + 1}");
                Console.WriteLine($"ID:         {id}");
                Console.WriteLine($"Fecha:      {p[P_FECHA]} {p[P_HORA]}");
                Console.WriteLine($"Cliente:    {p[P_CLIENTE]}");
                Console.WriteLine($"Producto:   {p[P_PRODUCTO]}");
                Console.WriteLine($"Cantidad:   {p[P_CANTIDAD]}");
                Console.WriteLine($"Entrega:    {p[P_ENTREGA]}");
                Console.WriteLine($"Direccion:  {p[P_DIRECCION]}");
                if (p[P_ENTREGA] == "Delivery")
                {
                    Console.WriteLine($"Referencia: {p[P_REFERENCIA]}");
                    Console.WriteLine($"Contacto:   {p[P_TELEFONO]}");
                    Console.WriteLine($"Plazo:      {p[P_DIAS]}");
                }
                Console.WriteLine($"Estado:     {p[P_ESTADO]}");
                Console.WriteLine($"Total:      S/ {ObtenerTotalPedido(p):F2}");
            }
            Console.WriteLine("---------------------------------------------------------");
            Console.WriteLine($"Total de pedidos registrados: {cantidad}");
        }

        // ==========================================================
        // FUNCIONES DEL ADMINISTRADOR
        // ==========================================================
        static void ReponerStock()
        {
            Console.WriteLine("\n--- INVENTARIO Y REPOSICION DE STOCK ---");
            MostrarCatalogo();

            int numero;
            while (true)
            {
                string entrada = Leer("Ingrese el ID del producto a reponer (o 0 para cancelar): ").Trim();
                if (!int.TryParse(entrada, out numero))
                {
                    Console.WriteLine("Error: ingrese solo el numero del producto.");
                }
                else if (numero == 0)
                {
                    return;
                }
                else if (diccionarioInventario.ContainsKey(numero))
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Error: opcion de producto invalida.");
                }
            }

            int cantidad;
            while (true)
            {
                string entrada = Leer("Cantidad a sumar: ").Trim();
                if (!int.TryParse(entrada, out cantidad))
                {
                    Console.WriteLine("Error: ingrese solo numeros.");
                }
                else if (cantidad <= 0)
                {
                    Console.WriteLine("Error: la cantidad debe ser mayor a 0.");
                }
                else
                {
                    break;
                }
            }

            CambiarStock(numero, cantidad);
            Console.WriteLine($"Stock actualizado: {ObtenerNombre(numero)} ahora tiene {ObtenerStock(numero)} und.");
        }

        static void CompletarPedidos()
        {
            Console.WriteLine("\n--- PEDIDOS PENDIENTES ---");
            int pendientes = 0;
            foreach (var ped in diccionarioPedidos)
            {
                if (ped.Value[P_ESTADO] != "Completado")
                {
                    pendientes++;
                    Console.WriteLine($"{ped.Key} | Usuario: {ped.Value[P_USUARIO]} | Cliente: {ped.Value[P_CLIENTE]} | {ped.Value[P_PRODUCTO]} | {ped.Value[P_ENTREGA]} | {ped.Value[P_ESTADO]}");
                }
            }
            if (pendientes == 0)
            {
                Console.WriteLine("No hay pedidos pendientes.");
                return;
            }

            while (true)
            {
                string idPedido = Leer("Ingrese el ID del pedido a completar (ej. PED-0001, o 0 para salir): ").Trim().ToUpper();
                if (idPedido == "0")
                {
                    return;
                }
                if (!diccionarioPedidos.ContainsKey(idPedido))
                {
                    Console.WriteLine("Error: ese pedido no existe.");
                }
                else if (diccionarioPedidos[idPedido][P_ESTADO] == "Completado")
                {
                    Console.WriteLine("Ese pedido ya estaba completado.");
                }
                else
                {
                    diccionarioPedidos[idPedido][P_ESTADO] = "Completado";
                    Console.WriteLine("Pedido marcado como completado.");
                    return;
                }
            }
        }

        // ==========================================================
        // REPORTES
        // ==========================================================
        // Escribe el resumen en cualquier salida (consola o archivo)
        static void EscribirResumen(TextWriter salida, int cantidad, int completados, int delivery, int recojo, double totalVendido)
        {
            salida.WriteLine("---------------------------------------------------------");
            salida.WriteLine("RESUMEN DEL DIA");
            salida.WriteLine($"Pedidos de hoy:       {cantidad}");
            salida.WriteLine($"Pedidos completados:  {completados}");
            salida.WriteLine($"Delivery:             {delivery}");
            salida.WriteLine($"Recojo en tienda:     {recojo}");
            salida.WriteLine($"Total vendido:        S/ {totalVendido:F2}");
            salida.WriteLine("---------------------------------------------------------");
        }

        static void ReporteDiario()
        {
            string hoy = DateTime.Now.ToString("yyyy-MM-dd");
            int cantidad = 0;
            int completados = 0;
            int delivery = 0;
            int recojo = 0;
            double totalVendido = 0;

            Console.WriteLine("\n--- REPORTE DIARIO (" + hoy + ") ---");
            Console.WriteLine("---------------------------------------------------------");
            foreach (var ped in diccionarioPedidos)
            {
                // Solo cuentan los pedidos de hoy.
                if (ped.Value[P_FECHA] == hoy)
                {
                    cantidad++;
                    totalVendido = totalVendido + ObtenerTotalPedido(ped.Value);
                    if (ped.Value[P_ENTREGA] == "Delivery")
                    {
                        delivery++;
                    }
                    else
                    {
                        recojo++;
                    }
                    if (ped.Value[P_ESTADO] == "Completado")
                    {
                        completados++;
                    }
                    Console.WriteLine($"{ped.Key} | {ped.Value[P_HORA]} | {ped.Value[P_CLIENTE]} | {ped.Value[P_PRODUCTO]} | {ped.Value[P_CANTIDAD]} und. | S/ {ObtenerTotalPedido(ped.Value):F2}");
                }
            }
            if (cantidad == 0)
            {
                Console.WriteLine("No hay pedidos registrados hoy.");
            }
            EscribirResumen(Console.Out, cantidad, completados, delivery, recojo, totalVendido);

            // Preguntar si se guardan los pedidos en el archivo
            if (diccionarioPedidos.Count > 0)
            {
                string respuesta = "";
                while (true)
                {
                    respuesta = Leer("\n¿Desea guardar los pedidos en archivo? (S = Si / N = No): ").Trim().ToUpper();
                    if (respuesta == "S" || respuesta == "N")
                    {
                        break;
                    }
                    Console.WriteLine("Error: responda S o N.");
                }
                if (respuesta == "S")
                {
                    GuardarPedidosArchivo();
                }
            }
        }

        // Ordena de mayor a menor con burbuja.
        // porVenta = true ordena por venta total, false ordena por unidades.
        static void OrdenarRanking(string[] tipos, int[] unidades, double[] ventas, bool porVenta)
        {
            for (int i = 0; i < tipos.Length - 1; i++)
            {
                for (int j = 0; j < tipos.Length - 1 - i; j++)
                {
                    bool cambiar = false;
                    if (porVenta)
                    {
                        if (ventas[j] < ventas[j + 1])
                        {
                            cambiar = true;
                        }
                    }
                    else
                    {
                        if (unidades[j] < unidades[j + 1])
                        {
                            cambiar = true;
                        }
                    }

                    if (cambiar)
                    {
                        string auxTipo = tipos[j];
                        tipos[j] = tipos[j + 1];
                        tipos[j + 1] = auxTipo;

                        int auxUnidades = unidades[j];
                        unidades[j] = unidades[j + 1];
                        unidades[j + 1] = auxUnidades;

                        double auxVenta = ventas[j];
                        ventas[j] = ventas[j + 1];
                        ventas[j + 1] = auxVenta;
                    }
                }
            }
        }

        static void ProductosMasVendidos()
        {
            string[] tipos = { "Blend Premium", "Cafe Tradicion", "Cafe Catuai", "Cafe Geisha" };
            int[] unidades = new int[4];
            double[] ventas = new double[4];

            // Copiar las ventas acumuladas (asi no se desordenan los originales)
            for (int i = 0; i < tipos.Length; i++)
            {
                unidades[i] = unidadesCafe[i];
                ventas[i] = montosCafe[i];
            }

            // RANKING 1: POR UNIDADES VENDIDAS
            OrdenarRanking(tipos, unidades, ventas, false);

            Console.WriteLine("\n===============================================================");
            Console.WriteLine("       UNIDADES MAS VENDIDAS - FINCA GRANDEZ");
            Console.WriteLine("===============================================================");
            Console.WriteLine($"{"Puesto",-9}{"Tipo de cafe",-20}Unidades vendidas");
            for (int i = 0; i < tipos.Length; i++)
            {
                string puesto = "[" + (i + 1) + "]";
                Console.WriteLine($"{puesto,-9}{tipos[i],-20}{unidades[i]} und.");
            }
            Console.WriteLine("===============================================================");

            // RANKING 2: POR VENTA TOTAL EN SOLES
            OrdenarRanking(tipos, unidades, ventas, true);

            Console.WriteLine("\n===============================================================");
            Console.WriteLine("       PRODUCTOS CON MAYOR VENTA - FINCA GRANDEZ");
            Console.WriteLine("===============================================================");
            Console.WriteLine($"{"Puesto",-9}{"Tipo de cafe",-20}Venta total");
            for (int i = 0; i < tipos.Length; i++)
            {
                string puesto = "[" + (i + 1) + "]";
                Console.WriteLine($"{puesto,-9}{tipos[i],-20}S/ {ventas[i]:F2}");
            }
            Console.WriteLine("===============================================================");
        }

        // ==========================================================
        // BOLETA / FACTURA
        // ==========================================================
        // Centra un texto en 56 caracteres (el largo de la linea de guiones)
        static string Centrar(string texto)
        {
            return texto.PadLeft((56 + texto.Length) / 2);
        }

        static void MostrarBoleta(string tipoComprobante, string idPedido,
            string numDocumento, string razonSocial, string nombreCliente, string telefonoCliente,
            string direccionCliente, string distritoCliente, string textoEntrega,
            string referenciaCliente, string telContacto, string diasEntregas,
            int[] carrito,
            double subtotal, double costoDelivery, double totalPagar, string codigoOperacionYape)
        {
            Console.WriteLine();
            Console.WriteLine(Centrar(EMISOR_NOMBRE));
            Console.WriteLine(Centrar("FINCA GRANDEZ | RUC: " + EMISOR_RUC));
            Console.WriteLine(Centrar(EMISOR_DIRECCION));

            if (tipoComprobante == "1")
            {
                Console.WriteLine(Centrar("BOLETA DE VENTA ELECTRONICA: B001-" + idPedido));
            }
            else
            {
                Console.WriteLine(Centrar("FACTURA ELECTRONICA: F001-" + idPedido));
            }
            Console.WriteLine();

            // ---- DATOS DEL CLIENTE ----
            if (tipoComprobante == "2")
            {
                Console.WriteLine("RUC: " + numDocumento + " | " + razonSocial);
                Console.WriteLine("Contacto: " + nombreCliente);
            }
            else
            {
                Console.WriteLine("DNI: " + numDocumento + " | " + nombreCliente);
            }
            Console.WriteLine("Fecha: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
            Console.WriteLine("Telefono: " + telefonoCliente);
            Console.WriteLine("Direccion: " + direccionCliente + ", " + distritoCliente);
            if (textoEntrega == "Delivery")
            {
                Console.WriteLine("Referencia: " + referenciaCliente);
                Console.WriteLine("Tel. contacto: " + telContacto);
                Console.WriteLine("Entrega en: " + diasEntregas);
            }
            else
            {
                Console.WriteLine("Entrega: Recojo en tienda");
            }

            // ---- DETALLE ----
            Console.WriteLine("--------------------------------------------------------");
            Console.WriteLine($"{"ARTICULO",-30}{"CANT",6}{"P/U",10}{"TOTAL",10}");
            Console.WriteLine("--------------------------------------------------------");
            int items = 0;
            for (int i = 0; i < carrito.Length; i++)
            {
                if (carrito[i] > 0)
                {
                    items = items + 1;
                    double precioUnitario = ObtenerPrecio(i + 1);
                    double totalProducto = precioUnitario * carrito[i];
                    Console.WriteLine($"{ObtenerNombre(i + 1),-30}{carrito[i],6}{precioUnitario,10:F2}{totalProducto,10:F2}");
                }
            }
            Console.WriteLine("--------------------------------------------------------");

            // ---- TOTALES ----
            double igv = totalPagar - totalPagar / 1.18;
            string textoItems = "Items: " + items;
            Console.WriteLine($"{"SUBTOTAL PEDIDO:",41}  S/ {subtotal,10:F2}");
            Console.WriteLine($"{"COSTO ENVIO:",41}  S/ {costoDelivery,10:F2}");
            Console.WriteLine($"{"IGV (18%):",41}  S/ {igv,10:F2}");
            Console.WriteLine($"{textoItems,-14}{"TOTAL A PAGAR:",27}  S/ {totalPagar,10:F2}");
            Console.WriteLine("--------------------------------------------------------");

            // ---- PAGO ----
            Console.WriteLine("Forma de pago: YAPE");
            Console.WriteLine("Pagado a: 987 654 321 (Finca Grandez)");
            Console.WriteLine("Operacion ID: " + codigoOperacionYape);
            Console.WriteLine("--------------------------------------------------------");
            Console.WriteLine(Centrar("*Pedido registrado correctamente!"));
            Console.WriteLine();
        }

        // ==========================================================
        // CUENTA Y SESION
        // ==========================================================
        static void CrearCuenta()
        {
            Console.WriteLine("\n--- CREACION DE CUENTA ---");

            // Correo
            string correoNuevo = "";
            while (true)
            {
                string correoInput = Leer("Correo electronico: ").Trim().ToLower();
                if (correoInput == "")
                {
                    Console.WriteLine("Error: el correo no puede estar vacio.");
                }
                else if (!correoInput.Contains("@"))
                {
                    Console.WriteLine("Error: el correo debe contener el simbolo '@'.");
                }
                else if (!correoInput.EndsWith(".com"))
                {
                    Console.WriteLine("Error: el correo debe terminar obligatoriamente en '.com'.");
                }
                else if (correoInput.StartsWith("@") || correoInput.Split('@')[1] == ".com")
                {
                    Console.WriteLine("Error: el correo esta incompleto.");
                }
                else if (CorreoYaRegistrado(correoInput))
                {
                    Console.WriteLine("Error: ese correo ya esta registrado. Use otro.");
                }
                else
                {
                    correoNuevo = correoInput;
                    break;
                }
            }

            // Usuario
            string usuarioNuevo = "";
            while (true)
            {
                usuarioNuevo = Leer("Nombre de usuario : ").Trim().ToLower();
                if (usuarioNuevo.Length < 3)
                {
                    Console.WriteLine("Error: el usuario debe tener al menos 3 caracteres.");
                }
                else if (diccionarioUsuarios.ContainsKey(usuarioNuevo))
                {
                    Console.WriteLine("Error: ese usuario ya existe. Elija otro.");
                }
                else
                {
                    break;
                }
            }

            // Contrasena
            string claveTemporal = "";
            while (true)
            {
                claveTemporal = Leer("Contrasena (minimo 4 caracteres): ");
                if (claveTemporal.Length >= 4)
                {
                    break;
                }
                Console.WriteLine("Error: la contrasena debe tener al menos 4 caracteres.");
            }

            // Confirmar contrasena
            string claveConfirmacion = "";
            while (true)
            {
                claveConfirmacion = Leer("Confirme su contrasena: ");
                if (claveConfirmacion == claveTemporal)
                {
                    break;
                }
                Console.WriteLine("Error: las contrasenas no coinciden.");
            }

            // Recien aqui se guarda todo. Toda cuenta nueva es de cliente ("user").
            RegistrarUsuario(usuarioNuevo, claveConfirmacion, "user", usuarioNuevo, "", correoNuevo);
            Console.WriteLine("\n*Cuenta creada exitosamente! Ahora puede iniciar sesion.\n");
        }

        static void IniciarSesion()
        {
            string rol = "fallo";
            string usuarioIngresado = "";
            while (true)
            {
                usuarioIngresado = Leer("Usuario (0 para volver): ").Trim().ToLower();
                if (usuarioIngresado == "0")
                {
                    return;
                }
                string claveIngresada = Leer("Contrasena: ");

                rol = ValidarLogin(usuarioIngresado, claveIngresada);
                if (rol != "fallo")
                {
                    break;
                }
                Console.WriteLine("Error: usuario o contrasena incorrectos. Intente de nuevo.");
            }

            usuarioActual = usuarioIngresado;
            rolActual = rol;

            if (rolActual == "admin")
            {
                Console.WriteLine("\nBienvenido Administrador.\n");
                Pausa();
                MenuAdmin();
            }
            else
            {
                Console.WriteLine("\nInicio de sesion exitoso. Bienvenido a Finca Grandez.\n");
                Pausa();
                MenuUsuario();
            }
            usuarioActual = "";
            rolActual = "";
        }

        // ==========================================================
        // REALIZAR PEDIDO
        // ==========================================================
        static void RealizarPedido()
        {
            // Carrito: cantidad pedida de cada producto (posicion = id del producto - 1)
            int totalProductos = diccionarioInventario.Count;
            int[] carrito = new int[totalProductos];

            string otroProducto = "S";
            while (otroProducto == "S")
            {
                MostrarCatalogo();

                // Elegir producto
                int productoElegido = 0;
                while (true)
                {
                    string entrada = Leer($"Elija el numero de producto (1-{totalProductos}): ");
                    if (!int.TryParse(entrada, out productoElegido))
                    {
                        Console.WriteLine("Error: ingrese solo el numero del producto.");
                    }
                    else if (diccionarioInventario.ContainsKey(productoElegido))
                    {
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Error: opcion de producto invalida.");
                    }
                }

                int stockActual = ObtenerStock(productoElegido);

                // Si no hay stock, no se puede pedir ese producto.
                if (stockActual == 0)
                {
                    Console.WriteLine("Error: ese producto no tiene stock disponible.");
                }
                else
                {
                    // Elegir cantidad
                    int cantidad = 0;
                    while (true)
                    {
                        string entrada = Leer("Ingrese la cantidad a comprar (Paquetes): ");
                        if (!int.TryParse(entrada, out cantidad))
                        {
                            Console.WriteLine("Error: ingrese solo numeros.");
                        }
                        else if (cantidad <= 0)
                        {
                            Console.WriteLine("Error: la cantidad debe ser mayor a 0.");
                        }
                        else if (cantidad > stockActual)
                        {
                            Console.WriteLine($"Error: debe ingresar una cantidad menor o igual al stock ({stockActual} und.).");
                        }
                        else
                        {
                            break;
                        }
                    }

                    // Agregar al carrito y descontar del stock
                    carrito[productoElegido - 1] = carrito[productoElegido - 1] + cantidad;
                    CambiarStock(productoElegido, -cantidad);
                    Console.WriteLine("Producto agregado al carrito.");
                }

                // Preguntar si desea seguir comprando
                while (true)
                {
                    otroProducto = Leer("¿Desea anadir otro producto? (S = Si / N = Continuar con la compra): ").Trim().ToUpper();
                    if (otroProducto == "S" || otroProducto == "N")
                    {
                        break;
                    }
                    Console.WriteLine("Error: responda S o N.");
                }
            }

            // Calcular el subtotal, las unidades y el resumen de productos
            double subtotal = 0;
            int totalUnidades = 0;
            string resumenProductos = "";
            for (int i = 0; i < carrito.Length; i++)
            {
                if (carrito[i] > 0)
                {
                    subtotal = subtotal + ObtenerPrecio(i + 1) * carrito[i];
                    totalUnidades = totalUnidades + carrito[i];
                    if (resumenProductos != "")
                    {
                        resumenProductos = resumenProductos + " + ";
                    }
                    resumenProductos = resumenProductos + ObtenerNombre(i + 1) + " x" + carrito[i];
                }
            }

            // Si no agrego ningun producto, no hay pedido
            if (totalUnidades == 0)
            {
                Console.WriteLine("No se agrego ningun producto al carrito.");
                return;
            }

            Console.WriteLine("\nCarrito confirmado. Procediendo al registro del cliente...");

            // ---- 2. DATOS DEL CLIENTE ----
            Console.WriteLine("\n--- 2. DATOS DEL CLIENTE ---");
            string nombreCliente = "";
            while (true)
            {
                nombreCliente = Leer("Nombre completo: ").Trim();
                if (nombreCliente.Length >= 3 && !TieneNumero(nombreCliente))
                {
                    break;
                }
                Console.WriteLine("Error: el nombre debe tener al menos 3 caracteres y no contener numeros.");
            }
            string telefonoCliente = PedirCelular("Telefono celular: ");

            // ---- 3. METODO DE ENTREGA ----
            Console.WriteLine("\n--- 3. METODO DE ENTREGA ---");
            Console.WriteLine("1. Delivery (solo Lima - S/ 15.00)");
            Console.WriteLine("2. Recojo en Tienda (Gratis)");
            string tipoEntrega = "";
            while (true)
            {
                tipoEntrega = Leer("Seleccione una opcion (1-2): ");
                if (tipoEntrega == "1" || tipoEntrega == "2")
                {
                    break;
                }
                Console.WriteLine("Error: opcion invalida.");
            }

            string direccionCliente = "";
            string referenciaCliente = "";
            string distritoCliente = "";
            string telContacto = "";
            string diasEntregas = "";
            string textoEntrega = "";
            double costoDelivery = 0;

            if (tipoEntrega == "1")
            {
                // Delivery
                while (true)
                {
                    direccionCliente = Leer("Direccion exacta de entrega: ").Trim();
                    if (direccionCliente.Length >= 7)
                    {
                        break;
                    }
                    Console.WriteLine("Error: ingrese una direccion valida (minimo 7 caracteres).");
                }

                while (true)
                {
                    referenciaCliente = Leer("Referencia (ej. frente al parque, porton negro): ").Trim();
                    if (referenciaCliente.Length >= 5)
                    {
                        break;
                    }
                    Console.WriteLine("Error: ingrese una referencia valida (minimo 5 caracteres).");
                }

                distritoCliente = ElegirDistrito();
                costoDelivery = COSTO_DELIVERY;
                diasEntregas = DIAS_ENTREGA;
                Console.WriteLine($"Distrito elegido: {distritoCliente} (costo de envio: S/ {costoDelivery:F2})");
                Console.WriteLine($"Tiempo estimado de entrega: {diasEntregas}");
                telContacto = PedirCelular("Telefono de contacto para la entrega: ");
                textoEntrega = "Delivery";
            }
            else
            {
                // Recojo en tienda
                direccionCliente = "C. Jose Antonio Encinas 184";
                distritoCliente = "Pueblo Libre 15084";
                costoDelivery = 0.00;
                referenciaCliente = "-";
                telContacto = "-";
                diasEntregas = "-";
                textoEntrega = "Recojo en tienda";
            }

            double totalPagar = subtotal + costoDelivery;
            Console.WriteLine($"Costo de envio calculado: S/ {costoDelivery:F2}");

            // ---- 4. METODO DE PAGO (YAPE) ----
            Console.WriteLine("\n--- 4. METODO DE PAGO ---");
            Console.WriteLine("Unico metodo disponible: YAPE");
            Console.WriteLine($"Realice el pago de S/ {totalPagar:F2} al siguiente numero:");
            Console.WriteLine("*** 987 654 321 - Finca Grandez ***\n");

            while (true)
            {
                string confirmacionPago = Leer("¿Ya realizo el pago por Yape? (S = Si / N = No): ").Trim().ToUpper();
                if (confirmacionPago == "S")
                {
                    break;
                }
                else if (confirmacionPago == "N")
                {
                    Console.WriteLine("Por favor realice el pago antes de continuar.");
                }
                else
                {
                    Console.WriteLine("Error: responda S o N.");
                }
            }

            string codigoOperacionYape = "";
            while (true)
            {
                codigoOperacionYape = Leer("Ingrese el codigo de operacion de Yape (8 digitos): ");
                if (!SoloDigitos(codigoOperacionYape))
                {
                    Console.WriteLine("Error: el codigo debe contener solo numeros.");
                }
                else if (codigoOperacionYape.Length == 8)
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Error: el codigo debe tener exactamente 8 digitos.");
                }
            }

            // ---- 5. TIPO DE COMPROBANTE ----
            string tipoComprobante = "";
            string numDocumento = "";
            string razonSocial = "";
            while (true)
            {
                Console.WriteLine("\n--- 5. TIPO DE COMPROBANTE ---");
                Console.WriteLine("1. Boleta (requiere DNI)");
                Console.WriteLine("2. Factura (requiere RUC y Razon Social)");
                while (true)
                {
                    tipoComprobante = Leer("Elija el tipo de comprobante (1-2): ");
                    if (tipoComprobante == "1" || tipoComprobante == "2")
                    {
                        break;
                    }
                    Console.WriteLine("Error: opcion invalida.");
                }

                int longitud;
                string documento;
                if (tipoComprobante == "1")
                {
                    longitud = 8;
                    documento = "DNI";
                }
                else
                {
                    longitud = 11;
                    documento = "RUC";
                }

                bool volver = false;
                while (true)
                {
                    numDocumento = Leer($"Ingrese su {documento} ({longitud} digitos) o 0 para volver: ").Trim();
                    if (numDocumento == "0")
                    {
                        volver = true;
                        break;
                    }
                    if (!SoloDigitos(numDocumento))
                    {
                        Console.WriteLine($"Error: el {documento} debe contener solo numeros.");
                    }
                    else if (numDocumento.Length == longitud)
                    {
                        break;
                    }
                    else
                    {
                        Console.WriteLine($"Error: el {documento} debe tener exactamente {longitud} digitos.");
                    }
                }

                // Si es factura, pedir la razon social (si no cancelo antes)
                if (tipoComprobante == "2" && !volver)
                {
                    while (true)
                    {
                        razonSocial = Leer("Ingrese la Razon Social (o 0 para volver): ").Trim();
                        if (razonSocial == "0")
                        {
                            volver = true;
                            break;
                        }
                        else if (razonSocial.Length >= 3)
                        {
                            break;
                        }
                        Console.WriteLine("Error: la Razon Social debe tener al menos 3 caracteres.");
                    }
                }

                if (!volver)
                {
                    break;
                }
            }

            // ---- GENERAR Y GUARDAR PEDIDO EN EL DICCIONARIO ----
            string idPedido = GenerarIdPedido();
            DateTime ahora = DateTime.Now;
            string estado;
            if (textoEntrega == "Delivery")
            {
                estado = "Pagado - Pendiente de entrega";
            }
            else
            {
                estado = "Pagado - Listo para recojo";
            }

            diccionarioPedidos.Add(idPedido, new string[]
            {
                ahora.ToString("yyyy-MM-dd"),          // [0] Fecha
                ahora.ToString("HH:mm"),               // [1] Hora
                usuarioActual,                         // [2] Usuario
                nombreCliente,                         // [3] Cliente
                resumenProductos,                      // [4] Producto
                totalUnidades.ToString(),              // [5] Cantidad
                textoEntrega,                          // [6] Entrega
                direccionCliente + ", " + distritoCliente, // [7] Direccion
                referenciaCliente,                     // [8] Referencia
                telContacto,                           // [9] Tel. contacto
                diasEntregas,                          // [10] Dias
                totalPagar.ToString("0.00"),  // [11] Total
                estado                                 // [12] Estado
            });
            siguienteIdPedido++;

            // ---- SUMAR LAS VENTAS POR TIPO DE CAFE (para el reporte) ----
            for (int i = 0; i < carrito.Length; i++)
            {
                if (carrito[i] > 0)
                {
                    int tipo;
                    if (i <= 2)
                    {
                        tipo = 0;
                    }
                    else if (i <= 5)
                    {
                        tipo = 1;
                    }
                    else if (i <= 7)
                    {
                        tipo = 2;
                    }
                    else
                    {
                        tipo = 3;
                    }
                    unidadesCafe[tipo] = unidadesCafe[tipo] + carrito[i];
                    montosCafe[tipo] = montosCafe[tipo] + ObtenerPrecio(i + 1) * carrito[i];
                }
            }

            // ---- BOLETA / FACTURA FINAL ----
            MostrarBoleta(tipoComprobante, idPedido, numDocumento, razonSocial, nombreCliente,
                telefonoCliente, direccionCliente, distritoCliente, textoEntrega,
                referenciaCliente, telContacto, diasEntregas, carrito,
                subtotal, costoDelivery, totalPagar, codigoOperacionYape);
        }


        // =================================================================================
        // 3. MENUS Y CONTROLADORES
        // =================================================================================

        // MENU DEL CLIENTE (rol "user")
        static void MenuUsuario()
        {
            while (true)
            {
                LimpiarPantalla();
                Console.WriteLine("============= FINCA GRANDEZ - CLIENTE =============");
                Console.WriteLine("Sesion: " + usuarioActual);
                Console.WriteLine("1. Ver Catalogo de Productos y Stock");
                Console.WriteLine("2. Realizar un Pedido");
                Console.WriteLine("3. Mis Pedidos");
                Console.WriteLine("4. Buscar un Pedido");
                Console.WriteLine("5. Cerrar sesion");

                int opc;
                if (!int.TryParse(Leer("Ingresar opcion: "), out opc))
                {
                    Console.WriteLine("Ingresar un valor numerico.");
                    Pausa();
                    continue;
                }

                switch (opc)
                {
                    case 1:
                        MostrarCatalogo();
                        string deseaPedido = "";
                        while (true)
                        {
                            deseaPedido = Leer("¿Desea realizar un pedido? (S = Si / N = No): ").Trim().ToUpper();
                            if (deseaPedido == "S" || deseaPedido == "N")
                            {
                                break;
                            }
                            Console.WriteLine("Error: responda S o N.");
                        }
                        if (deseaPedido == "S")
                        {
                            RealizarPedido();
                        }
                        break;
                    case 2:
                        RealizarPedido();
                        break;
                    case 3:
                        MostrarMisPedidos();
                        break;
                    case 4:
                        BuscarPedido();
                        break;
                    case 5:
                        Console.WriteLine("\nCerrando sesion...\n");
                        return;
                    default:
                        Console.WriteLine("Opcion no valida, intente de nuevo.\n");
                        break;
                }
                Pausa();
            }
        }

        // MENU DEL ADMINISTRADOR (rol "admin")
        static void MenuAdmin()
        {
            while (true)
            {
                LimpiarPantalla();
                Console.WriteLine("============= FINCA GRANDEZ - ADMINISTRADOR =============");
                Console.WriteLine("Sesion: " + usuarioActual);
                Console.WriteLine("1. Inventario y reposicion de stock");
                Console.WriteLine("2. Completar pedidos pendientes");
                Console.WriteLine("3. Ver todos los pedidos");
                Console.WriteLine("4. Buscar un pedido");
                Console.WriteLine("5. Reporte diario");
                Console.WriteLine("6. Productos mas vendidos");
                Console.WriteLine("7. Cerrar sesion");

                int opc;
                if (!int.TryParse(Leer("Ingresar opcion: "), out opc))
                {
                    Console.WriteLine("Ingresar un valor numerico.");
                    Pausa();
                    continue;
                }

                switch (opc)
                {
                    case 1:
                        ReponerStock();
                        break;
                    case 2:
                        CompletarPedidos();
                        break;
                    case 3:
                        MostrarTodosLosPedidos();
                        break;
                    case 4:
                        BuscarPedido();
                        break;
                    case 5:
                        ReporteDiario();
                        break;
                    case 6:
                        ProductosMasVendidos();
                        break;
                    case 7:
                        Console.WriteLine("\nCerrando sesion...\n");
                        return;
                    default:
                        Console.WriteLine("Opcion no valida, intente de nuevo.\n");
                        break;
                }
                Pausa();
            }
        }

        static void Main(string[] args)
        {
            CargarPedidosArchivo();
            while (true)
            {
                LimpiarPantalla();
                Console.WriteLine("=========================================================");
                Console.WriteLine("      SISTEMA DE PEDIDOS - FINCA GRANDEZ");
                Console.WriteLine("=========================================================");
                Console.WriteLine("1. Crear cuenta");
                Console.WriteLine("2. Iniciar sesion");
                Console.WriteLine("3. Salir");
                string opcionInicio = Leer("Ingrese una opcion (1-3): ").Trim();

                switch (opcionInicio)
                {
                    case "1":
                        CrearCuenta();
                        break;
                    case "2":
                        IniciarSesion();
                        break;
                    case "3":
                        Console.WriteLine("\nSaliendo del sistema...");
                        return;
                    default:
                        Console.WriteLine("Opcion no valida, intente de nuevo.\n");
                        break;
                }

                Leer("\nPresione ENTER para continuar...");
            }
        }
    }
}