using HospiVital_App.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace HospiVital_App.Controllers
{
    public class receptoresController : Controller
    {
        //Inventario de viales
        private static colaPrioridadViales inventarioViales = new colaPrioridadViales();

        //Lista de Pacientes
        private static listaEnlazadaPacientes registroPacientes = new listaEnlazadaPacientes();

        //Lista de Donantes
        private static listaEnlazadaDonantes registroDonantes = new listaEnlazadaDonantes();

        //Historial de donativos
        private static List<dynamic> asignacionesRealizadas = new List<dynamic>();

        //Compatibilidad
        private servicioCompatibilidad compatibilidad = new servicioCompatibilidad();

        // GET: receptoresController
        public ActionResult Index()
        {
            //Por si la lista está vacía
            if (asignacionesRealizadas.Count == 0)
            {
                CargarDatosPrueba();
            }

            ViewBag.TotalPacientes = registroPacientes.Total;
            ViewBag.Completados = asignacionesRealizadas.Count;

            return View(asignacionesRealizadas);
        }

        //Método para simular datos iniciales
        private void CargarDatosPrueba()
        {
            //Crear donantes
            Donante d1 = new Donante(1, "Juan", "Pérez", "02345678-9", "7712-3456");
            Donante d2 = new Donante(2, "María", "García", "05123456-1", "7100-9988");
            Donante d3 = new Donante(3, "Carlos", "Hernández", "01010101-0", "2222-3333");

            registroDonantes.insertarFinal(d1);
            registroDonantes.insertarFinal(d2);
            registroDonantes.insertarFinal(d3);

            //Crear unidades de sangre
            unidadDeSangre v1 = new unidadDeSangre("B-1024-A", "A", "+", DateTime.Now.AddDays(-5), DateTime.Now.AddDays(10), "Disponible", 450, d1);
            unidadDeSangre v2 = new unidadDeSangre("B-1025-O", "O", "+", DateTime.Now.AddDays(-2), DateTime.Now.AddDays(30), "Disponible", 500, d2);
            unidadDeSangre v3 = new unidadDeSangre("B-1026-B", "B", "-", DateTime.Now.AddDays(-1), DateTime.Now.AddDays(5), "Disponible", 450, d3);

            inventarioViales.encolar(v1); 
            inventarioViales.encolar(v2); 
            inventarioViales.encolar(v3);

            //Crear pacientes
            paciente p1 = new paciente("José", "López", "A", "+");
            paciente p2 = new paciente("Elena", "Rivas", "B", "-");
            paciente p3 = new paciente("Roberto", "Sosa", "O", "+");
            //Registrar pacientes
            registroPacientes.insertarFinal(p1);
            registroPacientes.insertarFinal(p2);
            registroPacientes.insertarFinal(p3);

            //Simular las asignaciones automáticas
            procesarPacienteReciente(p1); 
            procesarPacienteReciente(p2); 
            procesarPacienteReciente(p3);
        }

        // GET: receptoresController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: receptoresController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: receptoresController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(paciente nuevoPaciente)
        {
            try
            {
                //Insertar nuevo paciente
                registroPacientes.insertarFinal(nuevoPaciente);

                //Intentar buscarle el vial de sangre inmediatamente
                procesarPacienteReciente(nuevoPaciente);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        private void procesarPacienteReciente(paciente p)
        {
            if (inventarioViales.estaVacia()) return;

            //Recorrido de la cola de viales
            nodoUnidadSangre actualVial = inventarioViales.obtenerFrente();
            nodoUnidadSangre anteriorVial = null;

            while (actualVial != null)
            {
                //Si el vial es compatible y no está vencido
                if (compatibilidad.esCompatible(actualVial.Dato, p) && !actualVial.Dato.estaVencida())
                {
                    unidadDeSangre unidadAsignada = actualVial.Dato;

                    if (anteriorVial == null) inventarioViales.desencolar();
                    else anteriorVial.Sig = actualVial.Sig;

                    //Registramos la asignación para la vista
                    asignacionesRealizadas.Add(new
                    {
                        IdReceptor = "REC-" + p.IdPaciente,
                        Beneficiario = p.Nombre + " " + p.Apellido,
                        TipoSangre = p.TipoSangreRequerido + p.FactorRhRequerido,
                        Donante = unidadAsignada.Donante.Nombre + " " + unidadAsignada.Donante.Apellido,
                        UnidadAsignada = unidadAsignada.IdUnidad,
                        FechaTransfusion = DateTime.Now.ToString("dd MMM yyyy, HH:mm", new CultureInfo("es-ES")).Replace(".", "")
                    });

                    return;
                }
                anteriorVial = actualVial;
                actualVial = actualVial.Sig;
            }
        }

        // GET: receptoresController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: receptoresController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: receptoresController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: receptoresController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
