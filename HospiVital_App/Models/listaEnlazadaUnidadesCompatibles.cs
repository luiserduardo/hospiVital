namespace HospiVital_App.Models
{
    public class listaEnlazadaUnidadesCompatibles
    {
        private nodoUnidadCompatible? cabeza;
        private int totalNodos;

        public listaEnlazadaUnidadesCompatibles()
        {
            cabeza = null;
            totalNodos = 0;
        }

        public int Total => totalNodos;
        public nodoUnidadCompatible? Cabeza => cabeza;

        public bool estaVacia()
        {
            return cabeza == null;
        }

        public void insertarFinal(unidadCompatible item)
        {
            nodoUnidadCompatible nuevo = new nodoUnidadCompatible(item);

            if (cabeza == null)
            {
                cabeza = nuevo;
            }
            else
            {
                nodoUnidadCompatible actual = cabeza;

                while (actual.Siguiente != null)
                {
                    actual = actual.Siguiente;
                }

                actual.Siguiente = nuevo;
            }

            totalNodos++;
        }
    }
}
