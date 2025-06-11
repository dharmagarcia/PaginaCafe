namespace PaginaCafe.Models
{
    public class Carrito
    {

        private String carritoId;
        private int cantidad;
        private Pago pago;
        private List<Producto> productos;
        private CarritoItem items;
    }
}
