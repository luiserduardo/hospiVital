namespace HospiVital_App.Models
{
    public class nodoPaciente
    {
        private paciente dato;
        private nodoPaciente? siguiente;

        //Constructor vacío
        public nodoPaciente()
        {
            this.dato = new paciente();
            this.siguiente = null;
        }

        //Constructor con parametros
        public nodoPaciente(paciente dato)
        {
            this.dato = dato;
            this.siguiente = null;
        }

        // Getters y Setters
        public paciente Dato
        {
            get => dato;
            set => dato = value;
        }

        public nodoPaciente? Siguiente
        {
            get => siguiente;
            set => siguiente = value;
        }
    }
}
