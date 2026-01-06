using System.ComponentModel.DataAnnotations;

namespace back_bd.DTO_s.Auth
{
    public class RegisterDTO
    {
        [Required]
        [StringLength(100)]
        public required string Nombre { get; set; }

        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        [MinLength(6)]
        public required string Password { get; set; }

        [Required]
        public int PlanId { get; set; }
    }
}