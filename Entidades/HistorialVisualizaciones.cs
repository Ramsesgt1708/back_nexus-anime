using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace back_bd.Entidades
{
    public class HistorialVisualizaciones
    {
        [Key]
        [Column("_id")]
        public int _id { get; set; }

        public DateTime FechaVisualizacion { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Tiempo de reproducción en SEGUNDOS del episodio actual
        /// </summary>
        [Required]
        public int TiempoReproducido { get; set; } = 0;

        public bool Completado { get; set; } = false;

        // Claves foráneas
        [Required]
        [ForeignKey(nameof(Usuario))]
        public int UsuarioId { get; set; }

        // ✅ NUEVO: Referencia al anime para agrupación
        [Required]
        [ForeignKey(nameof(Anime))]
        public int AnimeId { get; set; }

        // Último episodio visto de este anime
        [Required]
        [ForeignKey(nameof(Episodio))]
        public int EpisodioId { get; set; }

        // Navegaciones
        public virtual Usuarios Usuario { get; set; } = null!;
        public virtual Anime Anime { get; set; } = null!;
        public virtual Episodios Episodio { get; set; } = null!;
    }
}