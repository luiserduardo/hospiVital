using System.ComponentModel.Design;

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

           //encolar
           cola.encolar(unidad);
            totalUnidades++;

        }

        public void editarUnidad(unidadDeSangre unidad) { 
        }

        public void retirarUnidad(unidadDeSangre unidad)
        {
            //aqui poner lo de id
        }

        public unidadDeSangre buscarUnidad(string id)
        {
            //guardar el nodo q esta al frente
            var actual = cola.obtenerFrente();

            //Hasta llegar al final
            while(actual != null) 
                {
                //del nodo comparar el id con el que estamos pasando y si es igual retornar
            if(actual.Dato.IdUnidad == id)
                    return actual.Dato;

            //pasar al siguiente
                actual = actual.Sig;

            }
            return null;
        }

        public List<unidadDeSangre> listaDisponibles() { 
        
            var lista = new List<unidadDeSangre>();

            var actual = cola.obtenerFrente();

            while (actual != null)
            {
                lista.Add(actual.Dato);
                actual = actual.Sig;
                
            }

            return lista;
        
        }


        //public List listarProximaAVencer() { 
        //}


    }
}
