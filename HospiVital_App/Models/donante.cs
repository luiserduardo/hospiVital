namespace HospiVital_App.Models
{
    public class donante
    {
        private int idDonante;
        private string nombre;
        private string apellido;
        private string dui;
        private string telefono;
        private string tipoSangre;
        private string factorRh;

        //constructor 

        public donante()
        {
            
        }
        public donante(int idDonante, 
            string nombre, string apellido, 
            string dui, string telefono, 
            string tipoSangre, string factorRh)
        {
            this.IdDonante = idDonante;
            this.Nombre = nombre;
            this.Apellido = apellido;
            this.Dui = dui;
            this.Telefono = telefono;
            this.TipoSangre = tipoSangre;
            this.FactorRh = factorRh;
        }

        public int IdDonante { get => idDonante; set => idDonante = value; }
        public string Nombre { get => nombre; set => nombre = value; }
        public string Apellido { get => apellido; set => apellido = value; }
        public string Dui { get => dui; set => dui = value; }
        public string Telefono { get => telefono; set => telefono = value; }
        public string TipoSangre { get => tipoSangre; set => tipoSangre = value; }
        public string FactorRh { get => factorRh; set => factorRh = value; }
    }
}
