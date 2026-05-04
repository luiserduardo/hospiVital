namespace HospiVital_App.Models
{
    // Registro que se muestra en la pantalla de receptores.
    // Se almacena dentro de una lista simplemente enlazada, no en listas genéricas.
    public class receptorAsignacion
    {
        public string IdReceptor { get; set; } = string.Empty;
        public int IdPaciente { get; set; }
        public string DuiReceptor { get; set; } = string.Empty;
        public string Beneficiario { get; set; } = string.Empty;
        public string Nombres { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
        public string TipoSangre { get; set; } = string.Empty;
        public string Donante { get; set; } = string.Empty;
        public string DonanteDui { get; set; } = string.Empty;
        public string DonanteTelefono { get; set; } = string.Empty;
        public string UnidadAsignada { get; set; } = string.Empty;
        public string FechaTransfusion { get; set; } = string.Empty;
        public DateTime? FechaTransfusionValor { get; set; }
        public string Estado { get; set; } = "Pendiente";
    }
}
