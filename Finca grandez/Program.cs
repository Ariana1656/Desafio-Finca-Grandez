using System;
using System.Collections.Generic;

namespace PrototipoWebFincaGrandez
{
    internal class Program
    {
        // =================================================================================
        // 1. ESTRUCTURAS Y CONSTANTES DE PEDIDOS Y REPORTES
        // =================================================================================

        // PEDIDOS. Clave: id del pedido (ej. "PED-0001")
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
        // 2. FUNCIONES DE APOYO INTERNAS
        // =================================================================================

        static string GenerarIdPedido()
        {
            return "PED-" + siguienteIdPedido.ToString("D4");
        }

        static double ObtenerTotalPedido(string[] pedido)
        {
            return double.Parse(pedido[P_TOTAL]);
        }

        static string ElegirDistrito()
        {
            Console.WriteLine("\nDistritos con delivery (solo Lima):");
            for (int i = 0; i < distritosLima.Length; i++)
            {
                Console.WriteLine($"{i + 1}. {distritosLima[i]}");
            }
            int opcion = Validador.PedirEnteroEnRango("Elija su distrito (numero): ", 1, distritosLima.Length);
            return distritosLima[opcion - 1];
        }

        // ==========================================================
        // CONSULTAS DE PEDIDOS
        // ==========================================================
        static void BuscarPedido()
        {
            Console.WriteLine("\n--- BUSCAR PEDIDO ---");
            string idBuscado = Validador.Leer("Ingrese el ID del pedido (ej. PED-0001): ").Trim().ToUpper();

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

        static void MostrarTodosLosPedidos()
        {
            Console.WriteLine("\n--- TODOS LOS PEDIDOS ---");

            int cantidad = diccionarioPedidos.Count;
            if (cantidad == 0)
            {
                Console.WriteLine("No hay pedidos registrados.");
                return;
            }

            string[] orden = new string[cantidad];
            diccionarioPedidos.Keys.CopyTo(orden, 0);

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
        // FUNCIONES ADMIN (Llaman al módulo GestorInventario)
        // ==========================================================
        static void ReponerStock()
        {
            Console.WriteLine("\n--- INVENTARIO Y REPOSICION DE STOCK ---");
            GestorInventario.MostrarCatalogo();

            int numero;
            while (true)
            {
                string entrada = Validador.Leer("Ingrese el ID del producto a reponer (o 0 para cancelar): ").Trim();
                if (!int.TryParse(entrada, out numero))
                {
                    Console.WriteLine("Error: ingrese solo el numero del producto.");
                }
                else if (numero == 0)
                {
                    return;
                }
                else if (GestorInventario.ExisteProducto(numero))
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
                string entrada = Validador.Leer("Cantidad a sumar: ").Trim();
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

            GestorInventario.CambiarStock(numero, cantidad);
            Console.WriteLine($"Stock actualizado: {GestorInventario.ObtenerNombre(numero)} ahora tiene {GestorInventario.ObtenerStock(numero)} und.");
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
                string idPedido = Validador.Leer("Ingrese el ID del pedido a completar (ej. PED-0001, o 0 para salir): ").Trim().ToUpper();
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
                    GestorArchivo.GuardarPedidosArchivo(diccionarioPedidos);
                    return;
                }
            }
        }

        // ==========================================================
        // REPORTES Y COMPROBANTES
        // ==========================================================
        static void EscribirResumen(System.IO.TextWriter salida, int cantidad, int completados, int delivery, int recojo, double totalVendido)
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
            int cantidad = 0, completados = 0, delivery = 0, recojo = 0;
            double totalVendido = 0;

            Console.WriteLine("\n--- REPORTE DIARIO (" + hoy + ") ---");
            Console.WriteLine("---------------------------------------------------------");
            foreach (var ped in diccionarioPedidos)
            {
                if (ped.Value[P_FECHA] == hoy)
                {
                    cantidad++;
                    totalVendido += ObtenerTotalPedido(ped.Value);
                    if (ped.Value[P_ENTREGA] == "Delivery") delivery++;
                    else recojo++;
                    if (ped.Value[P_ESTADO] == "Completado") completados++;

                    Console.WriteLine($"{ped.Key} | {ped.Value[P_HORA]} | {ped.Value[P_CLIENTE]} | {ped.Value[P_PRODUCTO]} | {ped.Value[P_CANTIDAD]} und. | S/ {ObtenerTotalPedido(ped.Value):F2}");
                }
            }
            if (cantidad == 0)
            {
                Console.WriteLine("No hay pedidos registrados hoy.");
            }
            EscribirResumen(Console.Out, cantidad, completados, delivery, recojo, totalVendido);
        }

        static void OrdenarRanking(string[] tipos, int[] unidades, double[] ventas, bool porVenta)
        {
            for (int i = 0; i < tipos.Length - 1; i++)
            {
                for (int j = 0; j < tipos.Length - 1 - i; j++)
                {
                    bool cambiar = porVenta ? (ventas[j] < ventas[j + 1]) : (unidades[j] < unidades[j + 1]);
                    if (cambiar)
                    {
                        string auxTipo = tipos[j]; tipos[j] = tipos[j + 1]; tipos[j + 1] = auxTipo;
                        int auxUnidades = unidades[j]; unidades[j] = unidades[j + 1]; unidades[j + 1] = auxUnidades;
                        double auxVenta = ventas[j]; ventas[j] = ventas[j + 1]; ventas[j + 1] = auxVenta;
                    }
                }
            }
        }

        static void ProductosMasVendidos()
        {
            string[] tipos = { "Blend Premium", "Cafe Tradicion", "Cafe Catuai", "Cafe Geisha" };
            int[] unidades = new int[4];
            double[] ventas = new double[4];

            for (int i = 0; i < tipos.Length; i++)
            {
                unidades[i] = unidadesCafe[i];
                ventas[i] = montosCafe[i];
            }

            OrdenarRanking(tipos, unidades, ventas, false);

            Console.WriteLine("\n===============================================================");
            Console.WriteLine("       UNIDADES MAS VENDIDAS - FINCA GRANDEZ");
            Console.WriteLine("===============================================================");
            Console.WriteLine($"{"Puesto",-9}{"Tipo de cafe",-20}Unidades vendidas");
            for (int i = 0; i < tipos.Length; i++)
            {
                Console.WriteLine($"{"[" + (i + 1) + "]",-9}{tipos[i],-20}{unidades[i]} und.");
            }
            Console.WriteLine("===============================================================");

            OrdenarRanking(tipos, unidades, ventas, true);

            Console.WriteLine("\n===============================================================");
            Console.WriteLine("       PRODUCTOS CON MAYOR VENTA - FINCA GRANDEZ");
            Console.WriteLine("===============================================================");
            Console.WriteLine($"{"Puesto",-9}{"Tipo de cafe",-20}Venta total");
            for (int i = 0; i < tipos.Length; i++)
            {
                Console.WriteLine($"{"[" + (i + 1) + "]",-9}{tipos[i],-20}S/ {ventas[i]:F2}");
            }
            Console.WriteLine("===============================================================");
        }

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

            if (tipoComprobante == "1") Console.WriteLine(Centrar("BOLETA DE VENTA ELECTRONICA: B001-" + idPedido));
            else Console.WriteLine(Centrar("FACTURA ELECTRONICA: F001-" + idPedido));
            Console.WriteLine();

            if (tipoComprobante == "2")
            {
                Console.WriteLine("RUC: " + numDocumento + " | " + razonSocial);
                Console.WriteLine("Contacto: " + nombreCliente);
            }
            else Console.WriteLine("DNI: " + numDocumento + " | " + nombreCliente);

            Console.WriteLine("Fecha: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
            Console.WriteLine("Telefono: " + telefonoCliente);
            Console.WriteLine("Direccion: " + direccionCliente + ", " + distritoCliente);
            if (textoEntrega == "Delivery")
            {
                Console.WriteLine("Referencia: " + referenciaCliente);
                Console.WriteLine("Tel. contacto: " + telContacto);
                Console.WriteLine("Entrega en: " + diasEntregas);
            }
            else Console.WriteLine("Entrega: Recojo en tienda");

            Console.WriteLine("--------------------------------------------------------");
            Console.WriteLine($"{"ARTICULO",-30}{"CANT",6}{"P/U",10}{"TOTAL",10}");
            Console.WriteLine("--------------------------------------------------------");
            int items = 0;
            for (int i = 0; i < carrito.Length; i++)
            {
                if (carrito[i] > 0)
                {
                    items++;
                    double precioUnitario = GestorInventario.ObtenerPrecio(i + 1);
                    double totalProducto = precioUnitario * carrito[i];
                    Console.WriteLine($"{GestorInventario.ObtenerNombre(i + 1),-30}{carrito[i],6}{precioUnitario,10:F2}{totalProducto,10:F2}");
                }
            }
            Console.WriteLine("--------------------------------------------------------");

            double igv = totalPagar - totalPagar / 1.18;
            Console.WriteLine($"{"SUBTOTAL PEDIDO:",41}  S/ {subtotal,10:F2}");
            Console.WriteLine($"{"COSTO ENVIO:",41}  S/ {costoDelivery,10:F2}");
            Console.WriteLine($"{"IGV (18%):",41}  S/ {igv,10:F2}");
            Console.WriteLine($"{"Items: " + items,-14}{"TOTAL A PAGAR:",27}  S/ {totalPagar,10:F2}");
            Console.WriteLine("--------------------------------------------------------");
            Console.WriteLine("Forma de pago: YAPE");
            Console.WriteLine("Pagado a: 987 654 321 (Finca Grandez)");
            Console.WriteLine("Operacion ID: " + codigoOperacionYape);
            Console.WriteLine("--------------------------------------------------------");
            Console.WriteLine(Centrar("*Pedido registrado correctamente!"));
            Console.WriteLine();
        }

        // ==========================================================
        // CUENTA Y SESION (Llaman al módulo Usuarios)
        // ==========================================================
        static void CrearCuenta()
        {
            Console.WriteLine("\n--- CREACION DE CUENTA ---");

            string correoNuevo = "";
            while (true)
            {
                string correoInput = Validador.Leer("Correo electronico: ").Trim().ToLower();
                if (correoInput == "") Console.WriteLine("Error: el correo no puede estar vacio.");
                else if (!correoInput.Contains("@")) Console.WriteLine("Error: el correo debe contener '@'.");
                else if (!correoInput.EndsWith(".com")) Console.WriteLine("Error: el correo debe terminar en '.com'.");
                else if (correoInput.StartsWith("@") || correoInput.Split('@')[1] == ".com") Console.WriteLine("Error: el correo esta incompleto.");
                else if (Usuarios.CorreoYaRegistrado(correoInput)) Console.WriteLine("Error: ese correo ya esta registrado.");
                else { correoNuevo = correoInput; break; }
            }

            string usuarioNuevo = "";
            while (true)
            {
                usuarioNuevo = Validador.Leer("Nombre de usuario : ").Trim().ToLower();
                if (usuarioNuevo.Length < 3) Console.WriteLine("Error: el usuario debe tener al menos 3 caracteres.");
                else if (Usuarios.ExisteUsuario(usuarioNuevo)) Console.WriteLine("Error: ese usuario ya existe.");
                else break;
            }

            string claveTemporal = Validador.PedirTexto("Contrasena (minimo 4 caracteres): ", 4, "Error: la contrasena debe tener al menos 4 caracteres.");
            string claveConfirmacion = "";
            while (true)
            {
                claveConfirmacion = Validador.Leer("Confirme su contrasena: ");
                if (claveConfirmacion == claveTemporal) break;
                Console.WriteLine("Error: las contrasenas no coinciden.");
            }

            Usuarios.RegistrarUsuario(usuarioNuevo, claveConfirmacion, "user", usuarioNuevo, "", correoNuevo);
            Console.WriteLine("\n*Cuenta creada exitosamente! Ahora puede iniciar sesion.\n");
        }

        static void IniciarSesion()
        {
            string rol = "fallo";
            string usuarioIngresado = "";
            while (true)
            {
                usuarioIngresado = Validador.Leer("Usuario (0 para volver): ").Trim().ToLower();
                if (usuarioIngresado == "0") return;

                string claveIngresada = Validador.Leer("Contrasena: ");
                rol = Usuarios.ValidarLogin(usuarioIngresado, claveIngresada);

                if (rol != "fallo") break;
                Console.WriteLine("Error: usuario o contrasena incorrectos. Intente de nuevo.");
            }

            usuarioActual = usuarioIngresado;
            rolActual = rol;

            if (rolActual == "admin")
            {
                Console.WriteLine("\nBienvenido Administrador.\n");
                Validador.Pausa();
                MenuAdmin();
            }
            else
            {
                Console.WriteLine("\nInicio de sesion exitoso. Bienvenido a Finca Grandez.\n");
                Validador.Pausa();
                MenuUsuario();
            }
            usuarioActual = "";
            rolActual = "";
        }

        // ==========================================================
        // REALIZAR PEDIDO (Usa GestorInventario y GestorArchivo)
        // ==========================================================
        static void RealizarPedido()
        {
            int totalProductos = GestorInventario.ObtenerTotalProductos();
            int[] carrito = new int[totalProductos];

            string otroProducto = "S";
            while (otroProducto == "S")
            {
                GestorInventario.MostrarCatalogo();

                int productoElegido = 0;
                while (true)
                {
                    string entrada = Validador.Leer($"Elija el numero de producto (1-{totalProductos}): ");
                    if (!int.TryParse(entrada, out productoElegido)) Console.WriteLine("Error: ingrese solo el numero.");
                    else if (GestorInventario.ExisteProducto(productoElegido)) break;
                    else Console.WriteLine("Error: opcion de producto invalida.");
                }

                int stockActual = GestorInventario.ObtenerStock(productoElegido);

                if (stockActual == 0)
                {
                    Console.WriteLine("Error: ese producto no tiene stock disponible.");
                }
                else
                {
                    int cantidad = 0;
                    while (true)
                    {
                        string entrada = Validador.Leer("Ingrese la cantidad a comprar (Paquetes): ");
                        if (!int.TryParse(entrada, out cantidad)) Console.WriteLine("Error: ingrese solo numeros.");
                        else if (cantidad <= 0) Console.WriteLine("Error: la cantidad debe ser mayor a 0.");
                        else if (cantidad > stockActual) Console.WriteLine($"Error: la cantidad supera el stock ({stockActual} und.).");
                        else break;
                    }

                    carrito[productoElegido - 1] += cantidad;
                    GestorInventario.CambiarStock(productoElegido, -cantidad);
                    Console.WriteLine("Producto agregado al carrito.");
                }

                otroProducto = Validador.PedirSN("¿Desea anadir otro producto? (S = Si / N = Continuar con la compra): ");
            }

            double subtotal = 0;
            int totalUnidades = 0;
            string resumenProductos = "";
            for (int i = 0; i < carrito.Length; i++)
            {
                if (carrito[i] > 0)
                {
                    subtotal += GestorInventario.ObtenerPrecio(i + 1) * carrito[i];
                    totalUnidades += carrito[i];
                    if (resumenProductos != "") resumenProductos += " + ";
                    resumenProductos += GestorInventario.ObtenerNombre(i + 1) + " x" + carrito[i];
                }
            }

            if (totalUnidades == 0)
            {
                Console.WriteLine("No se agrego ningun producto al carrito.");
                return;
            }

            Console.WriteLine("\nCarrito confirmado. Procediendo al registro del cliente...");

            Console.WriteLine("\n--- 2. DATOS DEL CLIENTE ---");
            string nombreCliente = "";
            while (true)
            {
                nombreCliente = Validador.Leer("Nombre completo: ").Trim();
                if (nombreCliente.Length >= 3 && !Validador.TieneNumero(nombreCliente)) break;
                Console.WriteLine("Error: el nombre debe tener al menos 3 caracteres y sin numeros.");
            }
            string telefonoCliente = Validador.PedirCelular("Telefono celular: ");

            Console.WriteLine("\n--- 3. METODO DE ENTREGA ---");
            Console.WriteLine("1. Delivery (solo Lima - S/ 15.00)");
            Console.WriteLine("2. Recojo en Tienda (Gratis)");
            int tipoEntrega = Validador.PedirEnteroEnRango("Seleccione una opcion (1-2): ", 1, 2);

            string direccionCliente = "", referenciaCliente = "", distritoCliente = "", telContacto = "", diasEntregas = "", textoEntrega = "";
            double costoDelivery = 0;

            if (tipoEntrega == 1)
            {
                direccionCliente = Validador.PedirTexto("Direccion exacta de entrega: ", 7, "Error: ingrese una direccion valida.");
                referenciaCliente = Validador.PedirTexto("Referencia: ", 5, "Error: ingrese una referencia valida.");
                distritoCliente = ElegirDistrito();
                costoDelivery = COSTO_DELIVERY;
                diasEntregas = DIAS_ENTREGA;
                telContacto = Validador.PedirCelular("Telefono de contacto: ");
                textoEntrega = "Delivery";
            }
            else
            {
                direccionCliente = "C. Jose Antonio Encinas 184";
                distritoCliente = "Pueblo Libre 15084";
                costoDelivery = 0.00;
                referenciaCliente = "-"; telContacto = "-"; diasEntregas = "-";
                textoEntrega = "Recojo en tienda";
            }

            double totalPagar = subtotal + costoDelivery;
            Console.WriteLine($"Costo de envio calculado: S/ {costoDelivery:F2}");

            Console.WriteLine("\n--- 4. METODO DE PAGO ---");
            Console.WriteLine("Unico metodo disponible: YAPE");
            Console.WriteLine($"Realice el pago de S/ {totalPagar:F2} al numero: 987 654 321 - Finca Grandez\n");

            while (true)
            {
                if (Validador.PedirSN("¿Ya realizo el pago por Yape? (S/N): ") == "S") break;
                Console.WriteLine("Por favor realice el pago antes de continuar.");
            }

            string codigoOperacionYape = Validador.PedirDigitos("Ingrese el codigo de operacion de Yape (8 digitos): ", 8, "codigo");

            string tipoComprobante = "", numDocumento = "", razonSocial = "";
            while (true)
            {
                Console.WriteLine("\n--- 5. TIPO DE COMPROBANTE ---");
                Console.WriteLine("1. Boleta (DNI)\n2. Factura (RUC y Razon Social)");
                tipoComprobante = Validador.PedirEnteroEnRango("Elija comprobante (1-2): ", 1, 2).ToString();

                int longitud = (tipoComprobante == "1") ? 8 : 11;
                string documento = (tipoComprobante == "1") ? "DNI" : "RUC";

                bool volver = false;
                while (true)
                {
                    numDocumento = Validador.Leer($"Ingrese su {documento} ({longitud} digitos) o 0 para volver: ").Trim();
                    if (numDocumento == "0") { volver = true; break; }
                    if (!Validador.SoloDigitos(numDocumento)) Console.WriteLine($"Error: el {documento} debe ser solo numeros.");
                    else if (numDocumento.Length == longitud) break;
                    else Console.WriteLine($"Error: debe tener {longitud} digitos.");
                }

                if (tipoComprobante == "2" && !volver)
                {
                    while (true)
                    {
                        razonSocial = Validador.Leer("Ingrese Razon Social (o 0 para volver): ").Trim();
                        if (razonSocial == "0") { volver = true; break; }
                        else if (razonSocial.Length >= 3) break;
                        Console.WriteLine("Error: la Razon Social debe tener al menos 3 caracteres.");
                    }
                }
                if (!volver) break;
            }

            string idPedido = GenerarIdPedido();
            DateTime ahora = DateTime.Now;
            string estado = (textoEntrega == "Delivery") ? "Pagado - Pendiente de entrega" : "Pagado - Listo para recojo";

            diccionarioPedidos.Add(idPedido, new string[]
            {
                ahora.ToString("yyyy-MM-dd"), ahora.ToString("HH:mm"), usuarioActual, nombreCliente,
                resumenProductos, totalUnidades.ToString(), textoEntrega, direccionCliente + ", " + distritoCliente,
                referenciaCliente, telContacto, diasEntregas, totalPagar.ToString("0.00"), estado
            });
            siguienteIdPedido++;

            for (int i = 0; i < carrito.Length; i++)
            {
                if (carrito[i] > 0)
                {
                    int tipo = (i <= 2) ? 0 : (i <= 5) ? 1 : (i <= 7) ? 2 : 3;
                    unidadesCafe[tipo] += carrito[i];
                    montosCafe[tipo] += GestorInventario.ObtenerPrecio(i + 1) * carrito[i];
                }
            }

            MostrarBoleta(tipoComprobante, idPedido, numDocumento, razonSocial, nombreCliente,
                telefonoCliente, direccionCliente, distritoCliente, textoEntrega,
                referenciaCliente, telContacto, diasEntregas, carrito,
                subtotal, costoDelivery, totalPagar, codigoOperacionYape);

            GestorArchivo.GuardarPedidosArchivo(diccionarioPedidos);
        }

        // =================================================================================
        // 3. MENUS PRINCIPALES
        // =================================================================================

        static void MenuUsuario()
        {
            while (true)
            {
                Validador.LimpiarPantalla();
                Console.WriteLine("============= FINCA GRANDEZ - CLIENTE =============");
                Console.WriteLine("Sesion: " + usuarioActual);
                Console.WriteLine("1. Ver Catalogo de Productos y Stock\n2. Realizar un Pedido\n3. Mis Pedidos\n4. Buscar un Pedido\n5. Cerrar sesion");

                int opc;
                if (!int.TryParse(Validador.Leer("Ingresar opcion: "), out opc))
                {
                    Console.WriteLine("Ingresar un valor numerico.");
                    Validador.Pausa();
                    continue;
                }

                switch (opc)
                {
                    case 1:
                        GestorInventario.MostrarCatalogo();
                        if (Validador.PedirSN("¿Desea realizar un pedido? (S/N): ") == "S") RealizarPedido();
                        break;
                    case 2: RealizarPedido(); break;
                    case 3: MostrarMisPedidos(); break;
                    case 4: BuscarPedido(); break;
                    case 5: Console.WriteLine("\nCerrando sesion...\n"); return;
                    default: Console.WriteLine("Opcion no valida.\n"); break;
                }
                Validador.Pausa();
            }
        }

        static void MenuAdmin()
        {
            while (true)
            {
                Validador.LimpiarPantalla();
                Console.WriteLine("============= FINCA GRANDEZ - ADMINISTRADOR =============");
                Console.WriteLine("Sesion: " + usuarioActual);
                Console.WriteLine("1. Inventario y reposicion de stock\n2. Completar pedidos pendientes\n3. Ver todos los pedidos\n4. Buscar un pedido\n5. Reporte diario\n6. Productos mas vendidos\n7. Cerrar sesion");

                int opc;
                if (!int.TryParse(Validador.Leer("Ingresar opcion: "), out opc))
                {
                    Console.WriteLine("Ingresar un valor numerico.");
                    Validador.Pausa();
                    continue;
                }

                switch (opc)
                {
                    case 1: ReponerStock(); break;
                    case 2: CompletarPedidos(); break;
                    case 3: MostrarTodosLosPedidos(); break;
                    case 4: BuscarPedido(); break;
                    case 5: ReporteDiario(); break;
                    case 6: ProductosMasVendidos(); break;
                    case 7: Console.WriteLine("\nCerrando sesion...\n"); return;
                    default: Console.WriteLine("Opcion no valida.\n"); break;
                }
                Validador.Pausa();
            }
        }

        // PROGRAMA PRINCIPAL (Punto de entrada)
        static void Main(string[] args)
        {
            GestorArchivo.CargarPedidosArchivo(diccionarioPedidos, ref siguienteIdPedido);

            while (true)
            {
                Validador.LimpiarPantalla();
                Console.WriteLine("=========================================================");
                Console.WriteLine("      SISTEMA DE PEDIDOS - FINCA GRANDEZ");
                Console.WriteLine("=========================================================");
                Console.WriteLine("1. Crear cuenta\n2. Iniciar sesion\n3. Salir");
                string opcionInicio = Validador.Leer("Ingrese una opcion (1-3): ").Trim();

                switch (opcionInicio)
                {
                    case "1": CrearCuenta(); break;
                    case "2": IniciarSesion(); break;
                    case "3": Console.WriteLine("\nSaliendo del sistema..."); return;
                    default: Console.WriteLine("Opcion no valida, intente de nuevo.\n"); break;
                }

                Validador.Leer("\nPresione ENTER para continuar...");
            }
        }
    }
}