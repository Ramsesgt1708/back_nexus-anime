using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace back_bd.Entidades
{
    public class Favoritos
    {
        [Key]
        [Column("_id")]
        public int _id { get; set; }

        public DateTime FechaAgregado { get; set; } = DateTime.UtcNow;

        // Claves foráneas
        [Required]
        [ForeignKey(nameof(Usuario))]
        public int UsuarioId { get; set; }

        [Required]
        [ForeignKey(nameof(Anime))]
        public int AnimeId { get; set; }

        // Navegaciones
        public virtual Usuarios Usuario { get; set; } = null!;
        public virtual Anime Anime { get; set; } = null!;
    }
}