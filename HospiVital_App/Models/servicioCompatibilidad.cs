namespace HospiVital_App.Models
{
    public class servicioCompatibilidad
    {
        //Verifica que el vial de sangre y el paciente sean compatibles
        public bool esCompatible(unidadDeSangre u, paciente p)
        {
            //Obtenemos el grupo sanguíneo
            string sangreU = u.TipoSangre.ToUpper();
            //Obtenemos el factor Rh
            string rhU = u.FactorRh;

            string sangreP = p.TipoSangreRequerido.ToUpper();
            string rhP = p.FactorRhRequerido;

            //Validar Factor Rh
            if (rhU == "+" && rhP == "-") return false;

            //Validar Grupo Sanguíneo
            switch (sangreU)
            {
                case "O":
                    return true; //Donante universal
                case "A":
                    return (sangreP == "A" || sangreP == "AB");
                case "B":
                    return (sangreP == "B" || sangreP == "AB");
                case "AB":
                    return (sangreP == "AB");
                default:
                    return false;
            }
        }

        //Filtra un grupo en específico
        public List<unidadDeSangre> filtrarCompatibles(List<unidadDeSangre> unidades, paciente p)
        {
            List<unidadDeSangre> compatibles = new List<unidadDeSangre>();

            foreach (var unidad in unidades)
            {
                //Si es compatible y no está vencida
                if (esCompatible(unidad, p) && !unidad.estaVencida())
                {
                    compatibles.Add(unidad);
                }
            }
            return compatibles;
        }
    }
}
