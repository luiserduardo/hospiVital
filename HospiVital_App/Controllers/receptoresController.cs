using HospiVital_App.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace HospiVital_App.Controllers
{
    [Authorize(Roles = AppRoles.Medico)]
    public class receptoresController : Controller
    {
        private readonly baseDatosInterna baseDatos = baseDatosInterna.Instancia;

        public IActionResult obtenerListado(
            string? busqueda,
            string? estado,
            string? tipoSangre,
            string? factorRh,
            string? fecha)
        {
            string estadoFiltro = string.IsNullOrWhiteSpace(estado) ? "all" : estado.Trim();
            string tipoFiltro = string.IsNullOrWhiteSpace(tipoSangre) ? "all" : tipoSangre.Trim().ToUpper();
            string rhFiltro = string.IsNullOrWhiteSpace(factorRh) ? "all" : factorRh.Trim();
            string fechaFiltro = string.IsNullOrWhiteSpace(fecha) ? "all" : fecha.Trim();
            string busquedaFiltro = busqueda?.Trim() ?? string.Empty;

            listaEnlazadaReceptorAsignacion listadoFiltrado = baseDatos.AsignacionesRealizadas.filtrar(
                busquedaFiltro,
                estadoFiltro,
                tipoFiltro,
                rhFiltro,
                fechaFiltro);

            ViewBag.TotalPacientes = baseDatos.RegistroPacientes.Total;
            ViewBag.Completados = baseDatos.AsignacionesRealizadas.contarPorEstado("Completado");
            ViewBag.Compatibles = new listaEnlazadaUnidadesCompatibles();
            ViewBag.TipoFiltrado = "Seleccione tipo y Rh";
            ViewBag.FiltroBusqueda = busquedaFiltro;
            ViewBag.FiltroEstado = estadoFiltro;
            ViewBag.FiltroTipoSangre = tipoFiltro;
            ViewBag.FiltroFactorRh = rhFiltro;
            ViewBag.FiltroFecha = fechaFiltro;
            ViewBag.TotalFiltrados = listadoFiltrado.Total;

            return View(listadoFiltrado);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(
            string dui,
            string nombre,
            string apellido,
            string tipoSangreRequerido,
            string factorRhRequerido,
            string? idUnidadSeleccionada)
        {
            string duiNormalizado = (dui ?? string.Empty).Trim();
            string nombreNormalizado = NormalizarTexto(nombre);
            string apellidoNormalizado = NormalizarTexto(apellido);
            string tipo = (tipoSangreRequerido ?? string.Empty).Trim().ToUpper();
            string factor = (factorRhRequerido ?? string.Empty).Trim();

            string? mensajeValidacion = ValidarDatosReceptor(
                duiNormalizado,
                nombreNormalizado,
                apellidoNormalizado,
                tipo,
                factor);

            if (mensajeValidacion != null)
            {
                TempData["ReceptoresTipoMensaje"] = "error";
                TempData["ReceptoresMensaje"] = mensajeValidacion;
                return RedirectToAction(nameof(obtenerListado));
            }

            if (baseDatos.RegistroPacientes.buscarPorDui(duiNormalizado) != null)
            {
                TempData["ReceptoresTipoMensaje"] = "error";
                TempData["ReceptoresMensaje"] = "Ya existe un receptor registrado con ese DUI.";
                return RedirectToAction(nameof(obtenerListado));
            }

            string? mensajeUnidad = baseDatos.ValidarUnidadSeleccionadaParaReceptor(
                idUnidadSeleccionada,
                tipo,
                factor);

            if (mensajeUnidad != null)
            {
                TempData["ReceptoresTipoMensaje"] = "warning";
                TempData["ReceptoresMensaje"] = mensajeUnidad;
                return RedirectToAction(nameof(obtenerListado));
            }

            paciente nuevoPaciente = new paciente(
                duiNormalizado,
                nombreNormalizado,
                apellidoNormalizado,
                tipo,
                factor);

            receptorAsignacion asignacion = baseDatos.RegistrarPacienteYAsignar(nuevoPaciente, idUnidadSeleccionada);

            TempData["ReceptoresTipoMensaje"] = asignacion.Estado == "Completado" ? "success" : "error";
            TempData["ReceptoresMensaje"] = asignacion.Estado == "Completado"
                ? "Receptor registrado y vial seleccionado asignado correctamente."
                : "No se pudo asignar el vial seleccionado. Verifique que siga disponible y no esté vencido.";

            return RedirectToAction(nameof(obtenerListado));
        }

        private string? ValidarDatosReceptor(
            string dui,
            string nombre,
            string apellido,
            string tipoSangre,
            string factorRh)
        {
            if (string.IsNullOrWhiteSpace(dui) ||
                string.IsNullOrWhiteSpace(nombre) ||
                string.IsNullOrWhiteSpace(apellido) ||
                string.IsNullOrWhiteSpace(tipoSangre) ||
                string.IsNullOrWhiteSpace(factorRh))
            {
                return "Debe completar DUI, nombre, apellido, tipo de sangre y factor Rh.";
            }

            if (!Regex.IsMatch(dui, @"^\d{8}-\d{1}$") || dui == "00000000-0")
            {
                return "El DUI debe tener el formato 00000000-0 y no puede ser un valor vacío.";
            }

            if (!TextoPersonaValido(nombre) || nombre.Length < 2 || nombre.Length > 60)
            {
                return "El nombre solo debe contener letras y espacios, con una longitud de 2 a 60 caracteres.";
            }

            if (!TextoPersonaValido(apellido) || apellido.Length < 2 || apellido.Length > 60)
            {
                return "El apellido solo debe contener letras y espacios, con una longitud de 2 a 60 caracteres.";
            }

            if (!TipoSangreValido(tipoSangre))
            {
                return "Seleccione un tipo de sangre válido: A, B, AB u O.";
            }

            if (!FactorRhValido(factorRh))
            {
                return "Seleccione un factor Rh válido: + o -.";
            }

            return null;
        }

        private static string NormalizarTexto(string? valor)
        {
            return Regex.Replace(valor ?? string.Empty, @"\s+", " ").Trim();
        }

        private static bool TextoPersonaValido(string valor)
        {
            return Regex.IsMatch(valor, @"^[A-Za-zÁÉÍÓÚáéíóúÑñÜü]+(?: [A-Za-zÁÉÍÓÚáéíóúÑñÜü]+)*$");
        }

        private static bool TipoSangreValido(string tipoSangre)
        {
            return tipoSangre == "A" || tipoSangre == "B" || tipoSangre == "AB" || tipoSangre == "O";
        }

        private static bool FactorRhValido(string factorRh)
        {
            return factorRh == "+" || factorRh == "-";
        }

        [HttpPost]
        public IActionResult generarDetalleReceptor(string idReceptor)
        {
            receptorAsignacion? detalle = baseDatos.AsignacionesRealizadas.buscarPorId(idReceptor);

            if (detalle == null)
            {
                return Json(new
                {
                    success = false,
                    message = "No se encontró la información del receptor."
                });
            }

            return Json(new
            {
                success = true,
                receptor = detalle
            });
        }

        [HttpGet]
        public IActionResult ObtenerCompatibles(string tipoSangre, string factorRh)
        {
            listaEnlazadaUnidadesCompatibles compatibles = baseDatos.ObtenerCompatiblesPorTipo(tipoSangre, factorRh);

            return Json(new
            {
                success = true,
                filtro = $"{tipoSangre}{factorRh}",
                total = compatibles.Total,
                cabeza = compatibles.Cabeza
            });
        }
    }
}
