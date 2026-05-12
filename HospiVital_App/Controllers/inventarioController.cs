using HospiVital_App.Models;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace HospiVital_App.Controllers
{
    public class inventarioController : Controller
    {
        private readonly baseDatosInterna baseDatos = baseDatosInterna.Instancia;

        [HttpPost]
        public ActionResult agregarUnidad(
            string idUnidad,
            string tipoSangre,
            string factorRh,
            DateTime fechaIngreso,
            DateTime fechaCaducidad,
            string estadoUnidad,
            double cantidad,
            string dui,
            string nombre,
            string apellido,
            string telefono,
            string peso)
        {
            double pesoConvertido = ConvertirPeso(peso);

            Donante donante = baseDatos.RegistrarDonante(
                nombre,
                apellido,
                dui,
                telefono,
                pesoConvertido);

            unidadDeSangre unidad = new unidadDeSangre(
                idUnidad,
                tipoSangre,
                factorRh,
                fechaIngreso,
                fechaCaducidad,
                string.IsNullOrWhiteSpace(estadoUnidad) ? "Disponible" : estadoUnidad,
                cantidad,
                donante);

            bool unidadRegistrada = baseDatos.AgregarUnidad(unidad);

            TempData["InventarioTipoMensaje"] = unidadRegistrada ? "success" : "error";
            TempData["InventarioMensaje"] = unidadRegistrada
                ? "Donación registrada correctamente."
                : "No se pudo registrar la donación. Verifique que el ID de donación no exista ni haya sido asignado previamente.";

            return RedirectToAction("obtenerListado");
        }

        [HttpGet]
        public ActionResult obtenerListado(int pagina = 1)
        {
            int elementosPorPagina = 5;
            int totalUnidades = baseDatos.Inventario.TotalUnidades;
            int totalPaginas = (int)Math.Ceiling((double)totalUnidades / elementosPorPagina);
            pagina = Math.Max(1, Math.Min(pagina, Math.Max(1, totalPaginas)));

            var datosPagina = baseDatos.Inventario.obtenerPagina(pagina, elementosPorPagina);

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new
                {
                    paginas = new { actual = pagina, total = totalPaginas, totalUnidades },
                    filas = datosPagina.Select(u => new
                    {
                        idUnidad = u.IdUnidad,
                        tipoSangre = u.TipoSangre,
                        factorRh = u.FactorRh,
                        fechaIngreso = u.FechaIngreso.ToString("dd/MM/yyyy"),
                        fechaCaducidad = u.FechaCaducidad.ToString("dd/MM/yyyy"),
                        estadoUnidad = u.EstadoUnidad
                    })
                });
            }

            ViewBag.PaginaActual = pagina;
            ViewBag.TotalPaginas = totalPaginas;
            ViewBag.TotalUnidades = totalUnidades;
            return View(datosPagina);
        }




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
            var unidad = baseDatos.Inventario.buscarUnidad(idUnidad);

            if (unidad == null)
            {
                return Json(new { success = false, message = "Unidad no encontrada" });
            }

            return Json(new
            {
                success = true,
                unidad = new
                {
                    idUnidad = unidad.IdUnidad,
                    tipoSangre = unidad.TipoSangre,
                    factorRh = unidad.FactorRh,
                    fechaIngreso = unidad.FechaIngreso.ToString("dd/MM/yyyy"),
                    fechaCaducidad = unidad.FechaCaducidad.ToString("dd/MM/yyyy"),
                    cantidad = unidad.Cantidad.ToString(CultureInfo.InvariantCulture),
                    nombreDonante = unidad.Donante?.Nombre ?? "N/A",
                    apellidoDonante = unidad.Donante?.Apellido ?? "N/A",
                    dui = unidad.Donante?.Dui ?? "N/A",
                    telefono = unidad.Donante?.Telefono ?? "N/A",
                    peso = unidad.Donante?.Peso ?? 0,
                    estado = unidad.EstadoUnidad
                }
            });
        }

        [HttpGet]
        public ActionResult buscarUnidades(string termino)
        {
            IEnumerable<unidadDeSangre> lista = baseDatos.Inventario.listaUnidades();

            if (!string.IsNullOrWhiteSpace(termino))
            {
                termino = termino.Trim().ToLower();

                lista = lista.Where(u =>
                    u.IdUnidad.ToLower().Contains(termino) ||
                    ((u.Donante?.Nombre ?? string.Empty).ToLower().Contains(termino)) ||
                    ((u.Donante?.Apellido ?? string.Empty).ToLower().Contains(termino)) ||
                    ((u.Donante?.Dui ?? string.Empty).ToLower().Contains(termino)));
            }

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

            return Json(resultado);
        }

        [HttpGet]
        public ActionResult filtrarUnidades(string fecha, string estado, string tipoSangre)
        {
            IEnumerable<unidadDeSangre> listaFiltrada = baseDatos.Inventario.listaUnidades();

            if (fecha != "todos")
            {
                DateTime hoy = DateTime.Today;

                if (fecha == "hoy")
                    listaFiltrada = listaFiltrada.Where(u => u.FechaCaducidad.Date == hoy);
                else if (fecha == "7")
                    listaFiltrada = listaFiltrada.Where(u => u.FechaCaducidad.Date >= hoy.AddDays(-7) && u.FechaCaducidad.Date <= hoy);
                else if (fecha == "30")
                    listaFiltrada = listaFiltrada.Where(u => u.FechaCaducidad.Date >= hoy.AddDays(-30) && u.FechaCaducidad.Date <= hoy);
            }

            if (estado != "todos")
            {
                listaFiltrada = listaFiltrada.Where(u => u.EstadoUnidad.Equals(estado));
            }

            if (tipoSangre != "todos" && !string.IsNullOrEmpty(tipoSangre))
            {
                string grupo = tipoSangre.Substring(0, tipoSangre.Length - 1);
                string factor = tipoSangre.Substring(tipoSangre.Length - 1);

                listaFiltrada = listaFiltrada.Where(u =>
                    u.TipoSangre.Equals(grupo) &&
                    u.FactorRh == factor
                );
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

        private double ConvertirPeso(string peso)
        {
            if (string.IsNullOrWhiteSpace(peso)) return 0;

            string valorLimpio = peso
                .Replace("kg", "", StringComparison.OrdinalIgnoreCase)
                .Replace(" ", "")
                .Replace(",", ".");

            if (double.TryParse(valorLimpio, NumberStyles.Any, CultureInfo.InvariantCulture, out double resultado))
            {
                return resultado;
            }

            return 0;
        
    }

    //Funcion para depurar los viales de sangre
    [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult depurarVencidos()
        {
            int cantidad = baseDatos.DepurarVialesVencidos();

            //TempData sirve para guardar el dato, se va a utilizar en otra parte
            TempData["InventarioTipoMensaje"] = cantidad > 0 ? cantidad : 0;
            TempData["InventarioMensaje"] = cantidad > 0
                    ? $"Se depuraron {cantidad} vial(es) vencido(s) del inventario correctamente."
                    : "No se encontraron viales vencidos para depurar.";


            return RedirectToAction("obtenerListado");
        }


        [HttpGet]
        //Informacion para los contadores y colocar la informacion
        public JsonResult obtenerStasts()
        {
            var todas = baseDatos.Inventario.listaUnidades();
            return Json(
                new
                {

                    total = baseDatos.Inventario.TotalUnidades,
                    activas = todas.Count(u => u.EstadoUnidad == "Disponible"),
                    vencidas = todas.Count(u => u.EstadoUnidad == "Vencido"),
                    hoy = todas.Count(u => u.FechaIngreso.Date == DateTime.Today)
                }
                
                );
        }

    }
}
