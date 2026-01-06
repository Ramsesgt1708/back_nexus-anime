using Microsoft.AspNetCore.Mvc;
using back_bd.Entidades;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using back_bd.DTO_s;

namespace back_bd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "ROOT,ADMIN")]
    public class RolesController : ControllerBase
    {
        private readonly IOutputCacheStore _outputCacheStore;
        private const string CacheTag = "rolesCache";
        private readonly AppDBContext _db;

        public RolesController(IOutputCacheStore outputCacheStore, AppDBContext db)
        {
            _outputCacheStore = outputCacheStore;
            _db = db;
        }

        [HttpGet]
        [OutputCache(Tags = [CacheTag])]
        public async Task<List<Roles>> Get()
        {
            return await _db.Roles.ToListAsync();
        }

        [HttpGet("{_id:int}", Name = "ObtenerRolPorId")]
        [OutputCache(Tags = [CacheTag])]
        public async Task<ActionResult<Roles>> Get(int _id)
        {
            var rol = await _db.Roles.FindAsync(_id);
            return rol == null ? NotFound() : rol;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] RolesCreateDTO dto)
        {
            // Evitar duplicados por Nombre
            var exists = await _db.Roles.AnyAsync(r => r.Nombre == dto.Nombre);
            if (exists)
            {
                return BadRequest("El rol ya existe.");
            }

            var rol = new Roles { Nombre = dto.Nombre };
            _db.Add(rol);
            await _db.SaveChangesAsync();
            await _outputCacheStore.EvictByTagAsync(CacheTag, default);

            return CreatedAtRoute("ObtenerRolPorId", new { _id = rol._id }, rol);
        }

        [HttpPut("{_id:int}")]
        public async Task<IActionResult> Put(int _id, [FromBody] RolesCreateDTO dto)
        {
            var rol = await _db.Roles.FindAsync(_id);
            if (rol == null) return NotFound();

            // Evitar duplicados de Nombre (excluyendo el actual)
            var duplicate = await _db.Roles.AnyAsync(r => r.Nombre == dto.Nombre && r._id != _id);
            if (duplicate) return BadRequest("Ya existe un rol con ese nombre.");

            rol.Nombre = dto.Nombre;
            await _db.SaveChangesAsync();
            await _outputCacheStore.EvictByTagAsync(CacheTag, default);
            return NoContent();
        }

        [HttpPatch("{_id:int}/toggle-status")]
        public async Task<IActionResult> ToggleStatus(int _id)
        {
            var rol = await _db.Roles.FindAsync(_id);
            if (rol == null) return NotFound();

            rol.IsActive = !rol.IsActive;
            await _db.SaveChangesAsync();
            await _outputCacheStore.EvictByTagAsync(CacheTag, default);
            return Ok(new { _id = rol._id, rol.IsActive });
        }

        [HttpDelete("{_id:int}")]
        public async Task<IActionResult> Delete(int _id)
        {
            var deleted = await _db.Roles.Where(x => x._id == _id).ExecuteDeleteAsync();
            if (deleted == 0) return NotFound();

            await _outputCacheStore.EvictByTagAsync(CacheTag, default);
            return NoContent();
        }
    }
}