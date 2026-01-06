namespace back_bd.DTO_s
{
    public class HistorialVisualizacionesReadDTO
    {
        public int _id { get; set; }
        public DateTime FechaVisualizacion { get; set; }
        public int TiempoReproducido { get; set; }
        public bool Completado { get; set; }
        public int UsuarioId { get; set; }
        public int AnimeId { get; set; } 
        public int EpisodioId { get; set; }
  
        public string? TituloAnime { get; set; }
        public string? TituloEpisodio { get; set; }
        public int? NumeroEpisodio { get; set; }
        public int? DuracionTotal { get; set; }
    }
}