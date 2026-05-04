namespace HospiVital_App.Models
{
    // Lista simplemente enlazada para administrar usuarios sin listas genéricas ni SQL.
    public class listaEnlazadaUsuarios
    {
        private nodoUsuario? cabeza;
        private int totalNodos;

        public listaEnlazadaUsuarios()
        {
            cabeza = null;
            totalNodos = 0;
        }

        public int Total => totalNodos;
        public nodoUsuario? Cabeza => cabeza;

        public bool estaVacia()
        {
            return cabeza == null;
        }

        public void insertarFinal(AppUser usuario)
        {
            nodoUsuario nuevo = new nodoUsuario(usuario);

            if (cabeza == null)
            {
                cabeza = nuevo;
            }
            else
            {
                nodoUsuario actual = cabeza;

                while (actual.Siguiente != null)
                {
                    actual = actual.Siguiente;
                }

                actual.Siguiente = nuevo;
            }

            totalNodos++;
        }

        public AppUser? buscarPorUsuario(string usuario)
        {
            nodoUsuario? actual = cabeza;

            while (actual != null)
            {
                if (actual.Dato.Usuario.Equals(usuario, StringComparison.OrdinalIgnoreCase))
                {
                    return actual.Dato;
                }

                actual = actual.Siguiente;
            }

            return null;
        }

        public AppUser? validarCredenciales(string usuario, string contrasena)
        {
            nodoUsuario? actual = cabeza;

            while (actual != null)
            {
                if (actual.Dato.Usuario.Equals(usuario, StringComparison.OrdinalIgnoreCase) &&
                    actual.Dato.Contrasena == contrasena)
                {
                    return actual.Dato;
                }

                actual = actual.Siguiente;
            }

            return null;
        }

        public bool actualizar(string usuarioOriginal, AppUser actualizado)
        {
            AppUser? existente = buscarPorUsuario(usuarioOriginal);

            if (existente == null)
            {
                return false;
            }

            bool esAdministradorPrincipal = existente.EsAdministradorPrincipal;

            existente.Usuario = actualizado.Usuario;
            existente.Contrasena = actualizado.Contrasena;
            existente.Nombre = actualizado.Nombre;
            existente.Rol = actualizado.Rol;
            existente.EsAdministradorPrincipal = esAdministradorPrincipal;
            return true;
        }

        public bool eliminar(string usuario)
        {
            if (cabeza == null) return false;

            if (cabeza.Dato.Usuario.Equals(usuario, StringComparison.OrdinalIgnoreCase))
            {
                cabeza = cabeza.Siguiente;
                totalNodos--;
                return true;
            }

            nodoUsuario actual = cabeza;

            while (actual.Siguiente != null &&
                   !actual.Siguiente.Dato.Usuario.Equals(usuario, StringComparison.OrdinalIgnoreCase))
            {
                actual = actual.Siguiente;
            }

            if (actual.Siguiente != null)
            {
                actual.Siguiente = actual.Siguiente.Siguiente;
                totalNodos--;
                return true;
            }

            return false;
        }

        public int contarPorRol(string rol)
        {
            int contador = 0;
            nodoUsuario? actual = cabeza;

            while (actual != null)
            {
                if (actual.Dato.Rol.Equals(rol, StringComparison.OrdinalIgnoreCase))
                {
                    contador++;
                }

                actual = actual.Siguiente;
            }

            return contador;
        }

        public IEnumerable<AppUser> ComoEnumerable()
        {
            nodoUsuario? actual = cabeza;

            while (actual != null)
            {
                yield return actual.Dato;
                actual = actual.Siguiente;
            }
        }
    }
}
