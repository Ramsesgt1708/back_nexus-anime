using System.ComponentModel.DataAnnotations;
using back_bd.Validaciones;
namespace back_bd.Entidades
{
    public class Genero
    {
        [Key]
        public int _id { get; set; }
        [Required(ErrorMessage ="El campo {0} es requerido")]
        [StringLength(50,ErrorMessage ="El campo {0} debe tener menos de {1} caracteres")]
        [UpperCaseFirst]
       public required string Nombre { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public bool IsActive { get; set; }

        // Relación muchos a muchos con Anime
        public virtual ICollection<AnimeGeneros> AnimeGeneros { get; set; } = new List<AnimeGeneros>();
    }
}
