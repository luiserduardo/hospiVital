namespace HospiVital_App.Models
{
    public class salidasVialesVencidosViewModel
    {
        public IEnumerable<unidadDeSangre> VialesPendientes { get; set; } = [];
        public IEnumerable<registroSalidaVialVencido> RegistrosSalida { get; set; } = [];

        public int TotalPendientes { get; set; }
        public int TotalSalidas { get; set; }
        public registroSalidaVialVencido? UltimaSalida { get; set; }

        // Paginación — Pendientes
        public int PaginaPendientesActual { get; set; } = 1;
        public int TotalPaginasPendientes { get; set; } = 1;

        // Paginación — Salidas registradas
        public int PaginaSalidasActual { get; set; } = 1;
        public int TotalPaginasSalidas { get; set; } = 1;

        public int ElementosPorPagina { get; set; } = 5;
    }
}