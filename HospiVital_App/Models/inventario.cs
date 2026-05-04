namespace HospiVital_App.Models
{
    // Clase que maneja la informacion del inventario.
    // La estructura interna sigue siendo colaPrioridadViales.
    public class inventario
    {
        private colaPrioridadViales cola;
        private int totalUnidades;

        public inventario()
        {
            cola = new colaPrioridadViales();
            totalUnidades = 0;
        }

        public int TotalUnidades => totalUnidades;

        public bool estaVacio()
        {
            return cola.estaVacia();
        }

        public nodoUnidadSangre obtenerFrente()
        {
            return cola.obtenerFrente();
        }

        public bool agregarUnidad(unidadDeSangre unidad)
        {
            if (unidad == null || string.IsNullOrWhiteSpace(unidad.IdUnidad))
                return false;

            if (buscarUnidad(unidad.IdUnidad) != null)
                return false;

            cola.encolar(unidad);
            totalUnidades++;
            return true;
        }

        public void editarUnidad(unidadDeSangre unidad)
        {
        }

        public void retirarUnidad(unidadDeSangre unidad)
        {
            if (unidad == null) return;
            retirarUnidadPorId(unidad.IdUnidad);
        }

        public bool retirarUnidadPorId(string idUnidad)
        {
            if (string.IsNullOrWhiteSpace(idUnidad) || cola.estaVacia())
                return false;

            nodoUnidadSangre actual = cola.obtenerFrente();
            nodoUnidadSangre anterior = null;

            while (actual != null)
            {
                if (actual.Dato.IdUnidad.Equals(idUnidad.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    if (anterior == null)
                    {
                        cola.desencolar();
                    }
                    else
                    {
                        anterior.Sig = actual.Sig;
                    }

                    totalUnidades--;
                    return true;
                }

                anterior = actual;
                actual = actual.Sig;
            }

            return false;
        }

        public unidadDeSangre buscarUnidad(string id)
        {
            var actual = cola.obtenerFrente();

            while (actual != null)
            {
                if (!string.IsNullOrWhiteSpace(actual.Dato.IdUnidad) &&
                    actual.Dato.IdUnidad.Equals(id.Trim(), StringComparison.OrdinalIgnoreCase))
                    return actual.Dato;

                actual = actual.Sig;
            }

            return null;
        }

        public IEnumerable<unidadDeSangre> listaDisponibles()
        {
            return listaUnidades();
        }

        // Devuelve todas las unidades registradas en la TAD colaPrioridadViales.
        // Incluye Disponibles, Vencidas y Asignadas para que el inventario funcione
        // como una base de datos interna compartida entre roles.
        public IEnumerable<unidadDeSangre> listaUnidades()
        {
            var actual = cola.obtenerFrente();

            while (actual != null)
            {
                yield return actual.Dato;
                actual = actual.Sig;
            }
        }
    }
}
