using HospiVital_App.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HospiVital_App.Controllers
{
    [Authorize(Roles = AppRoles.Admin)]
    public class usuariosController : Controller
    {
        private static readonly baseDatosInterna baseDatos = baseDatosInterna.Instancia;

        public static AppUser? ValidarUsuario(string usuario, string contrasena)
        {
            return baseDatos.ValidarUsuario(usuario, contrasena);
        }

        public static listaEnlazadaUsuarios ObtenerUsuarios()
        {
            return baseDatos.ObtenerUsuarios();
        }

        [HttpGet]
        public IActionResult RegistroUsuarios()
        {
            ViewBag.Roles = new[]
            {
                AppRoles.AsistenteMedico,
                AppRoles.Medico,
                AppRoles.Admin
            };

            return View(baseDatos.ObtenerUsuarios());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Crear(string nuevoUsuario, string nuevaContrasena, string nuevoNombre, string nuevoRol)
        {
            AppUser usuario = new AppUser
            {
                Usuario = nuevoUsuario,
                Contrasena = nuevaContrasena,
                Nombre = nuevoNombre,
                Rol = nuevoRol
            };

            bool creado = baseDatos.CrearUsuario(usuario);
            TempData["UsuariosTipoMensaje"] = creado ? "success" : "error";
            TempData["UsuariosMensaje"] = creado
                ? "Usuario creado correctamente."
                : "No se pudo crear el usuario. Verifique que completó los datos y que el usuario no exista.";

            return RedirectToAction(nameof(RegistroUsuarios));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(
            string usuarioOriginal,
            string usuarioEditado,
            string contrasenaEditada,
            string nombreEditado,
            string rolEditado)
        {
            AppUser usuario = new AppUser
            {
                Usuario = usuarioEditado,
                Contrasena = contrasenaEditada,
                Nombre = nombreEditado,
                Rol = rolEditado
            };

            bool actualizado = baseDatos.ActualizarUsuario(usuarioOriginal, usuario);
            TempData["UsuariosTipoMensaje"] = actualizado ? "success" : "error";
            TempData["UsuariosMensaje"] = actualizado
                ? "Usuario actualizado correctamente."
                : "No se pudo actualizar el usuario. Verifique que completó los datos y que el nuevo usuario no esté repetido.";

            return RedirectToAction(nameof(RegistroUsuarios));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Eliminar(string usuario)
        {
            bool eliminado = baseDatos.EliminarUsuario(usuario);
            TempData["UsuariosTipoMensaje"] = eliminado ? "success" : "error";
            TempData["UsuariosMensaje"] = eliminado
                ? "Usuario eliminado correctamente."
                : "No se puede eliminar el administrador principal del sistema.";

            return RedirectToAction(nameof(RegistroUsuarios));
        }
    }
}
