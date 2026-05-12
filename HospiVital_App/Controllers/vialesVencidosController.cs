using HospiVital_App.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HospiVital_App.Controllers
{
    [Authorize(Roles = AppRoles.Admin)]
    public class vialesVencidosController : Controller
    {

        private readonly baseDatosInterna baseDatos = baseDatosInterna.Instancia;

        [HttpGet]
        public ActionResult obtenerListado()
        {
            // Recorre la lista enlazada y arma un IEnumerable para la vista
            List<unidadDeSangre> lista = new List<unidadDeSangre>();
            nodoVialVencido? actual = baseDatos.VialesVencidos.Cabeza;

            while (actual != null)
            {
                lista.Add(actual.Dato);
                actual = actual.Siguiente;
            }

            return View(lista);
        }
    }
}
