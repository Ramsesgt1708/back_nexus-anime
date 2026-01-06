using Microsoft.AspNetCore.Mvc;
using back_bd.Entidades;
using back_bd.DTO_s;
using Microsoft.EntityFrameworkCore;

namespace back_bd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HistorialVisualizacionesController : ControllerBase
    {
        private readonly AppDBContext _db;

        public HistorialVisualizacionesController(AppDBContext db)
        {
            _db = db;
        }

        // Obtener el progreso de un usuario en un anime específico
        [HttpGet("usuario/{usuarioId}/anime/{animeId}")]
        public async Task<ActionResult<HistorialVisualizacionesReadDTO>> GetProgresoAnime(int usuarioId, int animeId)
        {
            var historial = await _db.HistorialVisualizaciones
                .Include(h => h.Anime)
                .Include(h => h.Episodio)
                .FirstOrDefaultAsync(h => h.UsuarioId == usuarioId && h.AnimeId == animeId);

            if (historial == null)
            {
                return Ok(null);
            }

            var dto = new HistorialVisualizacionesReadDTO
            {
                _id = historial._id,
                FechaVisualizacion = historial.FechaVisualizacion,
                TiempoReproducido = historial.TiempoReproducido,
                Completado = historial.Completado,
                UsuarioId = historial.UsuarioId,
                AnimeId = historial.AnimeId,
                EpisodioId = historial.EpisodioId,
                TituloAnime = historial.Anime.Titulo,
                TituloEpisodio = historial.Episodio.Titulo,
                NumeroEpisodio = historial.Episodio.Numero,
                DuracionTotal = historial.Episodio.Duracion * 60
            };

            return Ok(dto);
        }

        // ✅ NUEVO: Obtener el progreso de un usuario en un episodio específico
        [HttpGet("usuario/{usuarioId}/episodio/{episodioId}")]
        public async Task<ActionResult<HistorialVisualizacionesReadDTO>> GetProgresoEpisodio(int usuarioId, int episodioId)
        {
            // Primero obtener el episodio para saber a qué anime pertenece
            var episodio = await _db.Episodios
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e._id == episodioId);

            if (episodio == null)
            {
                return BadRequest(new { mensaje = $"El episodio con ID {episodioId} no existe" });
            }

            // Ahora buscar el historial usando el AnimeId del episodio
            var historial = await _db.HistorialVisualizaciones
                .Include(h => h.Anime)
                .Include(h => h.Episodio)
                .FirstOrDefaultAsync(h => h.UsuarioId == usuarioId && h.AnimeId == episodio.AnimeId);

            if (historial == null)
            {
                return Ok(null);
            }

            var dto = new HistorialVisualizacionesReadDTO
            {
                _id = historial._id,
                FechaVisualizacion = historial.FechaVisualizacion,
                TiempoReproducido = historial.TiempoReproducido,
                Completado = historial.Completado,
                UsuarioId = historial.UsuarioId,
                AnimeId = historial.AnimeId,
                EpisodioId = historial.EpisodioId,
                TituloAnime = historial.Anime.Titulo,
                TituloEpisodio = historial.Episodio.Titulo,
                NumeroEpisodio = historial.Episodio.Numero,
                DuracionTotal = historial.Episodio.Duracion * 60
            };

            return Ok(dto);
        }

        // Guardar o actualizar el progreso (AUTOMÁTICAMENTE actualiza si ya existe)
        [HttpPost]
        public async Task<IActionResult> GuardarProgreso([FromBody] HistorialVisualizacionesCreateDTO dto)
        {
            // ✅ Validar que el episodio existe y obtener su AnimeId
            var episodio = await _db.Episodios
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e._id == dto.EpisodioId);

            if (episodio == null)
            {
                return BadRequest(new { mensaje = $"El episodio con ID {dto.EpisodioId} no existe" });
            }

            // ✅ Usar el AnimeId del episodio para garantizar consistencia
            int animeIdReal = episodio.AnimeId;

            // ✅ Buscar por UsuarioId + AnimeId (usando el AnimeId correcto del episodio)
            var historialExistente = await _db.HistorialVisualizaciones
                .FirstOrDefaultAsync(h => h.UsuarioId == dto.UsuarioId && h.AnimeId == animeIdReal);

            if (historialExistente != null)
            {
                // ✅ Actualizar el registro existente con el nuevo episodio y progreso
                historialExistente.EpisodioId = dto.EpisodioId;
                historialExistente.TiempoReproducido = dto.TiempoReproducido;
                historialExistente.Completado = dto.Completado;
                historialExistente.FechaVisualizacion = DateTime.UtcNow;
            }
            else
            {
                // ✅ Crear un nuevo registro usando el AnimeId correcto
                var nuevoHistorial = new HistorialVisualizaciones
                {
                    UsuarioId = dto.UsuarioId,
                    AnimeId = animeIdReal,
                    EpisodioId = dto.EpisodioId,
                    TiempoReproducido = dto.TiempoReproducido,
                    Completado = dto.Completado
                };
                _db.HistorialVisualizaciones.Add(nuevoHistorial);
            }

            await _db.SaveChangesAsync();
            return Ok(new { mensaje = "Progreso guardado correctamente" });
        }

        // Obtener el historial completo de un usuario (por animes, no por episodios)
        [HttpGet("usuario/{usuarioId}")]
        public async Task<ActionResult<List<HistorialVisualizacionesReadDTO>>> GetHistorialUsuario(int usuarioId)
        {
            var historial = await _db.HistorialVisualizaciones
                .Include(h => h.Anime)
                .Include(h => h.Episodio)
                .Where(h => h.UsuarioId == usuarioId)
                .OrderByDescending(h => h.FechaVisualizacion)
                .Select(h => new HistorialVisualizacionesReadDTO
                {
                    _id = h._id,
                    FechaVisualizacion = h.FechaVisualizacion,
                    TiempoReproducido = h.TiempoReproducido,
                    Completado = h.Completado,
                    UsuarioId = h.UsuarioId,
                    AnimeId = h.AnimeId,
                    EpisodioId = h.EpisodioId,
                    TituloAnime = h.Anime.Titulo,
                    TituloEpisodio = h.Episodio.Titulo,
                    NumeroEpisodio = h.Episodio.Numero,
                    DuracionTotal = h.Episodio.Duracion * 60
                })
                .ToListAsync();

            return Ok(historial);
        }
    }
}