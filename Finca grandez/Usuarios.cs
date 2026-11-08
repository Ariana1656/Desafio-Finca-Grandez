using System;
using System.Collections.Generic;

namespace PrototipoWebFincaGrandez
{
    public static class Usuarios
    {
        const int U_CLAVE = 0, U_ROL = 1, U_NOMBRE = 2, U_DIRECCION = 3, U_CORREO = 4;

        static Dictionary<string, string[]> diccionarioUsuarios = new Dictionary<string, string[]>()
        {
            { "admin", new string[] { "admin", "admin", "Administrador Finca Grandez", "", "admin@fincagrandez.com" } },
            { "user", new string[] { "user", "user", "Cliente Prueba", "Av. Falsa 123", "user@test.com" } }
        };

        public static string ValidarLogin(string usuario, string clave)
        {
            if (diccionarioUsuarios.ContainsKey(usuario) && diccionarioUsuarios[usuario][U_CLAVE] == clave)
            {
                return diccionarioUsuarios[usuario][U_ROL];
            }
            return "fallo";
        }

        public static bool CorreoYaRegistrado(string correo)
        {
            foreach (var u in diccionarioUsuarios)
            {
                if (u.Value[U_CORREO] == correo) return true;
            }
            return false;
        }

        public static bool ExisteUsuario(string nick) => diccionarioUsuarios.ContainsKey(nick);

        public static void RegistrarUsuario(string nick, string clave, string rol, string nombre, string direccion, string correo)
        {
            diccionarioUsuarios.Add(nick, new string[] { clave, rol, nombre, direccion, correo });
        }
    }
}