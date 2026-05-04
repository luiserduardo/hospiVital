namespace HospiVital_App.Models
{
    public class nodoUnidadCompatible
    {
        private unidadCompatible dato;
        private nodoUnidadCompatible? siguiente;

        public nodoUnidadCompatible(unidadCompatible dato)
        {
            this.dato = dato;
            this.siguiente = null;
        }

        public unidadCompatible Dato
        {
            get => dato;
            set => dato = value;
        }

        public nodoUnidadCompatible? Siguiente
        {
            get => siguiente;
            set => siguiente = value;
        }
    }
}
