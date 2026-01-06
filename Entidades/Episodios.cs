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

        // ✅ NUEVO: Descripción del episodio
        [MaxLength(1000)]
        public string? Descripcion { get; set; }

        // ✅ ACTUALIZADO: Ruta relativa para el video (ej: "Naruto/Temporada1/episodio_01.mp4")
        [Required]
        [MaxLength(500)]
        public string VideoUrl { get; set; } = null!;

        [Required]
        public int Duracion { get; set; } // En minutos

        // Auditoría
        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

        public DateTime? FechaModificacion { get; set; }

        public bool IsActive { get; set; } = true;

        // Clave foránea
        [Required]
        [ForeignKey(nameof(Anime))]
        public int AnimeId { get; set; }

        // Navegación
        public virtual Anime Anime { get; set; } = null!;

        // Navegación inversa
        public virtual ICollection<HistorialVisualizaciones> HistorialVisualizaciones { get; set; } = new List<HistorialVisualizaciones>();
    }
}
