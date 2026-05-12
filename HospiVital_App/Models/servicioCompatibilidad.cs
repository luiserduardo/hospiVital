namespace HospiVital_App.Models
{
    public class servicioCompatibilidad
    {
        // Verifica que el vial tenga exactamente el mismo grupo sanguíneo y factor Rh requerido.
        // Ejemplo: receptor AB+ muestra/asigna únicamente unidades AB+.
        public bool esCompatible(unidadDeSangre u, paciente p)
        {
            return esCompatible(u, p.TipoSangreRequerido, p.FactorRhRequerido);
        }

        public bool esCompatible(unidadDeSangre u, string tipoSangreReceptor, string factorRhReceptor)
        {
            if (u == null)
            {
                return false;
            }

            string sangreUnidad = (u.TipoSangre ?? string.Empty).Trim().ToUpper();
            string rhUnidad = (u.FactorRh ?? string.Empty).Trim();
            string sangreReceptor = (tipoSangreReceptor ?? string.Empty).Trim().ToUpper();
            string rhReceptor = (factorRhReceptor ?? string.Empty).Trim();

            return sangreUnidad.Equals(sangreReceptor, StringComparison.OrdinalIgnoreCase) &&
                   rhUnidad == rhReceptor;
        }

        // Filtra unidades recorriendo nodos enlazados, sin List<T>.
        // La coincidencia es exacta por grupo sanguíneo y factor Rh.
        public listaEnlazadaUnidadesCompatibles filtrarCompatibles(
            nodoUnidadSangre? inicio,
            string tipoSangreReceptor,
            string factorRhReceptor,
            listaEnlazadaReceptorAsignacion? asignacionesRealizadas = null)
        {
            listaEnlazadaUnidadesCompatibles resultado = new listaEnlazadaUnidadesCompatibles();
            nodoUnidadSangre? actual = inicio;

            while (actual != null)
            {
                unidadDeSangre unidad = actual.Dato;

                if (unidad != null &&
                    esCompatible(unidad, tipoSangreReceptor, factorRhReceptor) &&
                    !unidad.estaVencida() &&
                    unidad.EstadoUnidad == "Disponible" &&
                    (asignacionesRealizadas == null || !asignacionesRealizadas.existeUnidadAsignada(unidad.IdUnidad)))
                {
                    resultado.insertarFinal(new unidadCompatible
                    {
                        IdVial = unidad.IdUnidad,
                        Tipo = unidad.TipoSangre + unidad.FactorRh,
                        Ubicacion = "Nevera A1-Estante 1",
                        Donante = unidad.Donante != null
                            ? unidad.Donante.Nombre + " " + unidad.Donante.Apellido
                            : "Sin registrar"
                    });
                }

                actual = actual.Sig;
            }

            return resultado;
        }
    }
}
