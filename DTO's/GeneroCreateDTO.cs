using back_bd.Validaciones;
using System.ComponentModel.DataAnnotations;

namespace back_bd.DTO_s
{
    public class GeneroCreateDTO
    {
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(50, ErrorMessage = "El campo {0} debe tener menos de {1} caracteres")]
        [UpperCaseFirst]
        public required string Nombre { get; set; }
    }
}
