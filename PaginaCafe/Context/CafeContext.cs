using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//using MVCBasico.Models;
using System.Collections.Generic;
using PaginaCafe.Models;
namespace PaginaCafe.Context
{

        public class CafeContext : DbContext
        {
            public CafeContext(DbContextOptions<CafeContext>
           options) : base(options)
            {
            }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Carrito> Carrito { get; set; }
        public DbSet<CarritoItem> CarritoItems { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<PedidoItem> PedidoItems { get; set; }
    }


}




