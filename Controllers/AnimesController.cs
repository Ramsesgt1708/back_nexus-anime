using Microsoft.AspNetCore.Mvc;
using back_bd.Entidades;
using Microsoft.AspNetCore.OutputCaching;
using back_bd.DTO_s;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using back_bd.Services;

namespace back_bd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnimesController : ControllerBase
    {
        private readonly IOutputCacheStore outputCacheStore;
        private const string cacheTag = "animesCache";
        private const string containerName = "animes";

        public readonly AppDBContext appDBContext;
        private readonly IMapper mapper;
        private readonly ISaveFiles saveFiles;

        public AnimesController(IOutputCacheStore outputCacheStore, AppDBContext appDBContext, IMapper mapper, ISaveFiles saveFiles)
        {
            this.outputCacheStore = outputCacheStore;
            this.appDBContext = appDBContext;
            this.mapper = mapper;
            this.saveFiles = saveFiles;
        }

        [HttpGet]
        [OutputCache(Tags = [cacheTag])]
        public async Task<List<AnimeReadDTO>> Get()
        {
            var animes = await appDBContext.Animes
                .Include(a => a.Estudio)
                .Include(a => a.AnimeGeneros)
                    .ThenInclude(ag => ag.Genero)
                .ToListAsync();

            return mapper.Map<List<AnimeReadDTO>>(animes);
        }

        [HttpGet("{_id:int}", Name = "ObtenerAnimePorId")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<AnimeReadDTO>> Get(int _id)
        {
            var anime = await appDBContext.Animes
                .Include(a => a.Estudio)
                .Include(a => a.AnimeGeneros)
                    .ThenInclude(ag => ag.Genero)
                .FirstOrDefaultAsync(x => x._id == _id);

            if (anime == null)
            {
                return NotFound();
            }

            return mapper.Map<AnimeReadDTO>(anime);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromForm] AnimeCreateDTO animeDTO)
        {
            var estudioExists = await appDBContext.Estudios.AnyAsync(e => e._id == animeDTO.EstudioId);
            if (!estudioExists)
            {
                return BadRequest("El estudio especificado no existe");
            }

            var anime = mapper.Map<Anime>(animeDTO);
            anime._id = 0;

            // Subir imagen a Azure si existe
            if (animeDTO.Imagen != null)
            {
                anime.ImagenUrl = await saveFiles.SaveFile(containerName, animeDTO.Imagen);
            }

            appDBContext.Add(anime);
            await appDBContext.SaveChangesAsync();

            // Agregar los géneros usando la tabla intermedia
            if (animeDTO.GenerosIds != null && animeDTO.GenerosIds.Any())
            {
                foreach (var generoId in animeDTO.GenerosIds)
                {
                    appDBContext.AnimeGeneros.Add(new AnimeGeneros
                    {
                        AnimeId = anime._id,
                        GeneroId = generoId
                    });
                }
                await appDBContext.SaveChangesAsync();
            }

            await outputCacheStore.EvictByTagAsync(cacheTag, default);

            // Cargar las relaciones para el DTO
            var animeCreado = await appDBContext.Animes
                .Include(a => a.Estudio)
                .Include(a => a.AnimeGeneros)
                    .ThenInclude(ag => ag.Genero)
                .FirstOrDefaultAsync(a => a._id == anime._id);

            var animeDTO_response = mapper.Map<AnimeReadDTO>(animeCreado);

            return CreatedAtRoute("ObtenerAnimePorId", new { _id = anime._id }, animeDTO_response);
        }

        [HttpPut("{_id:int}")]
        public async Task<IActionResult> Put(int _id, [FromForm] AnimeCreateDTO animeDTO)
        {
            Console.WriteLine($"\n=== PUT ANIME ID: {_id} ===");
            Console.WriteLine($"Titulo: {animeDTO.Titulo}");
            Console.WriteLine($"EstudioId: {animeDTO.EstudioId}");
            Console.WriteLine($"GenerosIds recibidos: {(animeDTO.GenerosIds != null ? string.Join(", ", animeDTO.GenerosIds) : "NULL")}");
            Console.WriteLine($"Cantidad de géneros: {animeDTO.GenerosIds?.Count ?? 0}");

            var anime = await appDBContext.Animes
                .FirstOrDefaultAsync(x => x._id == _id);

            if (anime == null)
            {
                return NotFound();
            }

            var estudioExists = await appDBContext.Estudios.AnyAsync(e => e._id == animeDTO.EstudioId);
            if (!estudioExists)
            {
                return BadRequest("El estudio especificado no existe");
            }

            var imagenAnterior = anime.ImagenUrl;

            // Actualizar propiedades del anime
            anime.Titulo = animeDTO.Titulo;
            anime.Sinopsis = animeDTO.Sinopsis;
            anime.FechaEstreno = animeDTO.FechaEstreno;
            anime.EstudioId = animeDTO.EstudioId;

            if (animeDTO.Imagen != null)
            {
                anime.ImagenUrl = await saveFiles.EditFile(containerName, animeDTO.Imagen, imagenAnterior);
            }

            // === ACTUALIZAR GÉNEROS ===
            Console.WriteLine($"Eliminando géneros del anime {_id} usando SQL directo...");
            
            // Usar SQL directo para evitar problemas de rastreo
            await appDBContext.Database.ExecuteSqlRawAsync(
                "DELETE FROM [AnimeGeneros] WHERE [AnimeId] = {0}", _id);
            
            Console.WriteLine("Géneros eliminados correctamente");

            // Agregar las nuevas relaciones
            if (animeDTO.GenerosIds != null && animeDTO.GenerosIds.Any())
            {
                Console.WriteLine($"Agregando {animeDTO.GenerosIds.Count} nuevos géneros al anime {_id}...");
                
                foreach (var generoId in animeDTO.GenerosIds)
                {
                    // Crear instancias completamente nuevas sin rastreo previo
                    var nuevaRelacion = new AnimeGeneros
                    {
                        AnimeId = _id,
                        GeneroId = generoId
                    };
                    
                    appDBContext.AnimeGeneros.Add(nuevaRelacion);
                    Console.WriteLine($"  - Preparando: AnimeId: {nuevaRelacion.AnimeId}, GeneroId: {nuevaRelacion.GeneroId}");
                }
            }
            else
            {
                Console.WriteLine($"No se recibieron géneros para el anime {_id}");
            }

            Console.WriteLine("Guardando cambios en la base de datos...");
            await appDBContext.SaveChangesAsync();
            
            // Verificar que se guardaron correctamente
            var generosGuardados = await appDBContext.AnimeGeneros
                .Where(ag => ag.AnimeId == _id)
                .Select(ag => ag.GeneroId)
                .ToListAsync();
            
            Console.WriteLine($"Géneros FINALES guardados para anime {_id}: {string.Join(", ", generosGuardados)}");
            Console.WriteLine("=== FIN PUT ===\n");

            await outputCacheStore.EvictByTagAsync(cacheTag, default);

            return NoContent();
        }

        [HttpPatch("{_id:int}/toggle-status")]
        public async Task<IActionResult> ToggleStatus(int _id)
        {
            var anime = await appDBContext.Animes.FindAsync(_id);
            if (anime == null)
            {
                return NotFound();
            }
            await appDBContext.Database.ExecuteSqlRawAsync("EXEC sp_DesactivarAnime @AnimeId = {0}", _id);
            await outputCacheStore.EvictByTagAsync(cacheTag, default);
            await appDBContext.Entry(anime).ReloadAsync();
            return Ok(new { _id = anime._id, isActive = anime.IsActive });
        }

        [HttpDelete("{_id:int}")]
        public async Task<IActionResult> Delete(int _id)
        {
            var anime = await appDBContext.Animes.FindAsync(_id);
            if (anime == null)
            {
                return NotFound();
            }

            // Eliminar imagen de Azure
            await saveFiles.DeleteFile(containerName, anime.ImagenUrl);

            await appDBContext.Animes.Where(x => x._id == _id).ExecuteDeleteAsync();
            await outputCacheStore.EvictByTagAsync(cacheTag, default);
            return NoContent();
        }
    }
}