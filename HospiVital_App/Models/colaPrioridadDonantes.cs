namespace HospiVital_App.Models
{
    public class colaPrioridadDonantes
    {
        private nodoDonante frente;

        //Constructor
        public colaPrioridadDonantes()
        {
            this.frente = null;
        }

        // Verifica si la cola está vacía
        public bool estaVacia()
        {
            return frente == null;
        }

        //Método para encolar
        public void Encolar(Donante nuevoDonante, int prioridad)
        {
            nodoDonante nuevoNodo = new nodoDonante(nuevoDonante, prioridad);

            //Validamos que no este vacío y si el nodo ingresado tiene mayor prioridad que el de frente
            if (estaVacia() || prioridad < frente.Prioridad)
            {
                nuevoNodo.Siguiente = frente;
                frente = nuevoNodo;
            }
            else
            {
                //Buscamos la posiciones que le corresponda segun su prioridad
                nodoDonante actual = frente;

                while (actual.Siguiente != null && actual.Siguiente.Prioridad <= prioridad)
                {
                    actual = actual.Siguiente;
                }

                nuevoNodo.Siguiente = actual.Siguiente;
                actual.Siguiente = nuevoNodo;
            }
        }

        //Método para desencolar
        public Donante Desencolar()
        {
            //Validamos que no este vacía
            if (estaVacia())
            {
                return null;
            }

            Donante dato = frente.Dato;
            frente = frente.Siguiente;
            return dato;
        }

        //Método para ver el siguiente donante
        public Donante VerFrente()
        {
            return estaVacia() ? null : frente.Dato;
        }
    }
}
