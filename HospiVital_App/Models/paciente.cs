namespace HospiVital_App.Models
{
    public class paciente
    {
        private int idPaciente;
        private string dui = string.Empty;
        private string nombre = string.Empty;
        private string apellido = string.Empty;
        private string tipoSangreRequerido = string.Empty;
        private string factorRhRequerido = string.Empty;
        //Para genera el id
        private static int ultimoId = 1000;

        //Constructor vacío
        public paciente()
        {
            this.idPaciente = GenerarNuevoId();
        }

        //Constructor con parametros
        public paciente(string nombre, string apellido,
                        string tipoSangreRequerido, string factorRhRequerido)
        {
            this.IdPaciente = GenerarNuevoId();
            this.Nombre = nombre;
            this.Apellido = apellido;
            this.TipoSangreRequerido = tipoSangreRequerido;
            this.FactorRhRequerido = factorRhRequerido;
        }

        public paciente(string dui, string nombre, string apellido,
                        string tipoSangreRequerido, string factorRhRequerido)
            : this(nombre, apellido, tipoSangreRequerido, factorRhRequerido)
        {
            this.Dui = dui;
        }

        //Genera un id único automaticamente 
        private static int GenerarNuevoId()
        {
            ultimoId++;
            return ultimoId;
        }

        //Getters y Setters
        public int IdPaciente
        {
            get => idPaciente;
            set => idPaciente = value;
        }

        public string Dui
        {
            get => dui;
            set => dui = value ?? string.Empty;
        }

        public string Nombre
        {
            get => nombre;
            set => nombre = value ?? string.Empty;
        }

        public string Apellido
        {
            get => apellido;
            set => apellido = value ?? string.Empty;
        }

        public string TipoSangreRequerido
        {
            get => tipoSangreRequerido;
            set => tipoSangreRequerido = value ?? string.Empty;
        }

        public string FactorRhRequerido
        {
            get => factorRhRequerido;
            set => factorRhRequerido = value ?? string.Empty;
        }
    }
}
