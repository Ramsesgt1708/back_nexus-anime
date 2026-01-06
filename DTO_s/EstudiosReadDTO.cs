namespace back_bd.DTO_s
{
    public class EstudiosReadDTO
    {
        public int _id { get; set; }
        public string Nombre { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public bool IsActive { get; set; }
    }
}
