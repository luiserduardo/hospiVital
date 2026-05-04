namespace HospiVital_App.Models
{
    // Nodo reservado para el futuro manejo de la lista de viales vencidos.
    public class nodoVialVencido
    {
        private unidadDeSangre? dato;
        private nodoVialVencido? siguiente;

        public nodoVialVencido()
        {
            dato = null;
            siguiente = null;
        }

        public nodoVialVencido(unidadDeSangre dato)
        {
            this.dato = dato;
            this.siguiente = null;
        }

        public unidadDeSangre? Dato
        {
            get => dato;
            set => dato = value;
        }

        public nodoVialVencido? Siguiente
        {
            get => siguiente;
            set => siguiente = value;
        }
    }
}
