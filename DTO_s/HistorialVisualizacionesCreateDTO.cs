namespace back_bd.DTO_s
{
    public class HistorialVisualizacionesCreateDTO
    {
        public int UsuarioId { get; set; }
       
        public int AnimeId { get; set; }
        
        public int EpisodioId { get; set; }
        public int TiempoReproducido { get; set; }
        public bool Completado { get; set; } = false;
    }
}