using HospiVital_App.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HospiVital_App.Controllers
{
    [Authorize(Roles = AppRoles.Admin)]
    public class usuariosController : Controller
    {
        private static readonly List<AppUser> _usuarios = new()
        {
            new AppUser
            {
                Usuario = "asistente",
                Contrasena = "1234",
                Nombre = "Asistente Médico",
                Rol = AppRoles.AsistenteMedico
            },
            new AppUser
            {
                Usuario = "medico",
                Contrasena = "1234",
                Nombre = "Médico",
                Rol = AppRoles.Medico
            },
            new AppUser
            {
                Usuario = "admin",
                Contrasena = "1234",
                Nombre = "Administrador",
                Rol = AppRoles.Admin
            }
        };

        public static AppUser? ValidarUsuario(string usuario, string contrasena)
        {
            return _usuarios.FirstOrDefault(x =>
                x.Usuario.Equals(usuario, StringComparison.OrdinalIgnoreCase) &&
                x.Contrasena == contrasena);
        }

        public static List<AppUser> ObtenerUsuarios()
        {
            return _usuarios;
        }

        [HttpGet]
        public IActionResult RegistroUsuarios()
        {
            return View(_usuarios);
        }
    }
}