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
    public class GenerosController : ControllerBase
    {
        private readonly IOutputCacheStore outputCacheStore;
        private const string cacheTag = "generosCache";

        public readonly AppDBContext appDBContext;
        private readonly IMapper mapper;

        public GenerosController(IOutputCacheStore outputCacheStore, IConfiguration configuration, AppDBContext appDBContext, IMapper mapper) {
            this.outputCacheStore = outputCacheStore;
            this.appDBContext = appDBContext;
            this.mapper = mapper;
        }

        [HttpGet]
        [OutputCache(Tags = [cacheTag])]
        public async Task<List<GeneroReadDTO>> Get()
        {
            return await appDBContext.Generos.ProjectTo<GeneroReadDTO>(mapper.ConfigurationProvider).ToListAsync();
        }

        [HttpGet("{_id:int}",Name = "ObtenerGeneroPorId")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<GeneroReadDTO>> Get(int _id) {
            var genero = await appDBContext.Generos.ProjectTo<GeneroReadDTO>(mapper.ConfigurationProvider).FirstOrDefaultAsync(x => x._id == _id);
            return genero == null ? NotFound() : genero;
        }
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]GeneroCreateDTO generoDTO)
        {
            var genero = mapper.Map<Genero>(generoDTO);
            genero._id = 0;
            appDBContext.Add(genero);
            await appDBContext.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync(cacheTag, default);
            return CreatedAtRoute("ObtenerGeneroPorId", new { _id = genero._id }, genero);
        }
        [HttpPut("{_id:int}")]
        public async Task<IActionResult> Put(int _id, [FromBody] GeneroCreateDTO generoCreateDTO)
        {
            var isfounded = await appDBContext.Generos.AnyAsync(x => x._id == _id);
            if (!isfounded) { 
                return NotFound();
            }
            var genero = mapper.Map<Genero>(generoCreateDTO);
            genero._id = _id;
            appDBContext.Update(genero);
            await appDBContext.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync(cacheTag, default);
            return NoContent();
        }
        
        [HttpPatch("{_id:int}/toggle-status")]
        public async Task<IActionResult> ToggleStatus(int _id)
        {
            var genero = await appDBContext.Generos.FindAsync(_id);
            if (genero == null)
            {
                return NotFound();
            }
            
            genero.IsActive = !genero.IsActive;
            await appDBContext.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync(cacheTag, default);
            
            return Ok(new { _id = genero._id, isActive = genero.IsActive });
        }
        
        [HttpDelete("{_id:int}")]
        public async Task<IActionResult> Delete(int _id)
        {
            var isfounded = await appDBContext.Generos.Where(x => x._id == _id).ExecuteDeleteAsync();
            if (isfounded == 0)
            {
                return NotFound();
            }
            await outputCacheStore.EvictByTagAsync(cacheTag, default);
            return NoContent();

        }
     }
}
