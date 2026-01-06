using System.ComponentModel.DataAnnotations;

namespace back_bd.Entidades
{
    public class Estudios
    {
        [Key]
        public int _id { get; set; }
        
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(50, ErrorMessage = "El campo {0} debe tener menos de {1} caracteres")]
        public required string Nombre { get; set; }
        
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public bool IsActive { get; set; }

        public virtual ICollection<Anime> Animes { get; set; }
    }
}
