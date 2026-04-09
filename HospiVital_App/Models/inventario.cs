namespace HospiVital_App.Models
{
    //Clase que maneja la informacion
    //unidad -> nodo-> cola->inventario
    public class inventario
    {
        private colaPrioridadViales cola;
        private int totalUnidades;

        public inventario()
        {
            cola = new colaPrioridadViales();
            totalUnidades = 0;
        }


        //metodos
        public void agregarUnidad(unidadDeSangre unidad)
        {
            //validar
            if (unidad == null)
                return;

            //creacion nodo
            nodoUnidadSangre nuevonodo = new nodoUnidadSangre(unidad);



        }

        public void editarUnidad(unidadDeSangre unidad) { 
        }

        public void retirarUnidad(unidadDeSangre unidad)
        {
            //aqui poner lo de id
        }

        public unidadDeSangre buscarUnidad(unidadDeSangre unidad)
        {

        }

        public List listaDisponibles() { 
        }

        public List listarProximaAVencer() { 
        }


    }
}
