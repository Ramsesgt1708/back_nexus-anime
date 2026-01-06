using System.ComponentModel.DataAnnotations;

namespace back_bd.DTO_s
{
    public class PlanesCreateDTO
    {
        [Required(ErrorMessage = "El nombre del plan es requerido")]
        [StringLength(50, ErrorMessage = "El nombre no puede exceder los {1} caracteres")]
        public required string Nombre { get; set; }

        [Required(ErrorMessage = "El precio es requerido")]
        [Range(0, 999999.99, ErrorMessage = "El precio debe estar entre {1} y {2}")]
        public decimal Precio { get; set; }
    }
}