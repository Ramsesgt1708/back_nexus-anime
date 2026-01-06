using System.ComponentModel.DataAnnotations;

namespace back_bd.Entidades
{
    public class Roles
    {
        [Key]
        public int _id { get; set; }

        [Required]
        [StringLength(50)]
        public required string Nombre { get; set; }

        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public bool IsActive { get; set; }

        public virtual ICollection<Usuarios> Usuarios { get; set; }
    }
}