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


        // GET: /Home/Index - Muestra el login

        [HttpGet]
        public IActionResult Index()
        {
            // Si ya esta autenticado, redirigir segun su rol
            if (HttpContext.Session.GetString("UsuarioId") != null)
            {
                return RedirigirSegunRol(HttpContext.Session.GetString("RolUsuario"));
            }

            return View("Login");
        }

        // POST: /Home/Index - Procesa el formulario de login

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(LoginViewModel model)
        {
            // Validar que el modelo sea valido (Data Annotations)
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

            // Autenticacion exitosa - Guardar datos en sesion
            HttpContext.Session.SetString("UsuarioId", usuario.Id.ToString());
            HttpContext.Session.SetString("Username", usuario.Username);
            HttpContext.Session.SetString("NombreCompleto", usuario.NombreCompleto);
            HttpContext.Session.SetString("RolUsuario", usuario.Rol.ToString());

            // Redirigir segun el rol del usuario
            return RedirigirSegunRol(usuario.Rol.ToString());
        }

        // POST: /Home/Logout - Cierra la sesion del usuario

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            // Limpiar la sesion
            HttpContext.Session.Clear();

            // Redirigir al login
            return RedirectToAction("Index");
        }

        // GET: /Home/AccessDenied - Vista de acceso denegado

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }


        // VISTAS POR ROL (dentro de Home)


        // GET: /Home/AdminDashboard
        public IActionResult AdminDashboard()
        {
            if (!VerificarAutenticacion("Administrador"))
            {
                return RedirectToAction("Index");
            }

            ViewBag.NombreUsuario = HttpContext.Session.GetString("NombreCompleto");
            ViewBag.Rol = HttpContext.Session.GetString("RolUsuario");
            return View();
        }

        // GET: /Home/RegistroRetiro
        public IActionResult RegistroRetiro()
        {
            if (!VerificarAutenticacion("Medico"))
            {
                return RedirectToAction("Index");
            }

            ViewBag.NombreUsuario = HttpContext.Session.GetString("NombreCompleto");
            ViewBag.Rol = HttpContext.Session.GetString("RolUsuario");
            return View();
        }

        // GET: /Home/IngresoSangre
        public IActionResult IngresoSangre()
        {
            if (!VerificarAutenticacion("TecnicoMedico"))
            {
                return RedirectToAction("Index");
            }

            ViewBag.NombreUsuario = HttpContext.Session.GetString("NombreCompleto");
            ViewBag.Rol = HttpContext.Session.GetString("RolUsuario");
            return View();
        }

        // OTRAS VISTAS 


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // METODOS PRIVADOS


        private IActionResult RedirigirSegunRol(string rol)
        {
            if (string.IsNullOrEmpty(rol))
            {
                return RedirectToAction("Index");
            }

            switch (rol)
            {
                case "Administrador":
                    return RedirectToAction("AdminDashboard"); //aquí cambienb lo que necesiten, ahí agregan las vistas que quieran para cada rol, solo es cuestión de crear/agregar la vista correspondiente

                case "Medico":
                    return RedirectToAction("RegistroRetiro");

                case "TecnicoMedico":
                    return RedirectToAction("IngresoSangre");

                default:
                    return RedirectToAction("Index");
            }
        }

        private bool VerificarAutenticacion(string rolRequerido)
        {
            var usuarioId = HttpContext.Session.GetString("UsuarioId");
            var rolUsuario = HttpContext.Session.GetString("RolUsuario");

            if (string.IsNullOrEmpty(usuarioId))
            {
                return false;
            }

            // Verificar si el rol coincide
            return rolUsuario == rolRequerido;
        }
    }
}
