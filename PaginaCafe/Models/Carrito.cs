namespace PaginaCafe.Models
{
    public class Carrito
    {

        public int Id { get; set; }

        // Foreign key a Usuario
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; }

        public DateTime FechaCreacion { get; set; }

        // Items del carrito
        public ICollection<CarritoItem> CarritoItems { get; set; }
    }
}
