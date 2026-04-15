namespace HospiVital_App.Models
{
    public class Donante
    {
        private int idDonante;
        private string nombre;
        private string apellido;
        private string dui;
        private string telefono;
        private double peso;
    

        //constructor 

        public Donante()
        {
            
        }


        public Donante(
           string nombre, string apellido,
           string dui, string telefono
          )
        {
           this.Nombre = nombre;
            this.Apellido = apellido;
            this.Dui = dui;
            this.Telefono = telefono;

        }
        public Donante(int idDonante, 
            string nombre, string apellido, 
            string dui, string telefono 
           )
        {
            this.IdDonante = idDonante;
            this.Nombre = nombre;
            this.Apellido = apellido;
            this.Dui = dui;
            this.Telefono = telefono;
         
        }

        public int IdDonante { get => idDonante; set => idDonante = value; }
        public string Nombre { get => nombre; set => nombre = value; }
        public string Apellido { get => apellido; set => apellido = value; }
        public string Dui { get => dui; set => dui = value; }
        public string Telefono { get => telefono; set => telefono = value; }
   
    }
}
