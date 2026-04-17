using System.ComponentModel.DataAnnotations;

namespace HospiVital_App.Models
{
    /// <summary>
    /// formulario de login con validaciones
    /// </summary>
    public class LoginViewModel
    {
        [Required(ErrorMessage = "El nombre de usuario es obligatorio")]
        [Display(Name = "Nombre de Usuario")]
        public string Username { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }
    }

    /// <summary>
    /// Modelo de usuario para el sistema de autenticacion
    /// </summary>
    public class Usuario
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string NombreCompleto { get; set; }
        public RolUsuario Rol { get; set; }
    }

    /// <summary>
    /// Enum para los roles de usuario del sistema
    /// </summary>
    public enum RolUsuario
    {
        Administrador,
        Medico,
        TecnicoMedico
    }
}
