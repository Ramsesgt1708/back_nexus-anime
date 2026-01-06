namespace back_bd.DTO_s.Usuarios
{
    public class UsuarioReadDTO
    {
        public int _id { get; set; }
        public required string Nombre { get; set; }
        public required string Email { get; set; }

        public DateTime FechaRegistro { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public bool IsActive { get; set; }

        public int RolId { get; set; }
        public string? RolNombre { get; set; }

        public int PlanId { get; set; }
        public required string PlanNombre { get; set; }
    }
}