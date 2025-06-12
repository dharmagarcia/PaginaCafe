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

        public class Context : DbContext
        {
            public Context(DbContextOptions<Context>
           options) : base(options)
            {
            }
            public DbSet<Carrito> carrito { get; set; }

              public DbSet<CarritoItem> carritoItem { get; set; }
              public DbSet<Pago> pago { get; set; }
              public DbSet<Pedido> pedito { get; set; }
                public DbSet<Producto> producto { get; set; }
             public DbSet<Usuario> usuario { get; set; }




    }
}



