using System;

namespace HospiVital_App.Models
{
    public class listaEnlazadaPacientes
    {
        private nodoPaciente cabeza;
        private int contador;

        public listaEnlazadaPacientes()
        {
            this.cabeza = null;
            this.contador = 0;
        }

        //Método para insertar al inicio
        public void insertarInicio(paciente p)
        {
            nodoPaciente nuevo = new nodoPaciente(p);
            nuevo.Siguiente = cabeza;
            cabeza = nuevo;
            contador++;
        }

        //Método para insertar al final
        public void insertarFinal(paciente p)
        {
            nodoPaciente nuevo = new nodoPaciente(p);
            if (cabeza == null)
            {
                cabeza = nuevo;
            }
            else
            {
                nodoPaciente actual = cabeza;
                while (actual.Siguiente != null)
                {
                    actual = actual.Siguiente;
                }
                actual.Siguiente = nuevo;
            }
            contador++;
        }

        //Buscar paciente por ID
        public paciente buscarPorId(int id)
        {
            nodoPaciente actual = cabeza;
            while (actual != null)
            {
                if (actual.Dato.IdPaciente == id)
                {
                    return actual.Dato;
                }
                actual = actual.Siguiente;
            }
            return null;
        }

        //Buscar paciente por nombre
        public paciente buscarPorNombre(string nombreBusqueda)
        {
            nodoPaciente actual = cabeza;
            while (actual != null)
            {
                if (actual.Dato.Nombre != null &&
                    actual.Dato.Nombre.Equals(nombreBusqueda, StringComparison.OrdinalIgnoreCase))
                {
                    return actual.Dato;
                }
                actual = actual.Siguiente;
            }
            return null;
        }

        //Eliminar un paciente por ID
        public bool eliminarPaciente(int id)
        {
            if (cabeza == null) return false;

            // Si es la cabeza
            if (cabeza.Dato.IdPaciente == id)
            {
                cabeza = cabeza.Siguiente;
                contador--;
                return true;
            }

            nodoPaciente actual = cabeza;
            while (actual.Siguiente != null && actual.Siguiente.Dato.IdPaciente != id)
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
        public nodoPaciente Cabeza => cabeza;

        public bool estaVacia()
        {
            return cabeza == null;
        }
    }
}