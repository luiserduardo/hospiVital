namespace HospiVital_App.Models
{
    public class pilaSalidasVialesVencidos
    {
        private readonly Stack<registroSalidaVialVencido> registros;

        public pilaSalidasVialesVencidos()
        {
            registros = new Stack<registroSalidaVialVencido>();
        }

        public int Total => registros.Count;

        public bool estaVacia()
        {
            return registros.Count == 0;
        }

        public bool apilar(registroSalidaVialVencido registro)
        {
            if (registro == null || registro.Vial == null || string.IsNullOrWhiteSpace(registro.Vial.IdUnidad))
            {
                return false;
            }

            if (existeRegistro(registro.Vial.IdUnidad))
            {
                return false;
            }

            registros.Push(registro);
            return true;
        }

        public registroSalidaVialVencido? consultarTope()
        {
            return registros.Count == 0 ? null : registros.Peek();
        }

        public bool existeRegistro(string idUnidad)
        {
            if (string.IsNullOrWhiteSpace(idUnidad))
            {
                return false;
            }

            string id = idUnidad.Trim();

            foreach (registroSalidaVialVencido registro in registros)
            {
                if (registro.Vial.IdUnidad.Equals(id, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        public IEnumerable<registroSalidaVialVencido> obtenerRegistros()
        {
            foreach (registroSalidaVialVencido registro in registros)
            {
                yield return registro;
            }
        }
    }
}
