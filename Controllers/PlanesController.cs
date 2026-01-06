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
    public class PlanesController : ControllerBase
    {
        private readonly IOutputCacheStore _outputCacheStore;
        private const string CacheTag = "planesCache";
        private readonly AppDBContext _db;

        public PlanesController(IOutputCacheStore outputCacheStore, AppDBContext db)
        {
            _outputCacheStore = outputCacheStore;
            _db = db;
        }

        [HttpGet]
        [OutputCache(Tags = [CacheTag])]
        public async Task<List<Planes>> Get()
        {
            return await _db.Planes.ToListAsync();
        }

        [HttpGet("{_id:int}", Name = "ObtenerPlanPorId")]
        [OutputCache(Tags = [CacheTag])]
        public async Task<ActionResult<Planes>> Get(int _id)
        {
            var plan = await _db.Planes.FindAsync(_id);
            return plan == null ? NotFound() : plan;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] PlanesCreateDTO dto)
        {
            // Evitar duplicados por Nombre
            var exists = await _db.Planes.AnyAsync(p => p.Nombre == dto.Nombre);
            if (exists)
            {
                return BadRequest("El plan ya existe.");
            }

            var plan = new Planes
            {
                Nombre = dto.Nombre,
                Precio = dto.Precio
            };

            _db.Add(plan);
            await _db.SaveChangesAsync();
            await _outputCacheStore.EvictByTagAsync(CacheTag, default);

            return CreatedAtRoute("ObtenerPlanPorId", new { _id = plan._id }, plan);
        }

        [HttpPut("{_id:int}")]
        public async Task<IActionResult> Put(int _id, [FromBody] PlanesCreateDTO dto)
        {
            var plan = await _db.Planes.FindAsync(_id);
            if (plan == null) return NotFound();

            // Evitar duplicados de Nombre (excluyendo el actual)
            var duplicate = await _db.Planes.AnyAsync(p => p.Nombre == dto.Nombre && p._id != _id);
            if (duplicate) return BadRequest("Ya existe un plan con ese nombre.");

            plan.Nombre = dto.Nombre;
            plan.Precio = dto.Precio;

            await _db.SaveChangesAsync();
            await _outputCacheStore.EvictByTagAsync(CacheTag, default);
            return NoContent();
        }

        [HttpPatch("{_id:int}/toggle-status")]
        public async Task<IActionResult> ToggleStatus(int _id)
        {
            var plan = await _db.Planes.FindAsync(_id);
            if (plan == null) return NotFound();

            plan.IsActive = !plan.IsActive;
            await _db.SaveChangesAsync();
            await _outputCacheStore.EvictByTagAsync(CacheTag, default);
            return Ok(new { _id = plan._id, plan.IsActive });
        }

        [HttpDelete("{_id:int}")]
        public async Task<IActionResult> Delete(int _id)
        {
            // Verificar que no haya usuarios usando este plan
            var usuariosConPlan = await _db.Usuarios.AnyAsync(u => u.PlanId == _id);
            if (usuariosConPlan)
            {
                return BadRequest("No se puede eliminar el plan porque hay usuarios asignados a él.");
            }

            var deleted = await _db.Planes.Where(x => x._id == _id).ExecuteDeleteAsync();
            if (deleted == 0) return NotFound();

            await _outputCacheStore.EvictByTagAsync(CacheTag, default);
            return NoContent();
        }
    }
}