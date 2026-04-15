using HospiVital_App.Models;
using Microsoft.AspNetCore.Mvc;

namespace HospiVital_App.Controllers
{
    public class inventarioController: Controller
    {
        //Para evitar que se borre statick
       private static inventario inventario = new inventario();

        int i = 0;
        //OJO, DATOS DE PRUEBA
        public inventarioController()
        {

            if (inventario.listaDisponibles().Count == i)
            {


                Donante donantePrueba = new Donante(0, "Roberto", "Gómez", "01234567-8", "7788-9900");

                inventario.agregarUnidad(new unidadDeSangre("HP202693361252", "O", "+",
    DateTime.Now, DateTime.Now.AddDays(5), "Disponible", 500, donantePrueba));

                inventario.agregarUnidad(new unidadDeSangre("HP202693363113",  "A", "+",
                    DateTime.Now, DateTime.Now.AddDays(2), "Disponible", 500, donantePrueba));

                inventario.agregarUnidad(new unidadDeSangre("HP202693361231",  "B", "-",
                    DateTime.Now, DateTime.Now.AddDays(10), "Disponible", 500, donantePrueba));
     }
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
            var lista = inventario.listaDisponibles();
            return View(lista);
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
        


    }
}
