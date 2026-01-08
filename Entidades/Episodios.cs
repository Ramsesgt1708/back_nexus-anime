using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace back_bd.Entidades
{
    [Table("Episodios")]
    public class Episodios
    {
        [Key]
        [Column("_id")]
        public int _id { get; set; }

        [Required]
        public int Numero { get; set; }

        [Required]
        [MaxLength(200)]
        public string Titulo { get; set; } = null!;

        [MaxLength(1000)]
        public string? Descripcion { get; set; }

        [Required]
        [MaxLength(500)]
        public string VideoUrl { get; set; } = null!;

        [Required]
        public int Duracion { get; set; }

        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

        public DateTime? FechaModificacion { get; set; }

        public bool IsActive { get; set; } = true;

        [Required]
        [ForeignKey(nameof(Anime))]
        public int AnimeId { get; set; }
        public virtual Anime Anime { get; set; } = null!;
        public virtual ICollection<HistorialVisualizaciones> HistorialVisualizaciones { get; set; } = new List<HistorialVisualizaciones>();
    }
}
