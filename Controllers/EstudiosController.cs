using Microsoft.AspNetCore.Mvc;
using back_bd.Entidades;
using Microsoft.AspNetCore.OutputCaching;
using back_bd.DTO_s;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;

namespace back_bd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EstudiosController : ControllerBase
    {
        private readonly IOutputCacheStore outputCacheStore;
        private const string cacheTag = "estudiosCache";

        public readonly AppDBContext appDBContext;
        private readonly IMapper mapper;

        public EstudiosController(IOutputCacheStore outputCacheStore, AppDBContext appDBContext, IMapper mapper)
        {
            this.outputCacheStore = outputCacheStore;
            this.appDBContext = appDBContext;
            this.mapper = mapper;
        }

        [HttpGet]
        [OutputCache(Tags = [cacheTag])]
        public async Task<List<EstudiosReadDTO>> Get()
        {
            return await appDBContext.Estudios
                .ProjectTo<EstudiosReadDTO>(mapper.ConfigurationProvider)
                .ToListAsync();
        }

        [HttpGet("{_id:int}", Name = "ObtenerEstudioPorId")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<EstudiosReadDTO>> Get(int _id)
        {
            var estudio = await appDBContext.Estudios
                .ProjectTo<EstudiosReadDTO>(mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(x => x._id == _id);
            return estudio == null ? NotFound() : estudio;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] EstudiosCreateDTO estudioDTO)
        {
            var estudio = mapper.Map<Estudios>(estudioDTO);
            estudio._id = 0;
            appDBContext.Add(estudio);
            await appDBContext.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync(cacheTag, default);
            return CreatedAtRoute("ObtenerEstudioPorId", new { _id = estudio._id }, estudio);
        }

        [HttpPut("{_id:int}")]
        public async Task<IActionResult> Put(int _id, [FromBody] EstudiosCreateDTO estudioDTO)
        {
            var isfounded = await appDBContext.Estudios.AnyAsync(x => x._id == _id);
            if (!isfounded)
            {
                return NotFound();
            }
            var estudio = mapper.Map<Estudios>(estudioDTO);
            estudio._id = _id;
            appDBContext.Update(estudio);
            await appDBContext.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync(cacheTag, default);
            return NoContent();
        }

        [HttpPatch("{_id:int}/toggle-status")]
        public async Task<IActionResult> ToggleStatus(int _id)
        {
            var estudio = await appDBContext.Estudios.FindAsync(_id);
            if (estudio == null)
            {
                return NotFound();
            }

            estudio.IsActive = !estudio.IsActive;
            await appDBContext.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync(cacheTag, default);

            return Ok(new { _id = estudio._id, isActive = estudio.IsActive });
        }

        [HttpDelete("{_id:int}")]
        public async Task<IActionResult> Delete(int _id)
        {
            var isfounded = await appDBContext.Estudios.Where(x => x._id == _id).ExecuteDeleteAsync();
            if (isfounded == 0)
            {
                return NotFound();
            }
            await outputCacheStore.EvictByTagAsync(cacheTag, default);
            return NoContent();
        }
    }
}
