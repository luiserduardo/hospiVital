namespace HospiVital_App.Models
{
    public class nodoPaciente
    {
        private paciente dato;
        private int prioridad;
        private nodoPaciente siguiente;

        //Constructor vacío
        public nodoPaciente()
        {
            this.dato = new paciente();
            this.prioridad = 0;
            this.siguiente = null;
        }

        //Constructor con parametros
        public nodoPaciente(paciente dato, int prioridad)
        {
            this.dato = dato;
            this.prioridad = prioridad;
            this.siguiente = null;
        }

        // Getters y Setters
        public paciente Dato
        {
            get => dato;
            set => dato = value;
        }

        public int Prioridad
        {
            get => prioridad;
            set => prioridad = value;
        }

        public nodoPaciente Siguiente
        {
            get => siguiente;
            set => siguiente = value;
        }
    }
}
