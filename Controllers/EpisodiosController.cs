using AutoMapper;
using back_bd.DTO_s;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using back_bd.Entidades;
namespace back_bd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EpisodiosController : ControllerBase
    {
        private readonly IOutputCacheStore outputCacheStore;
        private const string cacheTag = "episodiosCache";

        private readonly AppDBContext appDBContext;
        private readonly IMapper mapper;

        public EpisodiosController(IOutputCacheStore outputCacheStore, AppDBContext appDBContext, IMapper mapper)
        {
            this.outputCacheStore = outputCacheStore;
            this.appDBContext = appDBContext;
            this.mapper = mapper;
        }

        // GET: api/Episodios
        [HttpGet]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<List<EpisodioReadDTO>>> Get()
        {
            var episodios = await appDBContext.Episodios
                .Include(e => e.Anime)
                .Where(e => e.IsActive)
                .OrderBy(e => e.AnimeId)
                .ThenBy(e => e.Numero)
                .ToListAsync();

            return Ok(mapper.Map<List<EpisodioReadDTO>>(episodios));
        }

        // GET: api/Episodios/5
        [HttpGet("{_id:int}", Name = "ObtenerEpisodioPorId")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<EpisodioReadDTO>> Get(int _id)
        {
            var episodio = await appDBContext.Episodios
                .Include(e => e.Anime)
                .FirstOrDefaultAsync(e => e._id == _id);

            if (episodio == null)
            {
                return NotFound(new { message = "Episodio no encontrado" });
            }

            return Ok(mapper.Map<EpisodioReadDTO>(episodio));
        }

        // GET: api/Episodios/anime/5
        [HttpGet("anime/{animeId:int}")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<List<EpisodioReadDTO>>> GetByAnime(int animeId)
        {
            var animeExists = await appDBContext.Animes.AnyAsync(a => a._id == animeId);
            if (!animeExists)
            {
                return NotFound(new { message = "Anime no encontrado" });
            }

            var episodios = await appDBContext.Episodios
                .FromSqlRaw("EXEC sp_ObtenerEpisodiosPorAnime @AnimeId = {0}", animeId)
                .ToListAsync();
            episodios = episodios.OrderBy(e => e.Numero).ToList();

            foreach (var episodio in episodios)
            {
                await appDBContext.Entry(episodio)
                    .Reference(e => e.Anime)
                    .LoadAsync();
            }

            return Ok(mapper.Map<List<EpisodioReadDTO>>(episodios));
        }

        // POST: api/Episodios
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] EpisodioCreateDTO episodioDTO)
        {
            try
            {
                var parametroId = new SqlParameter("@EpisodioId", SqlDbType.Int) { Direction = ParameterDirection.Output };

                var parameters = new[]
                {
                new SqlParameter("@Numero", episodioDTO.Numero),
                new SqlParameter("@Titulo", episodioDTO.Titulo),
                new SqlParameter("@Descripcion", episodioDTO.Descripcion ?? (object)DBNull.Value),
                new SqlParameter("@VideoUrl", episodioDTO.VideoUrl),
                new SqlParameter("@Duracion", episodioDTO.Duracion),
                new SqlParameter("@AnimeId", episodioDTO.AnimeId),
                parametroId
                };

                await appDBContext.Database.ExecuteSqlRawAsync(
                    "EXEC sp_InsertarEpisodio @Numero, @Titulo, @Descripcion, @VideoUrl, @Duracion, @AnimeId, @EpisodioId OUTPUT",
                    parameters
                );

                var episodioId = (int)parametroId.Value;

                await outputCacheStore.EvictByTagAsync(cacheTag, default);

                var episodioCreado = await appDBContext.Episodios
                    .Include(e => e.Anime)
                    .FirstOrDefaultAsync(e => e._id == episodioId);

                var episodioResponse = mapper.Map<EpisodioReadDTO>(episodioCreado);

                return CreatedAtRoute("ObtenerEpisodioPorId", new { _id = episodioId }, episodioResponse);
            }
            catch (SqlException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT: api/Episodios/5
        [HttpPut("{_id:int}")]
        public async Task<IActionResult> Put(int _id, [FromBody] EpisodioCreateDTO episodioDTO)
        {
            var episodio = await appDBContext.Episodios.FindAsync(_id);
            if (episodio == null)
            {
                return NotFound(new { message = "Episodio no encontrado" });
            }

            // Verificar que el anime existe
            var animeExists = await appDBContext.Animes.AnyAsync(a => a._id == episodioDTO.AnimeId);
            if (!animeExists)
            {
                return BadRequest(new { message = "El anime especificado no existe" });
            }

            // Verificar que no exista otro episodio con el mismo número para ese anime
            var duplicado = await appDBContext.Episodios
                .AnyAsync(e => e.AnimeId == episodioDTO.AnimeId 
                            && e.Numero == episodioDTO.Numero 
                            && e._id != _id);

            if (duplicado)
            {
                return BadRequest(new { message = $"Ya existe otro episodio número {episodioDTO.Numero} para este anime" });
            }

            mapper.Map(episodioDTO, episodio);
            episodio.FechaModificacion = DateTime.UtcNow;

            await appDBContext.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync(cacheTag, default);

            return NoContent();
        }

        // PATCH: api/Episodios/5/toggle-status
        [HttpPatch("{_id:int}/toggle-status")]
        public async Task<IActionResult> ToggleStatus(int _id)
        {
            try
            {
                var parameter = new SqlParameter("@EpisodioId", _id);

                await appDBContext.Database.ExecuteSqlRawAsync(
                    "EXEC sp_DesactivarEpisodio @EpisodioId",
                    parameter
                );

                // Limpiar caché del servidor
                await outputCacheStore.EvictByTagAsync(cacheTag, default);
                
                // Limpiar tracking de EF
                appDBContext.ChangeTracker.Clear();

                // Consultar estado actualizado
                var episodio = await appDBContext.Episodios
                    .AsNoTracking()
                    .FirstOrDefaultAsync(e => e._id == _id);

                if (episodio == null)
                {
                    return NotFound(new { message = "Episodio no encontrado" });
                }

                // Agregar headers para evitar caché del navegador
                Response.Headers.Append("Cache-Control", "no-cache, no-store, must-revalidate");
                Response.Headers.Append("Pragma", "no-cache");
                Response.Headers.Append("Expires", "0");

                return Ok(new { _id = episodio._id, isActive = episodio.IsActive });
            }
            catch (SqlException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}