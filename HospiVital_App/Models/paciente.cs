namespace HospiVital_App.Models
{
    public class paciente
    {
        private int idPaciente;
        private string nombre;
        private string apellido;
        private string tipoSangreRequerido;
        private string factorRhRequerido;
        private int prioridad;

        //Constructor vacío
        public paciente()
        {
        }

        //Constructor con parametros
        public paciente(int idPaciente, string nombre, string apellido,
                        string tipoSangreRequerido, string factorRhRequerido, int prioridad)
        {
            this.IdPaciente = idPaciente;
            this.Nombre = nombre;
            this.Apellido = apellido;
            this.TipoSangreRequerido = tipoSangreRequerido;
            this.FactorRhRequerido = factorRhRequerido;
            this.Prioridad = prioridad;
        }

        //Getters y Setters
        public int IdPaciente
        {
            get => idPaciente;
            set => idPaciente = value;
        }

        public string Nombre
        {
            get => nombre;
            set => nombre = value;
        }

        public string Apellido
        {
            get => apellido;
            set => apellido = value;
        }

        public string TipoSangreRequerido
        {
            get => tipoSangreRequerido;
            set => tipoSangreRequerido = value;
        }

        public string FactorRhRequerido
        {
            get => factorRhRequerido;
            set => factorRhRequerido = value;
        }

        public int Prioridad
        {
            get => prioridad;
            set => prioridad = value;
        }
    }
}
