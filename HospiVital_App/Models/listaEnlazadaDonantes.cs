namespace HospiVital_App.Models
{
    public class listaEnlazadaDonantes
    {
        private nodoDonante cabeza;
        private int contador;

        public listaEnlazadaDonantes()
        {
            this.cabeza = null;
            this.contador = 0;
        }

        //Método para insertar al inicio
        public void insertarInicio(Donante d)
        {
            nodoDonante nuevo = new nodoDonante(d, 0);
            nuevo.Siguiente = cabeza;
            cabeza = nuevo;
            contador++;
        }

        //Método para insertar al final
        public void insertarFinal(Donante d)
        {
            nodoDonante nuevo = new nodoDonante(d, 0);
            if (cabeza == null)
            {
                cabeza = nuevo;
            }
            else
            {
                nodoDonante actual = cabeza;
                while (actual.Siguiente != null)
                {
                    actual = actual.Siguiente;
                }
                actual.Siguiente = nuevo;
            }
            contador++;
        }

        //Método para buscar por dui
        public Donante buscarPorDui(string dui)
        {
            nodoDonante actual = cabeza;
            while (actual != null)
            {
                if (actual.Dato.Dui == dui)
                {
                    return actual.Dato;
                }
                actual = actual.Siguiente;
            }
            return null; //No encontrado
        }

        //Método para buscar por nombres
        public Donante buscarPorNombre(string nombreBusqueda)
        {
            nodoDonante actual = cabeza;

            while (actual != null)
            {
                //Mayuscula o minuscula
                if (actual.Dato.Nombre.Equals(nombreBusqueda, StringComparison.OrdinalIgnoreCase))
                {
                    return actual.Dato;
                }
                actual = actual.Siguiente;
            }

            return null; //No encontrado
        }

        //Eliminar un donante de la lista
        public bool eliminarDonante(int id)
        {
            if (cabeza == null) return false;

            if (cabeza.Dato.IdDonante == id)
            {
                cabeza = cabeza.Siguiente;
                contador--;
                return true;
            }

            nodoDonante actual = cabeza;
            while (actual.Siguiente != null && actual.Siguiente.Dato.IdDonante != id)
            {
                actual = actual.Siguiente;
            }

            if (actual.Siguiente != null)
            {
                actual.Siguiente = actual.Siguiente.Siguiente;
                contador--;
                return true;
            }

            return false;
        }

        public int Total => contador;

        public nodoDonante Cabeza => cabeza;

        public bool estaVacia()
        {
            return cabeza == null;
        }
    }
}