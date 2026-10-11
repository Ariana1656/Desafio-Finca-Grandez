using System;
using System.Collections.Generic;

namespace PrototipoWebFincaGrandez
{
    internal class Program
    {
        // STOCK INICIAL
        static int[] stock = { 20, 10, 5, 15, 10, 5, 10, 5, 10 };

        // PRODUCTOS
        static string[] nombresProductos =
        {
            "Blend Premium (250g)", "Blend Premium (500g)", "Blend Premium (1kg)",
            "Cafe Tradicion (250g)", "Cafe Tradicion (500g)", "Cafe Tradicion (1kg)",
            "Cafe Catuai (250g)", "Cafe Catuai (500g)", "Cafe Geisha (250g)"
        };
        static double[] precios = { 32.00, 52.00, 72.00, 28.00, 48.00, 68.00, 35.00, 65.00, 45.00 };
        static string[] presentaciones = { "(250g)", "(500g)", "(1kg)", "(250g)", "(500g)", "(1kg)", "(250g)", "(500g)", "(250g)" };

        // DELIVERY
        const double COSTO_DELIVERY = 15.00;
        const string DIAS_ENTREGA = "1 a 3 dias";

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

        // PEDIDOS: LISTAS PARALELAS
        static List<string> pedidosId = new List<string>();
        static List<string> pedidosFecha = new List<string>();
        static List<string> pedidosHora = new List<string>();
        static List<string> pedidosCliente = new List<string>();
        static List<string> pedidosProducto = new List<string>();
        static List<int> pedidosCantidad = new List<int>();
        static List<string> pedidosEntrega = new List<string>();
        static List<string> pedidosDireccion = new List<string>();
        static List<string> pedidosReferencia = new List<string>();
        static List<string> pedidosTelContacto = new List<string>();
        static List<string> pedidosDias = new List<string>();
        static List<double> pedidosTotal = new List<double>();
        static List<string> pedidosEstado = new List<string>();

        // CUENTA Y SESION
        static bool cuentaCreada = false;
        static bool sesionIniciada = false;
        static List<string> listaCorreos = new List<string>();
        static List<string> listaUsuarios = new List<string>();
        static string usuarioRegistrado = "";
        static string claveRegistrada = "";

        // ==========================================================
        // FUNCIONES BASICAS
        // ==========================================================
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

        static int SetStock(int stockActual, int nuevoStock)
        {
            stockActual = nuevoStock;
            return stockActual;
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
            int numero = pedidosId.Count + 1;
            return "PED-" + numero.ToString("D4");
        }

        static int ObtenerStock(int numero)
        {
            return stock[numero - 1];
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
            for (int numero = 1; numero <= nombresProductos.Length; numero++)
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
                Console.WriteLine($"   [{numero}]  {presentaciones[numero - 1]}\t\tS/ {precios[numero - 1]:F2}\t\t{ObtenerStock(numero)} und.");
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
            bool encontrado = false;
            for (int i = 0; i < pedidosId.Count; i++)
            {
                if (pedidosId[i] == idBuscado)
                {
                    encontrado = true;
                    Console.WriteLine("\nPedido encontrado");
                    Console.WriteLine($"Estado:     {pedidosEstado[i]}");
                    Console.WriteLine("Detalle:");
                    Console.WriteLine($"  ID:        {pedidosId[i]}");
                    Console.WriteLine($"  Fecha:     {pedidosFecha[i]} {pedidosHora[i]}");
                    Console.WriteLine($"  Cliente:   {pedidosCliente[i]}");
                    Console.WriteLine($"  Producto:  {pedidosProducto[i]} x {pedidosCantidad[i]}");
                    Console.WriteLine($"  Entrega:   {pedidosEntrega[i]} - {pedidosDireccion[i]}");
                    if (pedidosEntrega[i] == "Delivery")
                    {
                        Console.WriteLine($"  Referencia:       {pedidosReferencia[i]}");
                        Console.WriteLine($"  Tel. contacto:    {pedidosTelContacto[i]}");
                        Console.WriteLine($"  Entrega estimada: {pedidosDias[i]}");
                    }
                    Console.WriteLine($"  Total:     S/ {pedidosTotal[i]:F2}\n");
                }
            }
            if (!encontrado)
            {
                Console.WriteLine("El pedido no existe.\n");
            }
        }

        // TODOS LOS PEDIDOS ORDENADOS POR TOTAL (BURBUJA)
        static void MostrarTodosLosPedidos()
        {
            Console.WriteLine("\n--- TODOS LOS PEDIDOS ---");

            int cantidad = pedidosId.Count;
            if (cantidad == 0)
            {
                Console.WriteLine("No hay pedidos registrados.");
                return;
            }

            // Cada posicion guarda el numero de un pedido de las listas originales.
            int[] orden = new int[cantidad];
            for (int i = 0; i < cantidad; i++)
            {
                orden[i] = i;
            }

            // Burbuja: se ordenan los indices de mayor a menor total.
            for (int i = 0; i < cantidad - 1; i++)
            {
                for (int j = 0; j < cantidad - 1 - i; j++)
                {
                    if (pedidosTotal[orden[j]] < pedidosTotal[orden[j + 1]])
                    {
                        int auxiliar = orden[j];
                        orden[j] = orden[j + 1];
                        orden[j + 1] = auxiliar;
                    }
                }
            }

            for (int puesto = 0; puesto < cantidad; puesto++)
            {
                int i = orden[puesto];
                Console.WriteLine("---------------------------------------------------------");
                Console.WriteLine($"Puesto:     {puesto + 1}");
                Console.WriteLine($"ID:         {pedidosId[i]}");
                Console.WriteLine($"Fecha:      {pedidosFecha[i]} {pedidosHora[i]}");
                Console.WriteLine($"Cliente:    {pedidosCliente[i]}");
                Console.WriteLine($"Producto:   {pedidosProducto[i]}");
                Console.WriteLine($"Cantidad:   {pedidosCantidad[i]}");
                Console.WriteLine($"Entrega:    {pedidosEntrega[i]}");
                Console.WriteLine($"Direccion:  {pedidosDireccion[i]}");
                if (pedidosEntrega[i] == "Delivery")
                {
                    Console.WriteLine($"Referencia: {pedidosReferencia[i]}");
                    Console.WriteLine($"Contacto:   {pedidosTelContacto[i]}");
                    Console.WriteLine($"Plazo:      {pedidosDias[i]}");
                }
                Console.WriteLine($"Estado:     {pedidosEstado[i]}");
                Console.WriteLine($"Total:      S/ {pedidosTotal[i]:F2}");
            }
            Console.WriteLine("---------------------------------------------------------");
            Console.WriteLine($"Total de pedidos registrados: {cantidad}");
        }

        // ==========================================================
        // REPORTES
        // ==========================================================
        static void ReporteDiario()
        {
            Console.WriteLine("\n--- REPORTE DIARIO DE PEDIDOS ---");
            string hoy = DateTime.Now.ToString("yyyy-MM-dd");
            int cantidadPedidos = 0;
            double totalVendido = 0;
            int totalDelivery = 0;
            int totalRecojo = 0;
            Console.WriteLine("---------------------------------------------------------");
            for (int i = 0; i < pedidosId.Count; i++)
            {
                // Solo cuentan los pedidos de hoy.
                if (pedidosFecha[i] == hoy)
                {
                    cantidadPedidos++;
                    totalVendido = totalVendido + pedidosTotal[i];
                    if (pedidosEntrega[i] == "Delivery")
                    {
                        totalDelivery++;
                    }
                    else
                    {
                        totalRecojo++;
                    }
                    Console.WriteLine($"{pedidosId[i]} | {pedidosHora[i]} | {pedidosCliente[i]} | {pedidosProducto[i]} x {pedidosCantidad[i]} | S/ {pedidosTotal[i]:F2}");
                }
            }
            if (cantidadPedidos == 0)
            {
                Console.WriteLine("No hay pedidos registrados hoy.");
            }
            Console.WriteLine("---------------------------------------------------------");
            Console.WriteLine($"Pedidos del dia:   {cantidadPedidos}");
            Console.WriteLine($"  - Delivery:      {totalDelivery}");
            Console.WriteLine($"  - Recojo:        {totalRecojo}");
            Console.WriteLine($"Total vendido:     S/ {totalVendido:F2}\n");
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

            // Sumar las ventas de todas las presentaciones por tipo de cafe.
            for (int i = 0; i < pedidosProducto.Count; i++)
            {
                int indiceProducto = -1;
                for (int k = 0; k < nombresProductos.Length; k++)
                {
                    if (nombresProductos[k] == pedidosProducto[i])
                    {
                        indiceProducto = k;
                    }
                }

                if (indiceProducto != -1)
                {
                    int tipo;
                    if (indiceProducto <= 2)
                    {
                        tipo = 0;
                    }
                    else if (indiceProducto <= 5)
                    {
                        tipo = 1;
                    }
                    else if (indiceProducto <= 7)
                    {
                        tipo = 2;
                    }
                    else
                    {
                        tipo = 3;
                    }

                    unidades[tipo] = unidades[tipo] + pedidosCantidad[i];
                    ventas[tipo] = ventas[tipo] + precios[indiceProducto] * pedidosCantidad[i];
                }
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
            string nombreProducto, int cantidad, double precioProducto,
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
            Console.WriteLine($"{nombreProducto,-30}{cantidad,6}{precioProducto,10:F2}{subtotal,10:F2}");
            Console.WriteLine("--------------------------------------------------------");

            // ---- TOTALES ----
            double igv = totalPagar - totalPagar / 1.18;
            string textoItems = "Items: 1";
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
                else if (listaCorreos.Contains(correoInput))
                {
                    Console.WriteLine("Error: ese correo ya esta registrado. Use otro.");
                }
                else
                {
                    correoNuevo = correoInput;
                    break;
                }
            }
            listaCorreos.Add(correoNuevo);

            // Usuario
            string usuarioNuevo = "";
            while (true)
            {
                usuarioNuevo = Leer("Nombre de usuario : ").Trim().ToLower();
                if (usuarioNuevo.Length < 3)
                {
                    Console.WriteLine("Error: el usuario debe tener al menos 3 caracteres.");
                }
                else if (listaUsuarios.Contains(usuarioNuevo))
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

            listaUsuarios.Add(usuarioNuevo);
            usuarioRegistrado = usuarioNuevo;
            claveRegistrada = claveConfirmacion;
            cuentaCreada = true;
            Console.WriteLine("\n*Cuenta creada exitosamente! Ahora puede iniciar sesion.\n");
        }

        static void IniciarSesion()
        {
            if (!cuentaCreada)
            {
                Console.WriteLine("Error: primero debe crear una cuenta (opcion 1).\n");
                return;
            }

            while (true)
            {
                string usuarioIngresado = Leer("Usuario: ").Trim().ToLower();
                if (usuarioIngresado == usuarioRegistrado)
                {
                    break;
                }
                Console.WriteLine("Error: usuario incorrecto. Intente de nuevo.");
            }

            while (true)
            {
                string claveIngresada = Leer("Contrasena: ");
                if (claveIngresada == claveRegistrada)
                {
                    break;
                }
                Console.WriteLine("Error: contrasena incorrecta. Intente de nuevo.");
            }

            Console.WriteLine("\nInicio de sesion exitoso.\n");
            sesionIniciada = true;
            MenuInterno();
        }

        // ==========================================================
        // REALIZAR PEDIDO
        // ==========================================================
        static void RealizarPedido()
        {
            MostrarCatalogo();

            // Elegir producto
            int productoElegido = 0;
            while (true)
            {
                string entrada = Leer($"Elija el numero de producto (1-{nombresProductos.Length}): ");
                if (!int.TryParse(entrada, out productoElegido))
                {
                    Console.WriteLine("Error: ingrese solo el numero del producto.");
                }
                else if (productoElegido >= 1 && productoElegido <= nombresProductos.Length)
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Error: opcion de producto invalida.");
                }
            }

            double precioProducto = precios[productoElegido - 1];
            string nombreProducto = nombresProductos[productoElegido - 1];
            int stockActual = ObtenerStock(productoElegido);

            // Si no hay stock, no se puede pedir ese producto.
            if (stockActual == 0)
            {
                Console.WriteLine("Error: ese producto no tiene stock disponible.");
                return;
            }

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

            Console.WriteLine("\nStock confirmado. Procediendo al registro del cliente...");
            double subtotal = precioProducto * cantidad;

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

            // ---- DESCONTAR STOCK ----
            stock[productoElegido - 1] = SetStock(stock[productoElegido - 1], stock[productoElegido - 1] - cantidad);

            // ---- GENERAR Y GUARDAR PEDIDO ----
            string idPedido = GenerarIdPedido();
            pedidosId.Add(idPedido);
            pedidosCliente.Add(nombreCliente);
            pedidosProducto.Add(nombreProducto);
            pedidosCantidad.Add(cantidad);
            pedidosEntrega.Add(textoEntrega);
            pedidosDireccion.Add(direccionCliente + ", " + distritoCliente);
            pedidosReferencia.Add(referenciaCliente);
            pedidosTelContacto.Add(telContacto);
            pedidosDias.Add(diasEntregas);
            pedidosTotal.Add(totalPagar);
            DateTime ahora = DateTime.Now;
            pedidosFecha.Add(ahora.ToString("yyyy-MM-dd"));
            pedidosHora.Add(ahora.ToString("HH:mm"));
            if (textoEntrega == "Delivery")
            {
                pedidosEstado.Add("Pagado - Pendiente de entrega");
            }
            else
            {
                pedidosEstado.Add("Pagado - Listo para recojo");
            }

            // ---- BOLETA / FACTURA FINAL ----
            MostrarBoleta(tipoComprobante, idPedido, numDocumento, razonSocial, nombreCliente,
                telefonoCliente, direccionCliente, distritoCliente, textoEntrega,
                referenciaCliente, telContacto, diasEntregas, nombreProducto,
                cantidad, precioProducto, subtotal, costoDelivery, totalPagar, codigoOperacionYape);
        }

        // ==========================================================
        // MENUS
        // ==========================================================
        static void MenuInterno()
        {
            while (sesionIniciada)
            {
                LimpiarPantalla();
                Console.WriteLine("============= FINCA GRANDEZ =============");
                Console.WriteLine("1. Ver Catalogo de Productos y Stock");
                Console.WriteLine("2. Realizar un Pedido");
                Console.WriteLine("3. Ver todos los pedidos");
                Console.WriteLine("4. Buscar un Pedido");
                Console.WriteLine("5. Reporte Diario de Pedidos");
                Console.WriteLine("6. Productos Mas Vendidos");
                Console.WriteLine("7. Cerrar sesion");
                string opcionMenu = Leer("Ingrese una opcion (1-7): ");

                if (opcionMenu == "1")
                {
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
                }
                else if (opcionMenu == "2")
                {
                    RealizarPedido();
                }
                else if (opcionMenu == "3")
                {
                    MostrarTodosLosPedidos();
                }
                else if (opcionMenu == "4")
                {
                    BuscarPedido();
                }
                else if (opcionMenu == "5")
                {
                    ReporteDiario();
                }
                else if (opcionMenu == "6")
                {
                    ProductosMasVendidos();
                }
                else if (opcionMenu == "7")
                {
                    Console.WriteLine("\nCerrando sesion...\n");
                    sesionIniciada = false;
                }
                else
                {
                    Console.WriteLine("Opcion no valida, intente de nuevo.\n");
                }

                if (sesionIniciada)
                {
                    Leer("\nPresione ENTER para volver al menu...");
                }
            }
        }

        static void Main(string[] args)
        {
            while (true)
            {
                LimpiarPantalla();
                Console.WriteLine("=========================================================");
                Console.WriteLine("      SISTEMA DE PEDIDOS - FINCA GRANDEZ");
                Console.WriteLine("=========================================================");
                Console.WriteLine("1. Crear cuenta");
                Console.WriteLine("2. Iniciar sesion");
                Console.WriteLine("3. Salir");
                string opcionInicio = Leer("Ingrese una opcion (1-3): ");

                if (opcionInicio == "1")
                {
                    CrearCuenta();
                }
                else if (opcionInicio == "2")
                {
                    IniciarSesion();
                }
                else if (opcionInicio == "3")
                {
                    Console.WriteLine("\nSaliendo del sistema...");
                    break;
                }
                else
                {
                    Console.WriteLine("Opcion no valida, intente de nuevo.\n");
                }

                Leer("\nPresione ENTER para continuar...");
            }
        }
    }
}