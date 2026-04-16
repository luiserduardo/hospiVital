namespace HospiVital_App.Models
{
    public class nodoDonante
    {
        //Parametros
        private Donante dato;
        private int prioridad;
        private nodoDonante siguiente;

        //Constructor 
        public nodoDonante()
        {
            this.dato = new Donante();
            this.prioridad = 0;
            this.siguiente = null;
        }

        //Constructor con datos
        public nodoDonante(Donante dato, int prioridad)
        {
            this.dato = dato;
            this.prioridad = prioridad;
            this.siguiente = null;
        }

        //Getters y Setters
        public Donante Dato
        {
            get => dato;
            set => dato = value;
        }

        public int Prioridad
        {
            get => prioridad;
            set => prioridad = value;
        }

        public nodoDonante Siguiente
        {
            get => siguiente;
            set => siguiente = value;
        }

    }
}
