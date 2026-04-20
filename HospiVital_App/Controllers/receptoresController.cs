using HospiVital_App.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace HospiVital_App.Controllers
{
    [Authorize(Roles = AppRoles.Medico)]
    public class receptoresController : Controller
    {
        private static colaPrioridadViales inventarioViales = new colaPrioridadViales();
        private static listaEnlazadaPacientes registroPacientes = new listaEnlazadaPacientes();
        private static listaEnlazadaDonantes registroDonantes = new listaEnlazadaDonantes();
        private static List<dynamic> asignacionesRealizadas = new List<dynamic>();

        private readonly servicioCompatibilidad compatibilidad = new servicioCompatibilidad();

        public IActionResult obtenerListado()
        {
            if (asignacionesRealizadas.Count == 0)
            {
                CargarDatosPrueba();
            }

            ViewBag.TotalPacientes = registroPacientes.Total;
            ViewBag.Completados = asignacionesRealizadas.Count;
            ViewBag.Compatibles = ObtenerCompatiblesPorTipo("O", "+");
            ViewBag.TipoFiltrado = "O+";

            return View(asignacionesRealizadas);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(paciente nuevoPaciente)
        {
            try
            {
                registroPacientes.insertarFinal(nuevoPaciente);
                procesarPacienteReciente(nuevoPaciente);

                return RedirectToAction(nameof(obtenerListado));
            }
            catch
            {
                return RedirectToAction(nameof(obtenerListado));
            }
        }

        [HttpPost]
        public IActionResult generarDetalleReceptor(string idReceptor)
        {
            var detalle = asignacionesRealizadas.FirstOrDefault(x => x.IdReceptor == idReceptor);

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
            var compatibles = ObtenerCompatiblesPorTipo(tipoSangre, factorRh);

            return Json(new
            {
                success = true,
                filtro = $"{tipoSangre}{factorRh}",
                data = compatibles
            });
        }

        private void CargarDatosPrueba()
        {
            if (asignacionesRealizadas.Count > 0) return;

            Donante d1 = new Donante(1, "Juan", "Pérez", "02345678-9", "7712-3456");
            Donante d2 = new Donante(2, "María", "García", "05123456-1", "7100-9988");
            Donante d3 = new Donante(3, "Carlos", "Hernández", "01010101-0", "2222-3333");

            registroDonantes.insertarFinal(d1);
            registroDonantes.insertarFinal(d2);
            registroDonantes.insertarFinal(d3);

            unidadDeSangre v1 = new unidadDeSangre(
                "B-1024-A", "A", "+",
                DateTime.Now.AddDays(-5),
                DateTime.Now.AddDays(10),
                "Disponible",
                450,
                d1
            );

            unidadDeSangre v2 = new unidadDeSangre(
                "B-1025-O", "O", "+",
                DateTime.Now.AddDays(-2),
                DateTime.Now.AddDays(30),
                "Disponible",
                500,
                d2
            );

            unidadDeSangre v3 = new unidadDeSangre(
                "B-1026-B", "B", "-",
                DateTime.Now.AddDays(-1),
                DateTime.Now.AddDays(5),
                "Disponible",
                450,
                d3
            );

            inventarioViales.encolar(v1);
            inventarioViales.encolar(v2);
            inventarioViales.encolar(v3);

            paciente p1 = new paciente("José", "López", "A", "+");
            paciente p2 = new paciente("Elena", "Rivas", "B", "-");
            paciente p3 = new paciente("Roberto", "Sosa", "O", "+");

            registroPacientes.insertarFinal(p1);
            registroPacientes.insertarFinal(p2);
            registroPacientes.insertarFinal(p3);

            procesarPacienteReciente(p1);
            procesarPacienteReciente(p2);
            procesarPacienteReciente(p3);
        }

        private void procesarPacienteReciente(paciente p)
        {
            if (inventarioViales.estaVacia()) return;

            nodoUnidadSangre actualVial = inventarioViales.obtenerFrente();
            nodoUnidadSangre anteriorVial = null;

            while (actualVial != null)
            {
                if (compatibilidad.esCompatible(actualVial.Dato, p) && !actualVial.Dato.estaVencida())
                {
                    unidadDeSangre unidadAsignada = actualVial.Dato;

                    if (anteriorVial == null)
                        inventarioViales.desencolar();
                    else
                        anteriorVial.Sig = actualVial.Sig;

                    asignacionesRealizadas.Add(new
                    {
                        IdReceptor = "REC-" + p.IdPaciente,
                        Beneficiario = p.Nombre + " " + p.Apellido,
                        Nombres = p.Nombre,
                        Apellidos = p.Apellido,
                        TipoSangre = p.TipoSangreRequerido + p.FactorRhRequerido,
                        Donante = unidadAsignada.Donante.Nombre + " " + unidadAsignada.Donante.Apellido,
                        DonanteDui = unidadAsignada.Donante.Dui,
                        DonanteTelefono = unidadAsignada.Donante.Telefono,
                        UnidadAsignada = unidadAsignada.IdUnidad,
                        FechaTransfusion = DateTime.Now
                            .ToString("dd MMM yyyy, HH:mm", new CultureInfo("es-ES"))
                            .Replace(".", "")
                    });

                    return;
                }

                anteriorVial = actualVial;
                actualVial = actualVial.Sig;
            }
        }

        private List<dynamic> ObtenerCompatiblesPorTipo(string tipoSangre, string factorRh)
        {
            var resultado = new List<dynamic>();
            nodoUnidadSangre actual = inventarioViales.obtenerFrente();

            while (actual != null)
            {
                var unidad = actual.Dato;

                if (!unidad.estaVencida()
                    && unidad.EstadoUnidad == "Disponible"
                    && unidad.TipoSangre == tipoSangre
                    && unidad.FactorRh == factorRh)
                {
                    resultado.Add(new
                    {
                        IdVial = unidad.IdUnidad,
                        Tipo = unidad.TipoSangre + unidad.FactorRh,
                        Ubicacion = "Nevera A1-Estante 1"
                    });
                }

                actual = actual.Sig;
            }

            return resultado;
        }
    }
}