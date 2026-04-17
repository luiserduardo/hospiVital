using Microsoft.AspNetCore.Mvc;
using HospiVital_App.Models;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace HospiVital_App.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        // DATOS DE PRUEBA - Usuarios en memoria
        private static readonly List<Usuario> _usuarios = new List<Usuario>
        {
            new Usuario
            {
                Id = 1,
                Username = "admin",
                Password = "admin123",
                NombreCompleto = "Administrador del Sistema",
                Rol = RolUsuario.Administrador
            },
            new Usuario
            {
                Id = 2,
                Username = "medico",
                Password = "medico123",
                NombreCompleto = "Dr. Eduardo Lopez",
                Rol = RolUsuario.Medico
            },
            new Usuario
            {
                Id = 3,
                Username = "tecnico",
                Password = "tecnico123",
                NombreCompleto = "Tec. Yanira Rivas",
                Rol = RolUsuario.TecnicoMedico
            }
        };

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        // --- MÉTODOS DE AUTENTICACIÓN ---

        // GET: /Home/Index - Cambiado de Login a Index para coincidir con la ruta por defecto
        [HttpGet]
        public IActionResult Index()
        {
            // Si ya está autenticado, redirigir según su rol
            if (HttpContext.Session.GetString("UsuarioId") != null)
            {
                return RedirigirSegunRol(HttpContext.Session.GetString("RolUsuario"));
            }

            // Busca específicamente el archivo Login.cshtml en Views/Home/
            return View("Login");
        }

        // POST: /Home/Index - Procesa el formulario de login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(LoginViewModel model)
        {
            // Validar que el modelo sea válido
            if (!ModelState.IsValid)
            {
                return View("Login", model);
            }

            // Buscar usuario en la lista
            var usuario = _usuarios.FirstOrDefault(u =>
                u.Username.ToLower() == model.Username.ToLower() &&
                u.Password == model.Password);

            // Validar credenciales
            if (usuario == null)
            {
                ModelState.AddModelError(string.Empty, "Usuario o contraseña incorrectos");
                return View("Login", model);
            }

            // Autenticación exitosa - Guardar datos en sesión
            HttpContext.Session.SetString("UsuarioId", usuario.Id.ToString());
            HttpContext.Session.SetString("Username", usuario.Username);
            HttpContext.Session.SetString("NombreCompleto", usuario.NombreCompleto);
            HttpContext.Session.SetString("RolUsuario", usuario.Rol.ToString());

            return RedirigirSegunRol(usuario.Rol.ToString());
        }

        // POST: /Home/Logout - Cierra la sesión
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        // --- VISTAS POR ROL ---

        public IActionResult AdminDashboard()
        {
            if (!VerificarAutenticacion("Administrador")) return RedirectToAction("Index");
            
            CargarDatosVista();
            return View();
        }

        public IActionResult RegistroRetiro()
        {
            if (!VerificarAutenticacion("Medico")) return RedirectToAction("Index");
            
            CargarDatosVista();
            return View();
        }

        public IActionResult IngresoSangre()
        {
            if (!VerificarAutenticacion("TecnicoMedico")) return RedirectToAction("Index");
            
            CargarDatosVista();
            return View();
        }

        // --- OTRAS VISTAS ---

        public IActionResult AccessDenied() => View();
        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // --- MÉTODOS PRIVADOS ---

        private void CargarDatosVista()
        {
            ViewBag.NombreUsuario = HttpContext.Session.GetString("NombreCompleto");
            ViewBag.Rol = HttpContext.Session.GetString("RolUsuario");
        }

        private IActionResult RedirigirSegunRol(string rol)
        {
            if (string.IsNullOrEmpty(rol)) return RedirectToAction("Index");

            return rol switch
            {
                "Administrador" => RedirectToAction("AdminDashboard"),
                "Medico" => RedirectToAction("RegistroRetiro"),
                "TecnicoMedico" => RedirectToAction("IngresoSangre"),
                _ => RedirectToAction("Index"),
            };
        }

        private bool VerificarAutenticacion(string rolRequerido)
        {
            var usuarioId = HttpContext.Session.GetString("UsuarioId");
            var rolUsuario = HttpContext.Session.GetString("RolUsuario");
            return !string.IsNullOrEmpty(usuarioId) && rolUsuario == rolRequerido;
        }
    }
}