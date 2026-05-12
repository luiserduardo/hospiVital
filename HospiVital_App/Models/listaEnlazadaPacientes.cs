using System;

namespace HospiVital_App.Models
{
    // Lista simplemente enlazada para receptores/pacientes.
    // Sigue la lógica de Nodo + Lista de la guía: cabeza, recorrido por punteros,
    // insertar al inicio/final/posición, eliminar y ver nodo.
    public class listaEnlazadaPacientes
    {
        private nodoPaciente? cabeza;
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

        // Inserta en posición específica. La primera posición es 1.
        public void insertarPosicion(paciente p, int posicion)
        {
            if (posicion <= 1 || cabeza == null)
            {
                insertarInicio(p);
                return;
            }

            if (posicion > contador)
            {
                insertarFinal(p);
                return;
            }

            nodoPaciente nuevo = new nodoPaciente(p);
            nodoPaciente actual = cabeza;

            for (int i = 1; i < posicion - 1 && actual.Siguiente != null; i++)
            {
                actual = actual.Siguiente;
            }

            nuevo.Siguiente = actual.Siguiente;
            actual.Siguiente = nuevo;
            contador++;
        }

        public nodoPaciente? eliminarInicio()
        {
            if (cabeza == null) return null;

            nodoPaciente eliminado = cabeza;
            cabeza = cabeza.Siguiente;
            eliminado.Siguiente = null;
            contador--;
            return eliminado;
        }

        public nodoPaciente? eliminarFinal()
        {
            if (cabeza == null) return null;

            if (cabeza.Siguiente == null)
            {
                return eliminarInicio();
            }

            nodoPaciente anterior = cabeza;
            nodoPaciente actual = cabeza.Siguiente;

            while (actual.Siguiente != null)
            {
                anterior = actual;
                actual = actual.Siguiente;
            }

            anterior.Siguiente = null;
            contador--;
            return actual;
        }

        public nodoPaciente? eliminarPosicion(int posicion)
        {
            if (cabeza == null || posicion < 1 || posicion > contador) return null;

            if (posicion == 1)
            {
                return eliminarInicio();
            }

            nodoPaciente anterior = cabeza;
            nodoPaciente actual = cabeza;

            for (int i = 1; i < posicion; i++)
            {
                anterior = actual;
                actual = actual.Siguiente!;
            }

            anterior.Siguiente = actual.Siguiente;
            actual.Siguiente = null;
            contador--;
            return actual;
        }

        public nodoPaciente? verNodo(int posicion)
        {
            if (posicion < 1 || posicion > contador) return null;

            nodoPaciente? actual = cabeza;

            for (int i = 1; i < posicion && actual != null; i++)
            {
                actual = actual.Siguiente;
            }

            return actual;
        }

        //Buscar paciente por ID
        public paciente? buscarPorId(int id)
        {
            nodoPaciente? actual = cabeza;

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

        //Buscar paciente por DUI
        public paciente? buscarPorDui(string dui)
        {
            nodoPaciente? actual = cabeza;

            while (actual != null)
            {
                if (actual.Dato.Dui.Equals(dui, StringComparison.OrdinalIgnoreCase))
                {
                    return actual.Dato;
                }

                actual = actual.Siguiente;
            }

            return null;
        }

        //Buscar paciente por nombre
        public paciente? buscarPorNombre(string nombreBusqueda)
        {
            nodoPaciente? actual = cabeza;

            while (actual != null)
            {
                if (actual.Dato.Nombre.Equals(nombreBusqueda, StringComparison.OrdinalIgnoreCase))
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

        public IEnumerable<paciente> ComoEnumerable()
        {
            nodoPaciente? actual = cabeza;

            while (actual != null)
            {
                yield return actual.Dato;
                actual = actual.Siguiente;
            }
        }

        public int Total => contador;
        public nodoPaciente? Cabeza => cabeza;

        public bool estaVacia()
        {
            return cabeza == null;
        }
    }
}
