using Microsoft.AspNetCore.Mvc;
using back_bd.Entidades;
using Microsoft.AspNetCore.OutputCaching;
using back_bd.DTO_s;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

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
                .Include(e => e.Anime)
                .Where(e => e.AnimeId == animeId && e.IsActive)
                .OrderBy(e => e.Numero)
                .ToListAsync();

            return Ok(mapper.Map<List<EpisodioReadDTO>>(episodios));
        }

        // POST: api/Episodios
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] EpisodioCreateDTO episodioDTO)
        {
            // Verificar que el anime existe
            var animeExists = await appDBContext.Animes.AnyAsync(a => a._id == episodioDTO.AnimeId);
            if (!animeExists)
            {
                return BadRequest(new { message = "El anime especificado no existe" });
            }

            // Verificar que no exista un episodio con el mismo número para ese anime
            var episodioExistente = await appDBContext.Episodios
                .AnyAsync(e => e.AnimeId == episodioDTO.AnimeId && e.Numero == episodioDTO.Numero);

            if (episodioExistente)
            {
                return BadRequest(new { message = $"Ya existe un episodio número {episodioDTO.Numero} para este anime" });
            }

            var episodio = mapper.Map<Episodios>(episodioDTO);
            episodio._id = 0;
            episodio.FechaRegistro = DateTime.UtcNow;

            appDBContext.Add(episodio);
            await appDBContext.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync(cacheTag, default);

            // Cargar la relación para el DTO
            var episodioCreado = await appDBContext.Episodios
                .Include(e => e.Anime)
                .FirstOrDefaultAsync(e => e._id == episodio._id);

            var episodioResponse = mapper.Map<EpisodioReadDTO>(episodioCreado);

            return CreatedAtRoute("ObtenerEpisodioPorId", new { _id = episodio._id }, episodioResponse);
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
        [Authorize(Roles = "ROOT,ADMIN")]
        public async Task<IActionResult> ToggleStatus(int _id)
        {
            var episodio = await appDBContext.Episodios.FindAsync(_id);
            if (episodio == null)
            {
                return NotFound(new { message = "Episodio no encontrado" });
            }

            episodio.IsActive = !episodio.IsActive;
            episodio.FechaModificacion = DateTime.UtcNow;

            await appDBContext.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync(cacheTag, default);

            return Ok(new { _id = episodio._id, isActive = episodio.IsActive });
        }

        // DELETE: api/Episodios/5
        [HttpDelete("{_id:int}")]
        [Authorize(Roles = "ROOT")]
        public async Task<IActionResult> Delete(int _id)
        {
            var episodio = await appDBContext.Episodios.FindAsync(_id);
            if (episodio == null)
            {
                return NotFound(new { message = "Episodio no encontrado" });
            }

            appDBContext.Episodios.Remove(episodio);
            await appDBContext.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync(cacheTag, default);

            return NoContent();
        }
    }
}