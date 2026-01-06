using System.ComponentModel.DataAnnotations;

namespace back_bd.DTO_s.Usuarios
{
    public class UsuarioUpdateDTO
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100)]
        public required string Nombre { get; set; }

        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "El formato del correo no es válido")]
        [StringLength(100)]
        public required string Email { get; set; }

        [Required(ErrorMessage = "El rol es obligatorio")]
        public int RolId { get; set; }

        [Required(ErrorMessage = "El plan es obligatorio")]
        public int PlanId { get; set; }

        // Opcional: solo se actualiza si se envía
        public string? Password { get; set; }
    }
}