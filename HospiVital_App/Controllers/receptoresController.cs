using HospiVital_App.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HospiVital_App.Controllers
{
    public class receptoresController : Controller
    {
        //Paciente
        private static colaPrioridadPaciente colaPacientes = new colaPrioridadPaciente();
        private static List<unidadDeSangre> inventarioUnidades = new List<unidadDeSangre>();
        //Servicio de Compatibilidad
        private servicioCompatibilidad compatibilidad = new servicioCompatibilidad();
        //Lista de Donativos
        private static List<dynamic> asignacionesRealizadas = new List<dynamic>();

        // GET: receptoresController
        public ActionResult Index()
        {
            //Por si la lista está vacía
            if (asignacionesRealizadas.Count == 0)
            {
                CargarDatosPrueba();
            }


            return View(asignacionesRealizadas);
        }

        //Método para simular datos iniciales
        private void CargarDatosPrueba()
        {
            // Donante ficticio
            Donante d1 = new Donante(1, "Juan", "Pérez", "000000-0", "7777-7777");

            //Creamos una unidad y un paciente
            unidadDeSangre u1 = new unidadDeSangre("B-1024-A", "A", "+", DateTime.Now, DateTime.Now.AddMonths(1), "Disponible", 450, d1);

            //Creamos el registro de la tabla
            asignacionesRealizadas.Add(new
            {
                IdReceptor = "REC-9201",
                Beneficiario = "José López",
                TipoSangre = "A+",
                Donante = d1.Nombre + " " + d1.Apellido,
                UnidadAsignada = u1.IdUnidad,
                FechaTransfusion = DateTime.Now.ToString("dd Oct yyyy, HH:mm")
            });
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
                colaPacientes.encolar(nuevoPaciente, nuevoPaciente.Prioridad);
                procesarSiguienteEnCola();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        private void procesarSiguienteEnCola()
        {
            //Verificamos que no esté vacía
            if (colaPacientes.estaVacia()) return;

            //Obtenemos el primero según prioridad
            paciente proximo = colaPacientes.verPrimero();

            //Revisamos compatibilidad
            var compatibles = compatibilidad.filtrarCompatibles(inventarioUnidades, proximo);

            if (compatibles.Count > 0)
            {
                //Sacamos al paciente atendido de la cola
                colaPacientes.desencolar();

                //Removemos la unidad de sangre del inventario
                unidadDeSangre unidadAsignada = compatibles[0];
                inventarioUnidades.Remove(unidadAsignada);

                //Registramos la asignación para la tabla de la vista
                asignacionesRealizadas.Add(new
                {
                    IdReceptor = "REC-" + proximo.IdPaciente,
                    Beneficiario = proximo.Nombre + " " + proximo.Apellido,
                    TipoSangre = proximo.TipoSangreRequerido + proximo.FactorRhRequerido,
                    Donante = unidadAsignada.Donante.Nombre + " " + unidadAsignada.Donante.Apellido,
                    UnidadAsignada = unidadAsignada.IdUnidad,
                    FechaTransfusion = DateTime.Now.ToString("dd Oct yyyy, HH:mm")
                });
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
