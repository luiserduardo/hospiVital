namespace HospiVital_App.Models
{
    public class listaEnlazadaVialesVencidos
    {
        private nodoVialVencido? cabeza;
        private int totalNodos;

        public listaEnlazadaVialesVencidos()
        {
            cabeza = null;
            totalNodos = 0;
        }

        public nodoVialVencido? Cabeza => cabeza;
        public int Total => totalNodos;

        public void insertarFinal(unidadDeSangre unidad)
        {
            nodoVialVencido nuevo = new nodoVialVencido(unidad);

            if (cabeza == null)
            {
                cabeza = nuevo;
                totalNodos++;
                return;
            }

            nodoVialVencido actual = cabeza;
            while (actual.Siguiente != null)
            {
                actual = actual.Siguiente;
            }

            actual.Siguiente = nuevo;
            totalNodos++;
        }
    }
}