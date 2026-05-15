using HospiVital_App.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HospiVital_App.Controllers
{
    [Authorize(Roles = AppRoles.Admin)]
    public class salidasVialesVencidosController : Controller
    {
        private readonly baseDatosInterna baseDatos = baseDatosInterna.Instancia;

        [HttpGet]
        public IActionResult RegistroSalidas()
        {
            return View(CrearViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RegistrarSalida(string idUnidad)
        {
            string responsable = User.Identity?.Name ?? "Administrador";
            bool registrado = baseDatos.RegistrarSalidaVialVencido(idUnidad, responsable);

            TempData["SalidasVialesTipoMensaje"] = registrado ? "success" : "error";
            TempData["SalidasVialesMensaje"] = registrado
                ? "Salida de vial vencido registrada correctamente en la pila."
                : "No se pudo registrar la salida. Verifique que el vial exista y no tenga una salida registrada.";

            return RedirectToAction(nameof(RegistroSalidas));
        }

        private salidasVialesVencidosViewModel CrearViewModel()
        {
            IEnumerable<unidadDeSangre> pendientes = baseDatos.ObtenerVialesVencidosPendientesSalida().ToList();
            IEnumerable<registroSalidaVialVencido> registros = baseDatos.SalidasVialesVencidos.obtenerRegistros().ToList();

            return new salidasVialesVencidosViewModel
            {
                VialesPendientes = pendientes,
                RegistrosSalida = registros,
                TotalPendientes = pendientes.Count(),
                TotalSalidas = baseDatos.SalidasVialesVencidos.Total,
                UltimaSalida = baseDatos.SalidasVialesVencidos.consultarTope()
            };
        }
    }
}
