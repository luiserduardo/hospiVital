namespace HospiVital_App.Models
{
    //Clase que define estructura 
    public class nodoUnidadSangre
    {
        private unidadDeSangre dato;
        private nodoUnidadSangre sig;
        private int prioridad;


        //Para establecer la prioridad, por fehca vencimineto
        public int Prioridad { get => prioridad; set => prioridad = value; }

        //Metodos publico para acceder
        public unidadDeSangre Dato { get => dato; set => dato = value; }
        public nodoUnidadSangre Sig { get => sig; set => sig = value; }


        //Construcot, aqui asignamos la prioridad
        public nodoUnidadSangre(unidadDeSangre nuevaUnidad)
        {
            this.dato = nuevaUnidad;
            this.sig = null;
            this.prioridad = nuevaUnidad.cantidadDiasParaVencer();


        }



    }
}
