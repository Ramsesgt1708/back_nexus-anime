using System.ComponentModel.DataAnnotations;

namespace back_bd.DTO_s
{
    public class RolesCreateDTO
    {
        [Required]
        [StringLength(50)]
        public required string Nombre { get; set; }
    }
}