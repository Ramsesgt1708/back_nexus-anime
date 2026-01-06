using back_bd.DTO_s.Auth;

namespace back_bd.Services
{
    public interface IAuthService
    {
        Task<LoginResponseDTO> Login(LoginDTO loginDto);
        Task<LoginResponseDTO> Register(RegisterDTO registerDto);
        Task<LoginResponseDTO> RefreshToken(string email);
        string GenerateJwtToken(int userId, string email, string rol);
    }
}