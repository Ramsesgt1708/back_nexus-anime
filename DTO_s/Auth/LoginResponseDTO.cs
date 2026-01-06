namespace back_bd.DTO_s.Auth
{
    public class LoginResponseDTO
    {
        public string Token { get; set; }
        public DateTime Expiracion { get; set; }
        public UsuarioInfoDTO Usuario { get; set; }
    }

    public class UsuarioInfoDTO
    {
        public int _id { get; set; }
        public string Nombre { get; set; }
        public string Email { get; set; }
        public string Rol { get; set; }
        public string Plan { get; set; }
        public bool IsActive { get; set; }
    }
}