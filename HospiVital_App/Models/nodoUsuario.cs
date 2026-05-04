namespace HospiVital_App.Models
{
    public class nodoUsuario
    {
        private AppUser dato;
        private nodoUsuario? siguiente;

        public nodoUsuario(AppUser dato)
        {
            this.dato = dato;
            this.siguiente = null;
        }

        public AppUser Dato
        {
            get => dato;
            set => dato = value;
        }

        public nodoUsuario? Siguiente
        {
            get => siguiente;
            set => siguiente = value;
        }
    }
}
