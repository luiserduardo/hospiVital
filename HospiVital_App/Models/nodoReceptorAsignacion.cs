namespace HospiVital_App.Models
{
    public class nodoReceptorAsignacion
    {
        private receptorAsignacion dato;
        private nodoReceptorAsignacion? siguiente;

        public nodoReceptorAsignacion(receptorAsignacion dato)
        {
            this.dato = dato;
            this.siguiente = null;
        }

        public receptorAsignacion Dato
        {
            get => dato;
            set => dato = value;
        }

        public nodoReceptorAsignacion? Siguiente
        {
            get => siguiente;
            set => siguiente = value;
        }
    }
}
