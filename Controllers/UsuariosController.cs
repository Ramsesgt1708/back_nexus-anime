using Microsoft.AspNetCore.Mvc;
using back_bd.Entidades;
using Microsoft.AspNetCore.OutputCaching;
using back_bd.DTO_s.Usuarios;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace back_bd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // [Authorize] ← COMENTA ESTA LÍNEA TEMPORALMENTE
    public class UsuariosController : ControllerBase
    {
        private readonly IOutputCacheStore _outputCacheStore;
        private const string CacheTag = "usuariosCache";
        private readonly AppDBContext _db;
        private readonly IMapper _mapper;

        public UsuariosController(IOutputCacheStore outputCacheStore, AppDBContext db, IMapper mapper)
        {
            _outputCacheStore = outputCacheStore;
            _db = db;
            _mapper = mapper;
        }

        [HttpGet]
        [OutputCache(Tags = [CacheTag])]
        public async Task<List<UsuarioReadDTO>> Get()
        {
            var usuarios = await _db.Usuarios
                .Include(u => u.Rol)
                .Include(u => u.Plan)
                .ToListAsync();

            return usuarios.Select(u => new UsuarioReadDTO
            {
                _id = u._id,
                Nombre = u.Nombre,
                Email = u.Email,
                FechaRegistro = u.FechaRegistro,
                FechaModificacion = u.FechaModificacion,
                IsActive = u.IsActive,
                RolId = u.RolId,
                RolNombre = u.Rol.Nombre,
                PlanId = u.PlanId,
                PlanNombre = u.Plan.Nombre
            }).ToList();
        }

        [HttpGet("{_id:int}", Name = "ObtenerUsuarioPorId")]
        //[Authorize(Roles = "ROOT,ADMIN")]
        [OutputCache(Tags = [CacheTag])]
        public async Task<ActionResult<UsuarioReadDTO>> Get(int _id)
        {
            var u = await _db.Usuarios
                .Include(x => x.Rol)
                .Include(x => x.Plan)
                .FirstOrDefaultAsync(x => x._id == _id);

            if (u == null) return NotFound();

            return new UsuarioReadDTO
            {
                _id = u._id,
                Nombre = u.Nombre,
                Email = u.Email,
                FechaRegistro = u.FechaRegistro,
                FechaModificacion = u.FechaModificacion,
                IsActive = u.IsActive,
                RolId = u.RolId,
                RolNombre = u.Rol.Nombre,
                PlanId = u.PlanId,
                PlanNombre = u.Plan.Nombre
            };
        }

        [HttpPost]
        //[Authorize(Roles = "ROOT,ADMIN")]
        public async Task<IActionResult> Post([FromBody] UsuarioCreateDTO dto)
        {
            if (await _db.Usuarios.AnyAsync(x => x.Email == dto.Email))
                return BadRequest("El email ya está registrado.");

            if (!await _db.Roles.AnyAsync(r => r._id == dto.RolId))
                return BadRequest("El rol especificado no existe.");

            if (!await _db.Planes.AnyAsync(p => p._id == dto.PlanId))
                return BadRequest("El plan especificado no existe.");

            var usuario = new Usuarios
            {
                Nombre = dto.Nombre,
                Email = dto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                RolId = dto.RolId,
                PlanId = dto.PlanId,
                IsActive = true
            };

            _db.Add(usuario);
            await _db.SaveChangesAsync();
            await _outputCacheStore.EvictByTagAsync(CacheTag, default);

            return CreatedAtRoute("ObtenerUsuarioPorId", new { _id = usuario._id }, new { usuario._id });
        }

        [HttpPut("{_id:int}")]
        //[Authorize(Roles = "ROOT,ADMIN")]
        public async Task<IActionResult> Put(int _id, [FromBody] UsuarioCreateDTO dto)
        {
            var usuario = await _db.Usuarios.FirstOrDefaultAsync(x => x._id == _id);
            if (usuario == null) return NotFound();

            if (await _db.Usuarios.AnyAsync(x => x.Email == dto.Email && x._id != _id))
                return BadRequest("El email ya está registrado por otro usuario.");

            if (!await _db.Roles.AnyAsync(r => r._id == dto.RolId))
                return BadRequest("El rol especificado no existe.");

            if (!await _db.Planes.AnyAsync(p => p._id == dto.PlanId))
                return BadRequest("El plan especificado no existe.");

            usuario.Nombre = dto.Nombre;
            usuario.Email = dto.Email;
            usuario.RolId = dto.RolId;
            usuario.PlanId = dto.PlanId;

            if (!string.IsNullOrWhiteSpace(dto.Password))
                usuario.Password = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            await _db.SaveChangesAsync();
            await _outputCacheStore.EvictByTagAsync(CacheTag, default);

            return NoContent();
        }

        [HttpPatch("{_id:int}/toggle-status")]
        //[Authorize(Roles = "ROOT,ADMIN")]
        public async Task<IActionResult> ToggleStatus(int _id)
        {
            var usuario = await _db.Usuarios.FindAsync(_id);
            if (usuario == null) return NotFound();

            usuario.IsActive = !usuario.IsActive;
            await _db.SaveChangesAsync();
            await _outputCacheStore.EvictByTagAsync(CacheTag, default);

            return Ok(new { _id = usuario._id, usuario.IsActive });
        }

        [HttpDelete("{_id:int}")]
        [Authorize(Roles = "ROOT")]
        public async Task<IActionResult> Delete(int _id)
        {
            var deleted = await _db.Usuarios.Where(x => x._id == _id).ExecuteDeleteAsync();
            if (deleted == 0) return NotFound();

            await _outputCacheStore.EvictByTagAsync(CacheTag, default);
            return NoContent();
        }
    }
}