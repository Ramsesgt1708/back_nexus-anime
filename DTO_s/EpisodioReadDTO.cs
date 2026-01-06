namespace back_bd.DTO_s
{
    public class EpisodioReadDTO
    {
        public int _id { get; set; }
        public int Numero { get; set; }
        public string Titulo { get; set; } = null!;
        public string? Descripcion { get; set; }
        public string VideoUrl { get; set; } = null!;
        public int Duracion { get; set; }
        public DateTime FechaRegistro { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public bool IsActive { get; set; }
        
        // Datos del anime relacionado
        public int AnimeId { get; set; }
        public string AnimeTitulo { get; set; } = null!;
    }
}