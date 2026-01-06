using System.ComponentModel.DataAnnotations;

namespace back_bd.DTO_s
{
    public class EpisodioCreateDTO
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "El número de episodio debe ser mayor a 0")]
        public int Numero { get; set; }

        [Required(ErrorMessage = "El título es requerido")]
        [MaxLength(200)]
        public string Titulo { get; set; } = null!;

        [MaxLength(1000)]
        public string? Descripcion { get; set; }

        [Required(ErrorMessage = "La ruta del video es requerida")]
        [MaxLength(500)]
        public string VideoUrl { get; set; } = null!;

        [Required]
        [Range(1, 300, ErrorMessage = "La duración debe estar entre 1 y 300 minutos")]
        public int Duracion { get; set; }

        [Required]
        public int AnimeId { get; set; }
    }
}