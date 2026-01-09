using Microsoft.AspNetCore.Mvc;
using back_bd.Entidades;
using back_bd.DTO_s;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

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

        [HttpGet("usuario/{usuarioId}/anime/{animeId}")]
        public async Task<ActionResult<HistorialVisualizacionesReadDTO>> GetProgresoAnime(int usuarioId, int animeId)
        {
            var result = await _db.Database
                .SqlQueryRaw<HistorialVisualizacionesReadDTO>(
                    "EXEC sp_ObtenerProgresoAnime @UsuarioId={0}, @AnimeId={1}",
                    usuarioId, animeId)
                .ToListAsync();

            if (result == null || !result.Any())
            {
                return Ok(null);
            }

            return Ok(result.First());
        }

        [HttpGet("usuario/{usuarioId}/episodio/{episodioId}")]
        public async Task<ActionResult<HistorialVisualizacionesReadDTO>> GetProgresoEpisodio(int usuarioId, int episodioId)
        {
            var result = await _db.Database
                .SqlQueryRaw<HistorialVisualizacionesReadDTO>(
                    "EXEC sp_ObtenerProgresoEpisodio @UsuarioId={0}, @EpisodioId={1}",
                    usuarioId, episodioId)
                .ToListAsync();

            if (result == null || !result.Any())
            {

                var episodio = await _db.Episodios
                    .AsNoTracking()
                    .AnyAsync(e => e._id == episodioId);

                if (!episodio)
                {
                    return BadRequest(new { mensaje = $"El episodio con ID {episodioId} no existe" });
                }

                return Ok(null);
            }

            return Ok(result.First());
        }

        [HttpPost]
        public async Task<IActionResult> GuardarProgreso([FromBody] HistorialVisualizacionesCreateDTO dto)
        {
            try
            {
                await _db.Database.ExecuteSqlRawAsync(
                    "EXEC sp_GuardarProgresoVisualizacion @UsuarioId={0}, @EpisodioId={1}, @TiempoReproducido={2}, @Completado={3}",
                    dto.UsuarioId, dto.EpisodioId, dto.TiempoReproducido, dto.Completado);

                return Ok(new { mensaje = "Progreso guardado correctamente" });
            }
            catch (SqlException ex)
            {
                if (ex.Message.Contains("no existe"))
                {
                    return BadRequest(new { mensaje = ex.Message });
                }
                throw;
            }
        }

        [HttpGet("usuario/{usuarioId}")]
        public async Task<ActionResult<List<HistorialVisualizacionesReadDTO>>> GetHistorialUsuario(int usuarioId)
        {
            var historial = await _db.Database
                .SqlQueryRaw<HistorialVisualizacionesReadDTO>(
                    "EXEC sp_ObtenerHistorialUsuario @UsuarioId={0}",
                    usuarioId)
                .ToListAsync();

            return Ok(historial);
        }
    }
}