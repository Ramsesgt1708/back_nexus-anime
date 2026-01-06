using back_bd.DTO_s.Auth;
using back_bd.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;

namespace back_bd.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDBContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(AppDBContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<LoginResponseDTO> Login(LoginDTO loginDto)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.Rol)
                .Include(u => u.Plan)
                .FirstOrDefaultAsync(u => u.Email == loginDto.Email);

            if (usuario == null)
            {
                throw new UnauthorizedAccessException("Credenciales incorrectas");
            }

            var passwordValido = BCrypt.Net.BCrypt.Verify(loginDto.Password, usuario.Password);
            if (!passwordValido)
            {
                throw new UnauthorizedAccessException("Credenciales incorrectas");
            }

            if (!usuario.IsActive)
            {
                throw new UnauthorizedAccessException("Usuario inactivo");
            }

            var token = GenerateJwtToken(usuario._id, usuario.Email, usuario.Rol.Nombre);

            return new LoginResponseDTO
            {
                Token = token,
                Expiracion = DateTime.UtcNow.AddHours(8),
                Usuario = new UsuarioInfoDTO
                {
                    _id = usuario._id,
                    Nombre = usuario.Nombre,
                    Email = usuario.Email,
                    Rol = usuario.Rol.Nombre,
                    Plan = usuario.Plan.Nombre,
                    IsActive = usuario.IsActive
                }
            };
        }

        public async Task<LoginResponseDTO> Register(RegisterDTO registerDto)
        {
            var emailExists = await _context.Usuarios.AnyAsync(u => u.Email == registerDto.Email);
            if (emailExists)
            {
                throw new InvalidOperationException("El email ya está registrado");
            }

            var planExists = await _context.Planes.AnyAsync(p => p._id == registerDto.PlanId && p.IsActive);
            if (!planExists)
            {
                throw new InvalidOperationException("El plan seleccionado no existe o no está activo");
            }

            var rolCliente = await _context.Roles.FirstOrDefaultAsync(r => r.Nombre == "CLIENTE");
            if (rolCliente == null)
            {
                throw new InvalidOperationException("El rol de cliente no está configurado");
            }

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

            var nuevoUsuario = new Usuarios
            {
                Nombre = registerDto.Nombre,
                Email = registerDto.Email,
                Password = passwordHash,
                RolId = rolCliente._id,
                PlanId = registerDto.PlanId,
                FechaRegistro = DateTime.UtcNow,
                IsActive = true
            };

            _context.Usuarios.Add(nuevoUsuario);
            await _context.SaveChangesAsync();

            await _context.Entry(nuevoUsuario).Reference(u => u.Rol).LoadAsync();
            await _context.Entry(nuevoUsuario).Reference(u => u.Plan).LoadAsync();

            var token = GenerateJwtToken(nuevoUsuario._id, nuevoUsuario.Email, nuevoUsuario.Rol.Nombre);

            return new LoginResponseDTO
            {
                Token = token,
                Expiracion = DateTime.UtcNow.AddHours(8),
                Usuario = new UsuarioInfoDTO
                {
                    _id = nuevoUsuario._id,
                    Nombre = nuevoUsuario.Nombre,
                    Email = nuevoUsuario.Email,
                    Rol = nuevoUsuario.Rol.Nombre,
                    Plan = nuevoUsuario.Plan.Nombre,
                    IsActive = nuevoUsuario.IsActive
                }
            };
        }

        public async Task<LoginResponseDTO> RefreshToken(string email)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.Rol)
                .Include(u => u.Plan)
                .FirstOrDefaultAsync(u => u.Email == email);

            if (usuario == null || !usuario.IsActive)
            {
                throw new UnauthorizedAccessException("Usuario no válido");
            }

            var token = GenerateJwtToken(usuario._id, usuario.Email, usuario.Rol.Nombre);

            return new LoginResponseDTO
            {
                Token = token,
                Expiracion = DateTime.UtcNow.AddHours(8),
                Usuario = new UsuarioInfoDTO
                {
                    _id = usuario._id,
                    Nombre = usuario.Nombre,
                    Email = usuario.Email,
                    Rol = usuario.Rol.Nombre,
                    Plan = usuario.Plan.Nombre,
                    IsActive = usuario.IsActive
                }
            };
        }

        public string GenerateJwtToken(int userId, string email, string rol)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"];
            if (string.IsNullOrWhiteSpace(secretKey))
            {
                throw new InvalidOperationException("JwtSettings:SecretKey no está configurado");
            }

            byte[] keyBytes;
            // Permitir claves en Base64 (recomendado) o texto plano
            try
            {
                keyBytes = Convert.FromBase64String(secretKey);
            }
            catch (FormatException)
            {
                keyBytes = Encoding.UTF8.GetBytes(secretKey);
            }

            if (keyBytes.Length < 32)
            {
                    throw new InvalidOperationException("La clave HMAC debe tener al menos 256 bits (32 bytes). Aumenta JwtSettings:SecretKey o usa una clave Base64 de 32+ bytes.");
            }

            var key = new SymmetricSecurityKey(keyBytes);
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim(ClaimTypes.Role, rol),
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(8),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}