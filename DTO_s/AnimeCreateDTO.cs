using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;

namespace back_bd.DTO_s
{
    public class AnimeCreateDTO
    {
        [Required(ErrorMessage = "El título es requerido")]
        [StringLength(150, ErrorMessage = "El título no puede exceder los {1} caracteres")]
        public required string Titulo { get; set; }

        [Required]
        public required string Sinopsis { get; set; }

        [Required]
        public DateTime FechaEstreno { get; set; }
        
        public IFormFile? Imagen { get; set; }

        [Required]
        public int EstudioId { get; set; }
        public List<int>? GenerosIds { get; set; }
    }
}
