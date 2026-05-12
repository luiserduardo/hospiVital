namespace HospiVital_App.Models
{
    // Lista simplemente enlazada para los registros de receptores/asignaciones.
    // Implementa cabeza, recorrido por punteros e inserción al final según la guía.
    public class listaEnlazadaReceptorAsignacion
    {
        private nodoReceptorAsignacion? cabeza;
        private int totalNodos;

        public listaEnlazadaReceptorAsignacion()
        {
            cabeza = null;
            totalNodos = 0;
        }

        public int Total => totalNodos;
        public nodoReceptorAsignacion? Cabeza => cabeza;

        public bool estaVacia()
        {
            return cabeza == null;
        }

        public void insertarFinal(receptorAsignacion item)
        {
            nodoReceptorAsignacion nuevo = new nodoReceptorAsignacion(item);

            if (cabeza == null)
            {
                cabeza = nuevo;
            }
            else
            {
                nodoReceptorAsignacion actual = cabeza;

                while (actual.Siguiente != null)
                {
                    actual = actual.Siguiente;
                }

                actual.Siguiente = nuevo;
            }

            totalNodos++;
        }

        public receptorAsignacion? buscarPorId(string idReceptor)
        {
            nodoReceptorAsignacion? actual = cabeza;

            while (actual != null)
            {
                if (actual.Dato.IdReceptor.Equals(idReceptor, StringComparison.OrdinalIgnoreCase))
                {
                    return actual.Dato;
                }

                actual = actual.Siguiente;
            }

            return null;
        }


        public bool existeUnidadAsignada(string idUnidad)
        {
            if (string.IsNullOrWhiteSpace(idUnidad))
            {
                return false;
            }

            nodoReceptorAsignacion? actual = cabeza;
            string idBuscado = idUnidad.Trim();

            while (actual != null)
            {
                receptorAsignacion item = actual.Dato;

                if (item != null &&
                    item.Estado.Equals("Completado", StringComparison.OrdinalIgnoreCase) &&
                    !string.IsNullOrWhiteSpace(item.UnidadAsignada) &&
                    !item.UnidadAsignada.Equals("Pendiente", StringComparison.OrdinalIgnoreCase) &&
                    item.UnidadAsignada.Equals(idBuscado, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }

                actual = actual.Siguiente;
            }

            return false;
        }

        public int contarPorEstado(string estado)
        {
            int contador = 0;
            nodoReceptorAsignacion? actual = cabeza;

            while (actual != null)
            {
                if (actual.Dato.Estado.Equals(estado, StringComparison.OrdinalIgnoreCase))
                {
                    contador++;
                }

                actual = actual.Siguiente;
            }

            return contador;
        }

        public listaEnlazadaReceptorAsignacion filtrar(
            string? busqueda,
            string? estado,
            string? tipoSangre,
            string? factorRh,
            string? filtroFecha)
        {
            listaEnlazadaReceptorAsignacion resultado = new listaEnlazadaReceptorAsignacion();
            nodoReceptorAsignacion? actual = cabeza;

            string textoBusqueda = (busqueda ?? string.Empty).Trim().ToLower();
            string estadoFiltro = (estado ?? "all").Trim();
            string tipoFiltro = (tipoSangre ?? "all").Trim().ToUpper();
            string rhFiltro = (factorRh ?? "all").Trim();
            string fechaFiltro = (filtroFecha ?? "all").Trim();

            while (actual != null)
            {
                receptorAsignacion item = actual.Dato;

                if (coincideBusqueda(item, textoBusqueda) &&
                    coincideEstado(item, estadoFiltro) &&
                    coincideTipoSangre(item, tipoFiltro) &&
                    coincideFactorRh(item, rhFiltro) &&
                    coincideFecha(item, fechaFiltro))
                {
                    resultado.insertarFinal(item);
                }

                actual = actual.Siguiente;
            }

            return resultado;
        }

        private bool coincideBusqueda(receptorAsignacion item, string busqueda)
        {
            if (string.IsNullOrWhiteSpace(busqueda))
            {
                return true;
            }

            return item.IdReceptor.ToLower().Contains(busqueda) ||
                   item.Beneficiario.ToLower().Contains(busqueda) ||
                   item.DuiReceptor.ToLower().Contains(busqueda) ||
                   item.UnidadAsignada.ToLower().Contains(busqueda) ||
                   item.Donante.ToLower().Contains(busqueda);
        }

        private bool coincideEstado(receptorAsignacion item, string estado)
        {
            return estado.Equals("all", StringComparison.OrdinalIgnoreCase) ||
                   item.Estado.Equals(estado, StringComparison.OrdinalIgnoreCase);
        }

        private bool coincideTipoSangre(receptorAsignacion item, string tipoSangre)
        {
            if (tipoSangre.Equals("all", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            string tipoItem = obtenerGrupoSanguineo(item.TipoSangre);
            return tipoItem.Equals(tipoSangre, StringComparison.OrdinalIgnoreCase);
        }

        private bool coincideFactorRh(receptorAsignacion item, string factorRh)
        {
            if (factorRh.Equals("all", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            string rhItem = obtenerFactorRh(item.TipoSangre);
            return rhItem == factorRh;
        }

        private bool coincideFecha(receptorAsignacion item, string filtroFecha)
        {
            if (filtroFecha.Equals("all", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (!item.FechaTransfusionValor.HasValue)
            {
                return false;
            }

            DateTime fecha = item.FechaTransfusionValor.Value.Date;
            DateTime hoy = DateTime.Today;

            return filtroFecha switch
            {
                "today" => fecha == hoy,
                "last7" => fecha >= hoy.AddDays(-7) && fecha <= hoy,
                "last30" => fecha >= hoy.AddDays(-30) && fecha <= hoy,
                _ => true
            };
        }

        private string obtenerGrupoSanguineo(string tipoCompleto)
        {
            if (string.IsNullOrWhiteSpace(tipoCompleto))
            {
                return string.Empty;
            }

            string valor = tipoCompleto.Trim().ToUpper();
            return valor.EndsWith("+") || valor.EndsWith("-")
                ? valor[..^1]
                : valor;
        }

        private string obtenerFactorRh(string tipoCompleto)
        {
            if (string.IsNullOrWhiteSpace(tipoCompleto))
            {
                return string.Empty;
            }

            string valor = tipoCompleto.Trim();
            return valor.EndsWith("+") || valor.EndsWith("-")
                ? valor[^1..]
                : string.Empty;
        }
    }
}
