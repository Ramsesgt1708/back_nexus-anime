namespace back_bd.DTO_s
{
    public class AnimeReadDTO
    {
        public int _id { get; set; }
        public string Titulo { get; set; }
        public string Sinopsis { get; set; }
        public DateTime FechaEstreno { get; set; }
        public string ImagenUrl { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public bool IsActive { get; set; }
        
        public int EstudioId { get; set; }
        public string EstudioNombre { get; set; }
        
        public List<GeneroSimpleDTO> Generos { get; set; }
    }

    public class GeneroSimpleDTO
    {
        public int _id { get; set; }
        public string Nombre { get; set; }
    }
}
