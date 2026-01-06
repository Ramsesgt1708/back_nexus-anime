using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace back_bd.Entidades
{
    public class Usuarios
    {
        [Key]
        [Column("_id")]
        public int _id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100)]
        public required string Nombre { get; set; }

        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "El formato del correo no es válido")]
        [StringLength(100)]
        public required string Email { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        public required string Password { get; set; }

        public DateTime FechaRegistro { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public bool IsActive { get; set; }

        public int RolId { get; set; }
        
        [ForeignKey("RolId")]
        public virtual Roles Rol { get; set; }

        public int PlanId { get; set; }

        [ForeignKey("PlanId")]
        public virtual Planes Plan { get; set; }

        public virtual ICollection<Favoritos> Favoritos { get; set; } = new List<Favoritos>();
        public virtual ICollection<HistorialVisualizaciones> HistorialVisualizaciones { get; set; } = new List<HistorialVisualizaciones>();
    }
}