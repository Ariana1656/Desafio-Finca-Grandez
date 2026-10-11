using System;
using System.Collections.Generic;
using System.Text;

class Program
{
    // ==========================================================
    // STOCK INICIAL (posiciones 0 a 8 = productos 1 a 9)
    // ==========================================================
    static int[] stock = { 20, 10, 5, 15, 10, 5, 10, 5, 10 };

    // ==========================================================
    // PRODUCTOS
    // ==========================================================
    static string[] productosNombre =
    {
        "Blend Premium (250g)",
        "Blend Premium (500g)",
        "Blend Premium (1kg)",
        "Cafe Tradicion (250g)",
        "Cafe Tradicion (500g)",
        "Cafe Tradicion (1kg)",
        "Cafe Catuai (250g)",
        "Cafe Catuai (500g)",
        "Cafe Geisha (250g)"
    };

    static double[] productosPrecio = { 32.00, 52.00, 72.00, 28.00, 48.00, 68.00, 35.00, 65.00, 45.00 };

    static string[] presentaciones =
    {
        "(250g)", "(500g)", "(1kg)",
        "(250g)", "(500g)", "(1kg)",
        "(250g)", "(500g)",
        "(250g)"
    };

    // ==========================================================
    // DELIVERY
    // ==========================================================
    const double COSTO_DELIVERY = 15.00;
    const string DIAS_ENTREGA = "1 a 3 dias";

    static string[] distritosLima =
    {
        "Cercado de Lima", "Brena", "La Victoria", "Rimac", "San Luis", "Lince",
        "Jesus Maria", "Pueblo Libre", "Magdalena del Mar", "San Miguel",
        "San Isidro", "Miraflores", "Surquillo", "Barranco", "San Borja",
        "Santiago de Surco", "Chorrillos", "La Molina", "Los Olivos",
        "San Martin de Porres", "Independencia", "Comas", "Carabayllo",
        "Puente Piedra", "Ancon", "Santa Anita", "Ate", "El Agustino",
        "San Juan de Lurigancho", "Lurigancho", "Chaclacayo",
        "San Juan de Miraflores", "Villa El Salvador", "Villa Maria del Triunfo"
    };

    // ==========================================================
    // PEDIDOS (listas porque crecen con cada pedido)
    // ==========================================================
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

    // ==========================================================
    // DATOS DEL PEDIDO ACTUAL (los usa mostrarBoleta)
    // ==========================================================
    static string tipoComprobante = "1";
    static string idPedido = "";
    static string numDocumento = "";
    static string nombreCliente = "";
    static string telefonoCliente = "";
    static string direccionCliente = "";
    static string distritoCliente = "";
    static string textoEntrega = "";
    static string referenciaCliente = "";
    static string telContacto = "";
    static string diasEntregas = "";
    static string nombreProducto = "";
    static int cantidad = 0;
    static double precioProducto = 0;
    static double subtotal = 0;
    static double costoDelivery = 0;
    static double totalPagar = 0;
    static string codigoOperacionYape = "";

    // ==========================================================
    // FUNCIONES
    // ==========================================================
    static void limpiarPantalla()
    {
        for (int i = 0; i < 20; i++)
        {
            Console.WriteLine();
        }
    }

    static int setStock(int stockActual, int nuevoStock)
    {
        stockActual = nuevoStock;
        return stockActual;
    }

    static string pedirCelular(string mensaje)
    {
        while (true)
        {
            Console.Write(mensaje);
            string telefono = Console.ReadLine();
            long numero;
            if (long.TryParse(telefono, out numero))
            {
                if (telefono.Length == 9 && telefono[0] == '9')
                {
                    return telefono;
                }
                Console.WriteLine("Error: el celular debe tener 9 digitos y empezar con 9.");
            }
            else
            {
                Console.WriteLine("Error: el celular debe contener solo numeros.");
            }
        }
    }

    static string generarIdPedido()
    {
        int numero = pedidosId.Count + 1;
        if (numero < 10)
        {
            return $"PED-000{numero}";
        }
        else if (numero < 100)
        {
            return $"PED-00{numero}";
        }
        else if (numero < 1000)
        {
            return $"PED-0{numero}";
        }
        else
        {
            return $"PED-{numero}";
        }
    }

    static int obtenerStock(int numero)
    {
        return stock[numero - 1];
    }

    static void mostrarCatalogo()
    {
        Console.WriteLine("\n=========================================================");
        Console.WriteLine("           CATALOGO FINCA GRANDEZ Y STOCK");
        Console.WriteLine("=========================================================");
        Console.WriteLine("   ID    Presentacion    Precio         Stock");
        for (int numero = 1; numero <= 9; numero++)
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
            string presentacion = presentaciones[numero - 1];
            double precio = productosPrecio[numero - 1];
            int stockMostrar = obtenerStock(numero);
            Console.WriteLine($"   [{numero}]  {presentacion}\t\tS/ {precio:F2}\t\t{stockMostrar} und.");
        }
        Console.WriteLine("=========================================================\n");
    }

    static string elegirDistrito()
    {
        Console.WriteLine("\nDistritos con delivery (solo Lima):");
        for (int i = 0; i < distritosLima.Length; i++)
        {
            Console.WriteLine($"{i + 1}. {distritosLima[i]}");
        }
        while (true)
        {
            Console.Write("Elija su distrito (numero): ");
            int opcion;
            if (int.TryParse(Console.ReadLine(), out opcion))
            {
                if (opcion >= 1 && opcion <= distritosLima.Length)
                {
                    return distritosLima[opcion - 1];
                }
                Console.WriteLine("Error: ingrese un numero de la lista.");
            }
            else
            {
                Console.WriteLine("Error: ingrese solo el numero del distrito.");
            }
        }
    }

    static void buscarPedido()
    {
        Console.WriteLine("\n--- BUSCAR PEDIDO ---");
        Console.Write("Ingrese el ID del pedido (ej. PED-0001): ");
        string idBuscado = Console.ReadLine().Trim().ToUpper();
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

    static void reporteDiario()
    {
        Console.WriteLine("\n--- REPORTE DIARIO DE PEDIDOS ---");
        int cantidadPedidos = 0;
        double totalVendido = 0.0;
        int totalDelivery = 0;
        int totalRecojo = 0;
        Console.WriteLine("---------------------------------------------------------");
        for (int i = 0; i < pedidosId.Count; i++)
        {
            if (pedidosFecha[i] != "")
            {
                cantidadPedidos = cantidadPedidos + 1;
                totalVendido = totalVendido + pedidosTotal[i];
                if (pedidosEntrega[i] == "Delivery")
                {
                    totalDelivery = totalDelivery + 1;
                }
                else
                {
                    totalRecojo = totalRecojo + 1;
                }
                Console.WriteLine(
                    $"{pedidosId[i]} | " +
                    $"{pedidosHora[i]} | " +
                    $"{pedidosCliente[i]} | " +
                    $"{pedidosProducto[i]} x {pedidosCantidad[i]} | " +
                    $"S/ {pedidosTotal[i]:F2}");
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

    static void mostrarBoleta()
    {
        Console.WriteLine("\n=========================================================");
        string tipoDocumento;
        if (tipoComprobante == "1")
        {
            Console.WriteLine("           BOLETA DE VENTA - FINCA GRANDEZ");
            tipoDocumento = "BOLETA";
        }
        else
        {
            Console.WriteLine("              FACTURA - FINCA GRANDEZ");
            tipoDocumento = "FACTURA";
        }
        Console.WriteLine("=========================================================");
        Console.WriteLine($"ID Pedido:      {idPedido}");
        Console.WriteLine("---------------------------------------------------------");
        Console.WriteLine($"Documento:      {tipoDocumento}");
        Console.WriteLine($"Nro. Doc:       {numDocumento}");
        Console.WriteLine($"Cliente:        {nombreCliente}");
        Console.WriteLine($"Telefono:       {telefonoCliente}");
        Console.WriteLine($"Direccion:      {direccionCliente}, {distritoCliente}");
        if (textoEntrega == "Delivery")
        {
            Console.WriteLine($"Referencia:     {referenciaCliente}");
            Console.WriteLine($"Tel. contacto:  {telContacto}");
            Console.WriteLine($"Entrega en:     {diasEntregas}");
        }
        else
        {
            Console.WriteLine("Entrega:        Recojo en tienda");
        }
        Console.WriteLine("---------------------------------------------------------");
        Console.WriteLine($"Producto:       {nombreProducto}");
        Console.WriteLine($"Cantidad:       {cantidad}");
        Console.WriteLine($"Precio unit.:   S/ {precioProducto:F2}");
        Console.WriteLine("---------------------------------------------------------");
        Console.WriteLine($"SUBTOTAL PEDIDO:  S/ {subtotal:F2}");
        Console.WriteLine($"COSTO ENVIO:      S/ {costoDelivery:F2}");
        Console.WriteLine("---------------------------------------------------------");
        Console.WriteLine($"TOTAL A PAGAR:    S/ {totalPagar:F2}");
        Console.WriteLine("=========================================================");
        Console.WriteLine("Metodo Pago:    YAPE");
        Console.WriteLine("Pagado a:       987 654 321 (Finca Grandez)");
        Console.WriteLine($"Operacion ID:   {codigoOperacionYape}");
        Console.WriteLine("=========================================================");
        Console.WriteLine("       *Pedido registrado correctamente!\n");
    }

    // ==========================================================
    // PROGRAMA PRINCIPAL
    // ==========================================================
    static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;

        bool cuentacreada = false;
        bool sesionIniciada = false;
        List<string> listacorreos = new List<string>();
        List<string> listaUsuarios = new List<string>();
        string usuarioRegistrado = "";
        string claveRegistrada = "";
        long numeroValidar = 0;

        while (true)
        {
            limpiarPantalla();
            Console.WriteLine("=========================================================");
            Console.WriteLine("      SISTEMA DE PEDIDOS - FINCA GRANDEZ");
            Console.WriteLine("=========================================================");
            Console.WriteLine("1. Crear cuenta");
            Console.WriteLine("2. Iniciar sesion");
            Console.WriteLine("3. Salir");
            Console.Write("Ingrese una opcion (1-3): ");
            string opcionInicio = Console.ReadLine();

            // ======================================================
            // CREAR CUENTA
            // ======================================================
            if (opcionInicio == "1")
            {
                Console.WriteLine("\n--- CREACION DE CUENTA ---");

                // CORREO
                string correoNuevo = "";
                while (true)
                {
                    Console.Write("Correo electronico: ");
                    string correoInput = Console.ReadLine().Trim().ToLower();

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
                    else if (listacorreos.Contains(correoInput))
                    {
                        Console.WriteLine("Error: ese correo ya esta registrado. Use otro.");
                    }
                    else
                    {
                        correoNuevo = correoInput;
                        break;
                    }
                }
                listacorreos.Add(correoNuevo);

                // USUARIO
                string usuarioNuevo = "";
                while (true)
                {
                    Console.Write("Nombre de usuario : ");
                    usuarioNuevo = Console.ReadLine().Trim().ToLower();

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

                // CONTRASEÑA
                string claveTemporal = "";
                while (true)
                {
                    Console.Write("Contrasena (minimo 4 caracteres): ");
                    claveTemporal = Console.ReadLine();
                    if (claveTemporal.Length >= 4)
                    {
                        break;
                    }
                    Console.WriteLine("Error: la contrasena debe tener al menos 4 caracteres.");
                }

                // CONFIRMAR CONTRASEÑA
                string claveConfirmacion = "";
                while (true)
                {
                    Console.Write("Confirme su contrasena: ");
                    claveConfirmacion = Console.ReadLine();
                    if (claveConfirmacion != claveTemporal)
                    {
                        Console.WriteLine("Error: las contrasenas no coinciden.");
                    }
                    else
                    {
                        break;
                    }
                }

                listaUsuarios.Add(usuarioNuevo);
                usuarioRegistrado = usuarioNuevo;
                claveRegistrada = claveConfirmacion;
                cuentacreada = true;
                Console.WriteLine("\n*Cuenta creada exitosamente! Ahora puede iniciar sesion.\n");
            }

            // ======================================================
            // INICIAR SESION
            // ======================================================
            else if (opcionInicio == "2")
            {
                if (!cuentacreada)
                {
                    Console.WriteLine("Error: primero debe crear una cuenta (opcion 1).\n");
                }
                else
                {
                    // VALIDAR USUARIO
                    bool usuarioValido = false;
                    while (!usuarioValido)
                    {
                        Console.Write("Usuario: ");
                        string usuarioIngresado = Console.ReadLine().Trim().ToLower();
                        if (usuarioIngresado == usuarioRegistrado)
                        {
                            usuarioValido = true;
                        }
                        else
                        {
                            Console.WriteLine("Error: usuario incorrecto. Intente de nuevo.");
                        }
                    }

                    // VALIDAR CONTRASEÑA
                    bool claveValida = false;
                    while (!claveValida)
                    {
                        Console.Write("Contrasena: ");
                        string claveIngresada = Console.ReadLine();
                        if (claveIngresada == claveRegistrada)
                        {
                            claveValida = true;
                        }
                        else
                        {
                            Console.WriteLine("Error: contrasena incorrecta. Intente de nuevo.");
                        }
                    }

                    Console.WriteLine("\nInicio de sesion exitoso.\n");
                    sesionIniciada = true;

                    // ==================================================
                    // MENU INTERNO
                    // ==================================================
                    while (sesionIniciada)
                    {
                        limpiarPantalla();
                        Console.WriteLine("=========================================================");
                        Console.WriteLine("       BIENVENIDO FINCA GRANDEZ");
                        Console.WriteLine("=========================================================");
                        Console.WriteLine("1. Ver Catalogo de Productos y Stock");
                        Console.WriteLine("2. Realizar un Pedido");
                        Console.WriteLine("3. Buscar un Pedido");
                        Console.WriteLine("4. Reporte Diario de Pedidos");
                        Console.WriteLine("5. Cerrar sesion");
                        Console.Write("Ingrese una opcion (1-5): ");
                        string opcionMenu = Console.ReadLine();

                        // ==============================================
                        // VER CATALOGO
                        // ==============================================
                        if (opcionMenu == "1")
                        {
                            mostrarCatalogo();

                            string deseaPedido = "";
                            while (true)
                            {
                                Console.Write("¿Desea realizar un pedido? (S = Si / N = No): ");
                                deseaPedido = Console.ReadLine().Trim().ToUpper();
                                if (deseaPedido == "S" || deseaPedido == "N")
                                {
                                    break;
                                }
                                Console.WriteLine("Error: responda S o N.");
                            }

                            if (deseaPedido == "S")
                            {
                                opcionMenu = "2";
                            }
                        }

                        // ==============================================
                        // REALIZAR PEDIDO
                        // ==============================================
                        if (opcionMenu == "2")
                        {
                            mostrarCatalogo();

                            int totalProductos = productosNombre.Length;

                            // ------------------------------------------
                            // ELEGIR PRODUCTO
                            // ------------------------------------------
                            int productoElegido = 0;
                            while (true)
                            {
                                Console.Write($"Elija el numero de producto (1-{totalProductos}): ");
                                if (int.TryParse(Console.ReadLine(), out productoElegido))
                                {
                                    if (productoElegido >= 1 && productoElegido <= totalProductos)
                                    {
                                        break;
                                    }
                                    Console.WriteLine("Error: opcion de producto invalida.");
                                }
                                else
                                {
                                    Console.WriteLine("Error: ingrese solo el numero del producto.");
                                }
                            }

                            precioProducto = productosPrecio[productoElegido - 1];
                            nombreProducto = productosNombre[productoElegido - 1];
                            int stockActual = obtenerStock(productoElegido);

                            // ------------------------------------------
                            // ELEGIR CANTIDAD
                            // ------------------------------------------
                            while (true)
                            {
                                Console.Write("Ingrese la cantidad a comprar (Paquetes): ");
                                if (int.TryParse(Console.ReadLine(), out cantidad))
                                {
                                    if (cantidad <= 0)
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
                                else
                                {
                                    Console.WriteLine("Error: ingrese solo numeros.");
                                }
                            }

                            Console.WriteLine("\nStock confirmado. Procediendo al registro del cliente...");

                            subtotal = precioProducto * cantidad;

                            // ==========================================
                            // DATOS DEL CLIENTE
                            // ==========================================
                            Console.WriteLine("\n--- 2. DATOS DEL CLIENTE ---");

                            while (true)
                            {
                                Console.Write("Nombre completo: ");
                                nombreCliente = Console.ReadLine().Trim();

                                bool tieneNumero = false;
                                for (int i = 0; i < nombreCliente.Length; i++)
                                {
                                    if ("0123456789".IndexOf(nombreCliente[i]) >= 0)
                                    {
                                        tieneNumero = true;
                                    }
                                }

                                if (nombreCliente.Length >= 3 && !tieneNumero)
                                {
                                    break;
                                }
                                Console.WriteLine("Error: el nombre debe tener al menos 3 caracteres y no contener numeros.");
                            }

                            telefonoCliente = pedirCelular("Telefono celular: ");

                            // ==========================================
                            // METODO DE ENTREGA
                            // ==========================================
                            Console.WriteLine("\n--- 3. METODO DE ENTREGA ---");
                            Console.WriteLine("1. Delivery (solo Lima - S/ 15.00)");
                            Console.WriteLine("2. Recojo en Tienda (Gratis)");

                            string tipoEntrega = "";
                            while (true)
                            {
                                Console.Write("Seleccione una opcion (1-2): ");
                                tipoEntrega = Console.ReadLine();
                                if (tipoEntrega == "1" || tipoEntrega == "2")
                                {
                                    break;
                                }
                                Console.WriteLine("Error: opcion invalida.");
                            }

                            // DELIVERY
                            if (tipoEntrega == "1")
                            {
                                while (true)
                                {
                                    Console.Write("Direccion exacta de entrega: ");
                                    direccionCliente = Console.ReadLine().Trim();
                                    if (direccionCliente.Length >= 7)
                                    {
                                        break;
                                    }
                                    Console.WriteLine("Error: ingrese una direccion valida (minimo 7 caracteres).");
                                }

                                while (true)
                                {
                                    Console.Write("Referencia (ej. frente al parque, porton negro): ");
                                    referenciaCliente = Console.ReadLine().Trim();
                                    if (referenciaCliente.Length >= 5)
                                    {
                                        break;
                                    }
                                    Console.WriteLine("Error: ingrese una referencia valida (minimo 5 caracteres).");
                                }

                                distritoCliente = elegirDistrito();
                                costoDelivery = COSTO_DELIVERY;
                                diasEntregas = DIAS_ENTREGA;

                                Console.WriteLine($"Distrito elegido: {distritoCliente} (costo de envio: S/ {costoDelivery:F2})");
                                Console.WriteLine($"Tiempo estimado de entrega: {diasEntregas}");

                                telContacto = pedirCelular("Telefono de contacto para la entrega: ");
                                textoEntrega = "Delivery";
                            }
                            // RECOJO EN TIENDA
                            else
                            {
                                direccionCliente = "C. Jose Antonio Encinas 184";
                                distritoCliente = "Pueblo Libre 15084";
                                costoDelivery = 0.00;
                                referenciaCliente = "-";
                                telContacto = "-";
                                diasEntregas = "-";
                                textoEntrega = "Recojo en tienda";
                            }

                            totalPagar = subtotal + costoDelivery;
                            Console.WriteLine($"Costo de envio calculado: S/ {costoDelivery:F2}");

                            // ==========================================
                            // METODO DE PAGO
                            // ==========================================
                            Console.WriteLine("\n--- 4. METODO DE PAGO ---");
                            Console.WriteLine("Unico metodo disponible: YAPE");
                            Console.WriteLine($"Realice el pago de S/ {totalPagar:F2} al siguiente numero:");
                            Console.WriteLine("*** 987 654 321 - Finca Grandez ***\n");

                            while (true)
                            {
                                Console.Write("¿Ya realizo el pago por Yape? (S = Si / N = No): ");
                                string confirmacionPago = Console.ReadLine().Trim().ToUpper();

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

                            // CODIGO DE OPERACION YAPE
                            while (true)
                            {
                                Console.Write("Ingrese el codigo de operacion de Yape (8 digitos): ");
                                codigoOperacionYape = Console.ReadLine();

                                if (long.TryParse(codigoOperacionYape, out numeroValidar))
                                {
                                    if (codigoOperacionYape.Length == 8)
                                    {
                                        break;
                                    }
                                    Console.WriteLine("Error: el codigo debe tener exactamente 8 digitos.");
                                }
                                else
                                {
                                    Console.WriteLine("Error: el codigo debe contener solo numeros.");
                                }
                            }

                            // ==========================================
                            // TIPO DE COMPROBANTE
                            // ==========================================
                            while (true)
                            {
                                Console.WriteLine("\n--- 5. TIPO DE COMPROBANTE ---");
                                Console.WriteLine("1. Boleta (requiere DNI)");
                                Console.WriteLine("2. Factura (requiere RUC)");

                                while (true)
                                {
                                    Console.Write("Elija el tipo de comprobante (1-2): ");
                                    tipoComprobante = Console.ReadLine();
                                    if (tipoComprobante == "1" || tipoComprobante == "2")
                                    {
                                        break;
                                    }
                                    Console.WriteLine("Error: opcion invalida.");
                                }

                                bool volver = false;

                                // DNI
                                if (tipoComprobante == "1")
                                {
                                    while (true)
                                    {
                                        Console.Write("Ingrese su DNI (8 digitos) o 0 para volver: ");
                                        numDocumento = Console.ReadLine();

                                        if (numDocumento == "0")
                                        {
                                            volver = true;
                                            break;
                                        }

                                        if (long.TryParse(numDocumento, out numeroValidar))
                                        {
                                            if (numDocumento.Length == 8)
                                            {
                                                break;
                                            }
                                            Console.WriteLine("Error: el DNI debe tener exactamente 8 digitos.");
                                        }
                                        else
                                        {
                                            Console.WriteLine("Error: el DNI debe contener solo numeros.");
                                        }
                                    }
                                }
                                // RUC
                                else
                                {
                                    while (true)
                                    {
                                        Console.Write("Ingrese su RUC (11 digitos) o 0 para volver: ");
                                        numDocumento = Console.ReadLine();

                                        if (numDocumento == "0")
                                        {
                                            volver = true;
                                            break;
                                        }

                                        if (long.TryParse(numDocumento, out numeroValidar))
                                        {
                                            if (numDocumento.Length == 11)
                                            {
                                                break;
                                            }
                                            Console.WriteLine("Error: el RUC debe tener exactamente 11 digitos.");
                                        }
                                        else
                                        {
                                            Console.WriteLine("Error: el RUC debe contener solo numeros.");
                                        }
                                    }
                                }

                                if (volver == false)
                                {
                                    break;
                                }
                            }

                            // ==========================================
                            // DESCONTAR STOCK
                            // ==========================================
                            stock[productoElegido - 1] = setStock(stock[productoElegido - 1], stock[productoElegido - 1] - cantidad);

                            // ==========================================
                            // GENERAR Y GUARDAR PEDIDO
                            // ==========================================
                            idPedido = generarIdPedido();

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
                            pedidosFecha.Add("2026-10-04");
                            pedidosHora.Add("19:12");

                            if (textoEntrega == "Delivery")
                            {
                                pedidosEstado.Add("Pagado - Pendiente de entrega");
                            }
                            else
                            {
                                pedidosEstado.Add("Pagado - Listo para recojo");
                            }

                            // ==========================================
                            // MOSTRAR COMPROBANTE
                            // ==========================================
                            mostrarBoleta();
                        }

                        // ==============================================
                        // BUSCAR PEDIDO
                        // ==============================================
                        else if (opcionMenu == "3")
                        {
                            buscarPedido();
                        }

                        // ==============================================
                        // REPORTE
                        // ==============================================
                        else if (opcionMenu == "4")
                        {
                            reporteDiario();
                        }

                        // ==============================================
                        // CERRAR SESION
                        // ==============================================
                        else if (opcionMenu == "5")
                        {
                            Console.WriteLine("\nCerrando sesion...\n");
                            sesionIniciada = false;
                        }

                        else if (opcionMenu != "1")
                        {
                            Console.WriteLine("Opcion no valida, intente de nuevo.\n");
                        }
                    }
                }
            }

            // ======================================================
            // SALIR DEL SISTEMA
            // ======================================================
            else if (opcionInicio == "3")
            {
                Console.WriteLine("\nSaliendo del sistema...");
                break;
            }

            // ======================================================
            // OPCION INCORRECTA
            // ======================================================
            else
            {
                Console.WriteLine("Opcion no valida, intente de nuevo.\n");
            }
        }
    }
}