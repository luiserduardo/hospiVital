namespace HospiVital_App.Models
{
    public class paciente
    {
        private int idPaciente;
        private string nombre;
        private string apellido;
        private string tipoSangreRequerido;
        private string factorRhRequerido;
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

    }
}
