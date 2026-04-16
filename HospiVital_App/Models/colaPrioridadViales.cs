namespace HospiVital_App.Models
{
    //Clase que definie el orden por prioridad
    public class colaPrioridadViales
    {
        private nodoUnidadSangre frente;

        // Ver si está vacía
        public bool estaVacia()
        {
            return frente == null;
        }

        public void encolar(unidadDeSangre nuevaUnidad)
        {
            nodoUnidadSangre nuevo = new nodoUnidadSangre(nuevaUnidad);

            //cola vacía
            if (frente == null)
            {
                frente = nuevo;
                return;
            }

            // va al inicio el mayor 
            if (nuevo.Prioridad < frente.Prioridad)
            {
                nuevo.Sig = frente;
                frente = nuevo;
                return;
            }

            // insertar en medio o final
            nodoUnidadSangre actual = frente;

            while (actual.Sig != null && actual.Sig.Prioridad <= nuevo.Prioridad)
            {
                actual = actual.Sig;
            }

            nuevo.Sig = actual.Sig;
            actual.Sig = nuevo;
        }

        // elimina el primero
        public unidadDeSangre desencolar()
        {
            if (estaVacia())
                return null;

            unidadDeSangre dato = frente.Dato;
            frente = frente.Sig;

            return dato;
        }

        // sin eliminar
        public unidadDeSangre verPrimero()
        {
            if (estaVacia())
                return null;

            return frente.Dato;
        }

        //Para acceder al frente
        public nodoUnidadSangre obtenerFrente()
        {
            return frente;
        }

//metodo para ver la info, tomar en cuenta por si mas adelante es necesario
    }
}