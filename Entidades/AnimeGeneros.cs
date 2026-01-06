using back_bd.Entidades;
public class AnimeGeneros
{
    public int AnimeId { get; set; }
    public virtual Anime Anime { get; set; }
    
    public int GeneroId { get; set; }
    public virtual Genero Genero { get; set; }
}