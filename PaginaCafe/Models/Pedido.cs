namespace PaginaCafe.Models
{
    public class Pedido
    {
        public int Id { get; set; }

        // Foreign key a Usuario
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; }

        public DateTime Fecha { get; set; }
        public decimal Total { get; set; }

        public ICollection<PedidoItem> PedidoItems { get; set; }
    }
}
