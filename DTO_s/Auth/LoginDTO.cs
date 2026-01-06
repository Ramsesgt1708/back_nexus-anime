using System.ComponentModel.DataAnnotations;

namespace back_bd.DTO_s.Auth
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "El email es requerido")]
        [EmailAddress]
        public required string Email { get; set; }

        [Required(ErrorMessage = "La contraseña es requerida")]
        public required string Password { get; set; }
    }
}