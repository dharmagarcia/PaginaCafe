using System.Collections;
using System.ComponentModel.DataAnnotations.Schema;

namespace PaginaCafe.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Email { get; set; }

        [Column("Contraseña")]
        public string Contrasena { get; set; }  // Ojo: guardar hash en producción

        // Un usuario puede tener 0 o 1 carrito activo
        public Carrito Carrito { get; set; }

        // Un usuario puede tener muchos pedidos
        public ICollection<Pedido> Pedidos { get; set; }
    }
}
                        