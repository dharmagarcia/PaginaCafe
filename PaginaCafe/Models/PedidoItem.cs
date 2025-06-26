namespace PaginaCafe.Models
{
    public class PedidoItem
    {
        public int Id { get; set; }

        // Foreign key a Pedido
        public int PedidoId { get; set; }
        public Pedido Pedido { get; set; }

        // Foreign key a Producto
        public int ProductoId { get; set; }
        public Producto Producto { get; set; }

        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
    }
}
