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

            //Para contabilizar los donativos realizados en el día
            DateTime hoy = DateTime.Today;

            ViewBag.CompletadosHoy = asignacionesRealizadas.Count(a => {
                //Compara le fecha almacenada con la fecha actual
                DateTime fechaReg;
                if (DateTime.TryParse(a.FechaTransfusion.ToString(), out fechaReg))
                {
                    return fechaReg.Date == hoy;
                }
                return false;
            });

            return View(asignacionesRealizadas);
        }

        //Método para simular datos iniciales
        private void CargarDatosPrueba()
        {
            //Crear donantes
            Donante d1 = new Donante(1, "Juan", "Pérez", "02345678-9", "7712-3456");
            Donante d2 = new Donante(2, "María", "García", "05123456-1", "7100-9988");
            Donante d3 = new Donante(3, "Carlos", "Hernández", "01010101-0", "2222-3333");
            Donante d4 = new Donante(4, "Lucía", "Méndez", "09876543-2", "7544-1122");
            Donante d5 = new Donante(5, "Pedro", "Rivas", "04455667-8", "6100-2233");

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
            //Mas unidades de prueba
            inventarioViales.encolar(new unidadDeSangre("B-2020-O-", "O", "-", DateTime.Now, DateTime.Now.AddDays(42), "Disponible", 450, d4));
            inventarioViales.encolar(new unidadDeSangre("B-3030-AB", "AB", "+", DateTime.Now, DateTime.Now.AddDays(15), "Disponible", 500, d5));
            inventarioViales.encolar(new unidadDeSangre("B-4040-A-", "A", "-", DateTime.Now.AddDays(-3), DateTime.Now.AddDays(20), "Disponible", 450, d1));
            inventarioViales.encolar(new unidadDeSangre("B-5050-B+", "B", "+", DateTime.Now.AddDays(-1), DateTime.Now.AddDays(25), "Disponible", 500, d2));
            inventarioViales.encolar(new unidadDeSangre("B-6060-O+", "O", "+", DateTime.Now, DateTime.Now.AddDays(3), "Disponible", 450, d3));

            //Crear pacientes
            paciente p1 = new paciente("12345678-9", "José", "López", "A", "+");
            paciente p2 = new paciente("98765432-1","Elena", "Rivas", "B", "-");
            paciente p3 = new paciente("22334455-6","Roberto", "Sosa", "O", "+");
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
                        FechaTransfusion = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        Dui = p.Dui
                    });

                    return;
                }
                anteriorVial = actualVial;
                actualVial = actualVial.Sig;
            }
        }

        [HttpGet]
        public JsonResult ObtenerVialesCompatibles(string tipoSangreCompleto)
        {
            if (string.IsNullOrEmpty(tipoSangreCompleto)) return Json(new List<object>());

            //Para buscar segun el tipo
            string tipo = tipoSangreCompleto.Substring(0, tipoSangreCompleto.Length - 1);
            string rh = tipoSangreCompleto.Substring(tipoSangreCompleto.Length - 1);

            //Paciente temporal para buscar un vial compatible
            paciente pTemp = new paciente { TipoSangreRequerido = tipo, FactorRhRequerido = rh };
            //Se crea una lista para los viales compatibles
            List<object> disponibles = new List<object>();
            nodoUnidadSangre actual = inventarioViales.obtenerFrente();

            while (actual != null)
            {
                //Si es compatible y no está vencida
                if (compatibilidad.esCompatible(actual.Dato, pTemp) && !actual.Dato.estaVencida())
                {
                    //Se agrega a la lista
                    disponibles.Add(new
                    {
                        id = actual.Dato.IdUnidad,
                        tipo = actual.Dato.TipoSangre + actual.Dato.FactorRh,
                        ubicacion = "Nevera Principal - Estante 1"
                    });
                }
                actual = actual.Sig;
            }
            //Se devuelven los viales disponibles
            return Json(disponibles);
        }

        [HttpPost]
        public ActionResult RegistrarAsignacion(string dui, string nombres, string apellidos, string tipoSangre, string vialId)
        {
            //Crear un paciente
            string tipo = tipoSangre.Substring(0, tipoSangre.Length - 1);
            string rh = tipoSangre.Substring(tipoSangre.Length - 1);
            paciente nuevo = new paciente(dui, nombres, apellidos, tipo, rh);
            registroPacientes.insertarFinal(nuevo);

            //Buscar el vial de sangre asignado
            unidadDeSangre unidadAsignada = null;
            nodoUnidadSangre actualVial = inventarioViales.obtenerFrente();
            nodoUnidadSangre anteriorVial = null;

            //Comprueba que haya existencia de viales
            while (actualVial != null)
            {
                if (actualVial.Dato.IdUnidad == vialId)
                {
                    unidadAsignada = actualVial.Dato;
                    //Se desencola de la TAD
                    if (anteriorVial == null) inventarioViales.desencolar();
                    else anteriorVial.Sig = actualVial.Sig;
                    break;
                }
                anteriorVial = actualVial;
                actualVial = actualVial.Sig;
            }

            //Comprueba que se haya seleccionado un vial de sangre existente
            if (unidadAsignada != null)
            {
                //Se registra en la vista
                asignacionesRealizadas.Add(new
                {
                    IdReceptor = "REC-" + nuevo.IdPaciente,
                    Beneficiario = nuevo.Nombre + " " + nuevo.Apellido,
                    TipoSangre = nuevo.TipoSangreRequerido + nuevo.FactorRhRequerido,
                    Donante = unidadAsignada.Donante.Nombre + " " + unidadAsignada.Donante.Apellido,
                    UnidadAsignada = unidadAsignada.IdUnidad,
                    FechaTransfusion = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Dui = dui
                });
            }

            return RedirectToAction("Index");
        }

        // GET: receptoresController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: receptoresController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, string dui, string nombres, string apellidos, string tipoSangre)
        {
            try
            {
                //Buscamos el paciente en nuestro inventario
                paciente p = registroPacientes.buscarPorId(id);
                if (p != null)
                {
                    p.Nombre = nombres;
                    p.Apellido = apellidos;
                }

                //Actualizamos la tabla
                string idReceptorBuscado = "REC-" + id;
                int index = asignacionesRealizadas.FindIndex(a => a.IdReceptor == idReceptorBuscado);

                if (index != -1)
                {
                    //Recuperamos los datos que no cambian
                    var original = asignacionesRealizadas[index];

                    //Reemplazamos los datos anteriores por un nuevo objeto con los datos actualizados
                    asignacionesRealizadas[index] = new
                    {
                        IdReceptor = idReceptorBuscado,
                        Beneficiario = nombres + " " + apellidos,
                        TipoSangre = tipoSangre,
                        Donante = original.Donante, 
                        UnidadAsignada = original.UnidadAsignada, 
                        FechaTransfusion = original.FechaTransfusion, 
                        Dui = original.Dui
                    };
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: receptoresController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: receptoresController/Delete/5
        [HttpPost]
        public ActionResult Delete(string id)
        {
            try
            {
                //Buscamos el registro en el historial
                var registro = asignacionesRealizadas.FirstOrDefault(a => a.IdReceptor == id);

                if (registro != null)
                {
                    //Recuperamos el id del paciente
                    int idPacienteNum = int.Parse(id.Replace("REC-", ""));
                    //Eliminar al paciente de la lista
                    registroPacientes.eliminarPaciente(idPacienteNum);

                    //Eliminamos del historial
                    asignacionesRealizadas.Remove(registro);

                    return Json(new
                    {
                        success = true,
                        message = "Registro eliminado correctamente. Inventario y contadores actualizados."
                    });
                }

                return Json(new { success = false, message = "No se encontró el registro." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }
    }
}
