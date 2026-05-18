using HospiVital_App.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HospiVital_App.Controllers
{
    [Authorize(Roles = AppRoles.Admin)]
    public class salidasVialesVencidosController : Controller
    {
        private readonly baseDatosInterna baseDatos = baseDatosInterna.Instancia;

        private const int ElementosPorPagina = 5;

        [HttpGet]
        public IActionResult RegistroSalidas(int paginaPendientes = 1, int paginaSalidas = 1)
        {
            return View(CrearViewModel(paginaPendientes, paginaSalidas));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RegistrarSalida(string idUnidad, int paginaPendientes = 1, int paginaSalidas = 1)
        {
            string responsable = User.Identity?.Name ?? "Administrador";
            bool registrado = baseDatos.RegistrarSalidaVialVencido(idUnidad, responsable);

            TempData["SalidasVialesTipoMensaje"] = registrado ? "success" : "error";
            TempData["SalidasVialesMensaje"] = registrado
                ? "Salida de vial vencido registrada correctamente en la pila."
                : "No se pudo registrar la salida. Verifique que el vial exista y no tenga una salida registrada.";

            return RedirectToAction(nameof(RegistroSalidas), new { paginaPendientes, paginaSalidas });
        }

        private salidasVialesVencidosViewModel CrearViewModel(int paginaPendientes, int paginaSalidas)
        {
            List<unidadDeSangre> todosLosPendientes =
                baseDatos.ObtenerVialesVencidosPendientesSalida().ToList();

            List<registroSalidaVialVencido> todosLosRegistros =
                baseDatos.SalidasVialesVencidos.obtenerRegistros().ToList();

            int totalPendientes = todosLosPendientes.Count;
            int totalSalidas = todosLosRegistros.Count;

           
            int totalPaginasPendientes = Math.Max(1, (int)Math.Ceiling(totalPendientes / (double)ElementosPorPagina));
            int totalPaginasSalidas = Math.Max(1, (int)Math.Ceiling(totalSalidas / (double)ElementosPorPagina));

            paginaPendientes = Math.Clamp(paginaPendientes, 1, totalPaginasPendientes);
            paginaSalidas = Math.Clamp(paginaSalidas, 1, totalPaginasSalidas);

            IEnumerable<unidadDeSangre> pendientesPagina = todosLosPendientes
                .Skip((paginaPendientes - 1) * ElementosPorPagina)
                .Take(ElementosPorPagina);

            IEnumerable<registroSalidaVialVencido> registrosPagina = todosLosRegistros
                .Skip((paginaSalidas - 1) * ElementosPorPagina)
                .Take(ElementosPorPagina);

            return new salidasVialesVencidosViewModel
            {
                VialesPendientes = pendientesPagina,
                RegistrosSalida = registrosPagina,
                TotalPendientes = totalPendientes,
                TotalSalidas = baseDatos.SalidasVialesVencidos.Total,
                UltimaSalida = baseDatos.SalidasVialesVencidos.consultarTope(),
                PaginaPendientesActual = paginaPendientes,
                TotalPaginasPendientes = totalPaginasPendientes,
                PaginaSalidasActual = paginaSalidas,
                TotalPaginasSalidas = totalPaginasSalidas,
                ElementosPorPagina = ElementosPorPagina
            };
        }

        [HttpGet]
        public IActionResult ObtenerPendientesPagina(int pagina = 1)
        {
            var todos = baseDatos.ObtenerVialesVencidosPendientesSalida().ToList();

            int total = todos.Count;
            int totalPaginas = Math.Max(1, (int)Math.Ceiling(total / (double)ElementosPorPagina));
            pagina = Math.Clamp(pagina, 1, totalPaginas);

            var porcion = todos
                .Skip((pagina - 1) * ElementosPorPagina)
                .Take(ElementosPorPagina)
                .Select(v => new
                {
                    idUnidad = v.IdUnidad,
                    tipoSangre = v.TipoSangre,
                    factorRh = v.FactorRh,
                    fecha = v.FechaCaducidad.ToString("dd/MM/yyyy"),
                    donante = (v.Donante?.Nombre + " " + v.Donante?.Apellido).Trim()
                })
                .ToList();

            return Json(new
            {
                registros = porcion,
                paginaActual = pagina,
                totalPaginas,
                totalRegistros = total
            });
       
    }
    }
}