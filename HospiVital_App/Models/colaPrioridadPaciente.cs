namespace HospiVital_App.Models
{
    public class colaPrioridadPaciente
    {
        private nodoPaciente frente;

        //Inicializamos el valor de frente
        public colaPrioridadPaciente()
        {
            this.frente = null;
        }

        //Método para enconlar (agregar) un nuevo paciente
        public void encolar(paciente p, int prioridad)
        {
            nodoPaciente nuevoNodo = new nodoPaciente(p, prioridad);

            //Validamos que la cola no esté vacía o el nuevo paciente tenga mayor prioridad que el frente
            if (frente == null || prioridad < frente.Prioridad)
            {
                nuevoNodo.Siguiente = frente;
                frente = nuevoNodo;
            }
            else
            {
                //Buscamos la posición para el nuevo paciente entre los nodos existentes
                nodoPaciente actual = frente;
                while (actual.Siguiente != null && actual.Siguiente.Prioridad <= prioridad)
                {
                    actual = actual.Siguiente;
                }

                nuevoNodo.Siguiente = actual.Siguiente;
                actual.Siguiente = nuevoNodo;
            }
        }

        //Método para desencolar (eliminar) un paciente
        public paciente desencolar()
        {
            //Validamos que la cola no esté vacía
            if (frente == null)
            {
                return null;
            }

            paciente p = frente.Dato;
            frente = frente.Siguiente;
            return p;
        }

        //Método para obtener el primer paciente
        public paciente verPrimero()
        {
            return (frente != null) ? frente.Dato : null;
        }

        //Método para verificar que la cola no esté vacía
        public bool estaVacia()
        {
            return frente == null;
        }
    }
}
