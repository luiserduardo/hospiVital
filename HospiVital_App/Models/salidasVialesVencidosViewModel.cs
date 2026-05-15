namespace HospiVital_App.Models
{
    public class salidasVialesVencidosViewModel
    {
        public IEnumerable<unidadDeSangre> VialesPendientes { get; set; } = Enumerable.Empty<unidadDeSangre>();
        public IEnumerable<registroSalidaVialVencido> RegistrosSalida { get; set; } = Enumerable.Empty<registroSalidaVialVencido>();
        public int TotalPendientes { get; set; }
        public int TotalSalidas { get; set; }
        public registroSalidaVialVencido? UltimaSalida { get; set; }
    }
}
