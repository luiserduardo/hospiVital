namespace HospiVital_App.Models
{

    //Clase que solo definimos datos
    public class unidadDeSangre
    {
        //Atributos segun diagrama
        private int idUnidad;
        private string tipoSangre;
        private string factorRh;
        private DateTime fechaIngreso;
        private DateTime fechaCaducidad;
        private string estadoUnidad;
        private double cantidad;

        //vial contiene info de donante
        //falta agregar eso

        //Constructuor iniacial

        public unidadDeSangre()
        {
            
        }
        public unidadDeSangre(int idUnidad, 
           string tipoSangre, 
            string factorRh, DateTime fechaIngreso, 
            DateTime fechaCaducidad, string estadoUnidad, double cantidad)
        {
            this.IdUnidad = idUnidad;
            this.TipoSangre = tipoSangre;
            this.FactorRh = factorRh;
            this.FechaIngreso = fechaIngreso;
            this.FechaCaducidad = fechaCaducidad;
            this.EstadoUnidad = estadoUnidad;
            this.Cantidad = cantidad;
        }

        //Metodos para acceder de manera publica
        public int IdUnidad { get => idUnidad; set => idUnidad = value; }
        public string TipoSangre { get => tipoSangre; set => tipoSangre = value; }
        public string FactorRh { get => factorRh; set => factorRh = value; }
        public DateTime FechaIngreso { get => fechaIngreso; set => fechaIngreso = value; }
        public DateTime FechaCaducidad { get => fechaCaducidad; set => fechaCaducidad = value; }
        public string EstadoUnidad { get => estadoUnidad; set => estadoUnidad = value; }
        public double Cantidad { get => cantidad; set => cantidad = value; }


       //metodo para ver si esta vencido vial de sangre
       public bool estaVencida()
        {

            //comprobar si fecha ingreso mayorr o igual a 3 dias antes de fecha 
            return DateTime.Now >= FechaCaducidad;
        }

        public int cantidadDiasParaVencer()
        {
            //Le agregamos 3 dias porque es un recurso que hay que tener mucho cuidad, no es como la comidad que si la pones el mismo dia no pasa nada
            return (FechaCaducidad - DateTime.Now.AddDays(3)).Days;

        }

    }
}
