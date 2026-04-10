using HospiVital_App.Models;
using Microsoft.AspNetCore.Mvc;

namespace HospiVital_App.Controllers
{
    public class inventarioController: Controller
    {
        //Para evitar que se borre statick
       private static inventario inventario = new inventario();

        public inventarioController()
        {
            if (inventario.listaDisponibles().Count == 0)
            {


                inventario.agregarUnidad(new unidadDeSangre(1, "O", "+",
    DateTime.Now, DateTime.Now.AddDays(5), "Disponible", 500));

                inventario.agregarUnidad(new unidadDeSangre(2,  "A", "+",
                    DateTime.Now, DateTime.Now.AddDays(2), "Disponible", 500));

                inventario.agregarUnidad(new unidadDeSangre(3,  "B", "-",
                    DateTime.Now, DateTime.Now.AddDays(10), "Disponible", 500));
     }
        }


        [HttpPost]
        public ActionResult agregarUnidad(int idUnidad,
             string tipoSangre,
            string factorRh, DateTime fechaIngreso,
            DateTime fechaCaducidad, string estadoUnidad, double cantidad){

            //objeto 
            unidadDeSangre unidad = new unidadDeSangre(
                        idUnidad, tipoSangre, factorRh, fechaIngreso,
                        fechaCaducidad, estadoUnidad,
                        cantidad);


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

    }
}
