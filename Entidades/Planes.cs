using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace back_bd.Entidades
{
    public class Planes
    {
        [Key]
        [Column("_id")]
        public int _id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = null!;

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Precio { get; set; }

        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

        public DateTime FechaModificacion { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;

        // Navegación inversa
        public virtual ICollection<Usuarios> Usuarios { get; set; } = new List<Usuarios>();
    }
}