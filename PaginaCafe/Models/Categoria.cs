namespace PaginaCafe.Models
{
    public class Categoria
    {
        public int Id { get; set; }
        public string Nombre { get; set; }

        // Relación 1 a muchos: una categoría tiene muchos productos
        public ICollection<Producto> Productos { get; set; }
    }
}

