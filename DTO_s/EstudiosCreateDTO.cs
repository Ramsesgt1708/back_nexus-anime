using System.ComponentModel.DataAnnotations;

namespace back_bd.DTO_s
{
    public class EstudiosCreateDTO
    {
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(50, ErrorMessage = "El campo {0} debe tener menos de {1} caracteres")]
        public required string Nombre { get; set; }
    }
}
