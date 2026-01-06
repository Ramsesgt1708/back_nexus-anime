using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace back_bd.Entidades
{
    public class Anime
    {
        [Key]
        [Column("_id")]
        public int _id { get; set; }

        [Required(ErrorMessage = "El título es requerido")]
        [StringLength(150, ErrorMessage = "El título no puede exceder los {1} caracteres")]
        public required string Titulo { get; set; }

        [Required]
        public required string Sinopsis { get; set; }

        [Required]
        public DateTime FechaEstreno { get; set; }

        [Url]
        public string ImagenUrl { get; set; }

        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public bool IsActive { get; set; }

        // Relación con ESTUDIOS
        public int EstudioId { get; set; }
        
        [ForeignKey("EstudioId")]
        public virtual Estudios Estudio { get; set; }

        // Relación con GÉNEROS a través de tabla intermedia
        public virtual ICollection<AnimeGeneros> AnimeGeneros { get; set; } = new List<AnimeGeneros>();

        // Otras relaciones
        public virtual ICollection<Episodios> Episodios { get; set; } = new List<Episodios>();
        public virtual ICollection<Favoritos> Favoritos { get; set; } = new List<Favoritos>();

        public virtual ICollection<HistorialVisualizaciones> HistorialVisualizaciones { get; set; } = new List<HistorialVisualizaciones>();
    }
}
