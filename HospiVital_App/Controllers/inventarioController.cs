using System.ComponentModel;
using System.Globalization;
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

           
            // Donantes de prueba adicionales para dar variedad
            Donante d1 = new Donante(1, "Ana", "Martínez", "05123456-7", "7123-4567");
            Donante d2 = new Donante(2, "Carlos", "Pérez", "03456789-2", "7234-5678");
            Donante d3 = new Donante(3, "Elena", "Rodríguez", "02345671-5", "7345-6789");
            Donante d4 = new Donante(4, "Juan", "López", "01987654-3", "7456-7890");

            DateTime hoy = DateTime.Today;
            Random rnd = new Random();

            // Arrays para aleatorizar
            string[] grupos = { "A", "B", "O", "AB" };
            string[] factores = { "+", "-" };

            for (int i = 1; i <= 100; i++)
            {
                string grupo = grupos[rnd.Next(grupos.Length)];
                string factor = factores[rnd.Next(factores.Length)];

                // Alternamos fechas: algunos vencidos hace poco, otros vencen hoy, otros a futuro
                int diasOffset = rnd.Next(-10, 20);
                DateTime fechaCadu = hoy.AddDays(diasOffset);
                string estado = fechaCadu < hoy ? "Vencido" : "Disponible";

                // Seleccionamos un donante al azar de los 4 creados
                Donante donanteAzar = (i % 4 == 0) ? d1 : (i % 3 == 0) ? d2 : (i % 2 == 0) ? d3 : d4;

                inventario.agregarUnidad(new unidadDeSangre(
                    $"HP2026{1000 + i}", 
                    grupo,
                    factor,
                    hoy.AddDays(-5),     // Fecha Ingreso
                    fechaCadu,           // Fecha Caducidad
                    estado,
                    450,                 // Cantidad ml
                    donanteAzar
                ));
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


        // metodo para buscar unidades por id especifico
        [HttpGet]
        public ActionResult buscarUnidades(string termino)
        {

            //Recordatorio para despues de la entrega, una vez implementada db mejorar esto con la gestion de recurso

            //si listas nulo crear una lista vacia
            var lista = inventario.listaDisponibles() ?? new List<unidadDeSangre>();

            if (!string.IsNullOrWhiteSpace(termino))
            {
                termino = termino.Trim().ToLower();

                //filtra dentro de la lista y crear una nueva lista ya filtardo
                lista = lista.Where(
                    u =>
                    u.IdUnidad.ToLower().Contains(termino)).ToList();
            }

            //mapeamos los datos como se quiere que se contrauyan
            var resultado = lista.Select(u => new
            {
                idUnidad = u.IdUnidad,
                tipoSangre = u.TipoSangre,
                factorRh = u.FactorRh,
                fechaIngreso = u.FechaIngreso.ToString("dd/MM/yyyy"),
                fechaCaducidad = u.FechaCaducidad.ToString("dd/MM/yyyy"),
                estadoUnidad = u.EstadoUnidad,
                nombreDonante = u.Donante?.Nombre ?? "N/A",
                apellidoDonante = u.Donante?.Apellido ?? "N/A"
            });

            //aqui mandar el resultado con el formato que se quiere
            return Json(resultado);

        }

        //metodo para buscar segun el filtro aplicado
        [HttpGet]
        public ActionResult filtrarUnidades(string fecha, string estado, string tipoSangre)
        {
            //aqui tomamos la lista de dispnibles para ir filtrando secuencialmente
            var listaFiltrada = inventario.listaDisponibles() ?? new List<unidadDeSangre>();

//filtro rango de fechas
            if (fecha != "todos")
            {
                DateTime hoy = DateTime.Today;
                if (fecha == "hoy")
                    listaFiltrada = listaFiltrada.Where(u => u.FechaCaducidad.Date == hoy).ToList();
                else if (fecha == "7")
                    listaFiltrada = listaFiltrada.Where(u => u.FechaCaducidad.Date >= hoy.AddDays(-7) && u.FechaCaducidad.Date <= hoy).ToList();
                else if (fecha == "30")
                    listaFiltrada = listaFiltrada.Where(u => u.FechaCaducidad.Date >= hoy.AddDays(-30) && u.FechaCaducidad.Date <= hoy).ToList();
            }

//Filtro estado
            if (estado != "todos")
            {
                listaFiltrada = listaFiltrada.Where(u => u.EstadoUnidad.Equals(estado)).ToList();
            }

//filtro snagre
            if (tipoSangre != "todos" && !string.IsNullOrEmpty(tipoSangre))
            {
//separar en grupo y factor
                string grupo = tipoSangre.Substring(0, tipoSangre.Length - 1); 
                string factor = tipoSangre.Substring(tipoSangre.Length - 1); 

                listaFiltrada = listaFiltrada.Where(u =>
                    u.TipoSangre.Equals(grupo) &&
                    u.FactorRh == factor
                ).ToList();
            }

            var resultado = listaFiltrada.Select(u => new
            {
                idUnidad = u.IdUnidad,
                tipoSangre = u.TipoSangre,
                factorRh = u.FactorRh,
                fechaIngreso = u.FechaIngreso.ToString("dd/MM/yyyy"),
                fechaCaducidad = u.FechaCaducidad.ToString("dd/MM/yyyy"),
                estadoUnidad = u.EstadoUnidad,
                nombreDonante = u.Donante?.Nombre ?? "N/A",
                apellidoDonante = u.Donante?.Apellido ?? "N/A"
            });

            return Json(resultado);
        }
    }
}
