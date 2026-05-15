namespace HospiVital_App.Models
{
    public class registroSalidaVialVencido
    {
        public registroSalidaVialVencido(unidadDeSangre vial, DateTime fechaSalida, string responsable)
        {
            Vial = vial;
            FechaSalida = fechaSalida;
            Responsable = responsable;
        }

        public unidadDeSangre Vial { get; }
        public DateTime FechaSalida { get; }
        public string Responsable { get; }
    }
}
