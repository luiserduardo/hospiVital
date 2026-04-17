using HospiVital_App.Models;
using Microsoft.AspNetCore.Mvc;

namespace HospiVital_App.Controllers
{
    public class inventarioController: Controller
    {
        //Para evitar que se borre statick
       private static inventario inventario = new inventario();

        //OJO, DATOS DE PRUEBA
        static inventarioController()
        {

            

                Donante donantePrueba = new Donante(0, "Roberto", "Gómez", "01234567-8", "7788-9900");

                inventario.agregarUnidad(new unidadDeSangre("HP202693361252", "B", "+",
    DateTime.Now, DateTime.Now.AddDays(-5), "Vencido", 500, donantePrueba));

                inventario.agregarUnidad(new unidadDeSangre("HP202693363113",  "A", "+",
                    DateTime.Now, DateTime.Now.AddDays(2), "Disponible", 500, donantePrueba));

                inventario.agregarUnidad(new unidadDeSangre("HP202693361231",  "B", "-",
                    DateTime.Now, DateTime.Now.AddDays(10), "Disponible", 500, donantePrueba));
     }
        


        [HttpPost]
        public ActionResult agregarUnidad(string idUnidad,
             string tipoSangre,
            string factorRh, DateTime fechaIngreso,
            DateTime fechaCaducidad, string estadoUnidad, double cantidad, 
            string dui, string nombre, string apellido, 
            string telefono, string peso, Donante donante
            ){

            //agregar validacion de si sucede algun error no hacer nada, RECORDAR

            //primer crear el donante
 
            unidadDeSangre unidad = new unidadDeSangre(
                        idUnidad, tipoSangre, factorRh, fechaIngreso,
                        fechaCaducidad, estadoUnidad,
                        cantidad, donante);


            //llamar inventario
            inventario.agregarUnidad(unidad);


            //agregar indicativo de exito
            return RedirectToAction("obtenerListado");
        }


        [HttpGet]
      public ActionResult obtenerListado()
        {
            var lista = inventario.listaDisponibles() ?? new List<unidadDeSangre>(); return View(lista);
        }


        //generar codigo, consultado por js inventario 
        [HttpGet]
        public ActionResult generarCodigo()
        {
            Random random = new Random();

            string anio = DateTime.Now.Year.ToString("D2");
            string numeroUnico = random.Next(1000000).ToString("D6");
            string segundos = DateTime.Now.Second.ToString("D2");

            string codigo = $"HP{anio}{numeroUnico}{segundos}";


            return Content(codigo);
        }

        [HttpPost]
        public ActionResult generarDetalleUnidad(string idUnidad)
        {
            var unidad = inventario.buscarUnidad(idUnidad);

            if (unidad == null)
            {
                return Json(new { success = false, message = "Unidad no encontrada" });
            }

            return Json(new
            {
                success = true,

                //Mandar todos los datoss, incluso de la clase que contiene 
                unidad = new
                {
                    idUnidad = unidad.IdUnidad,
                    tipoSangre = unidad.TipoSangre,
                    factorRh = unidad.FactorRh,
                    fechaIngreso = unidad.FechaIngreso.ToString("dd/MM/yyyy"),
                    fechaCaducidad = unidad.FechaCaducidad.ToString("dd/MM/yyyy"),
                    cantidad = unidad.Cantidad.ToString()
                    ,
                    nombreDonante = unidad.Donante?.Nombre ?? "N/A",
                    apellidoDonante = unidad.Donante.Apellido ?? "N/A",
                    dui = unidad.Donante?.Dui ?? "N/A",
                    telefono = unidad.Donante?.Telefono ?? "N/A",
                    peso = unidad.Donante.Peso,
                    estado = unidad.EstadoUnidad
                }
            });
        }

    }
}
