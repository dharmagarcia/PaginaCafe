﻿namespace PaginaCafe.Models
{
    public class CarritoItem
    {
        public int Id { get; set; }

        // Foreign key a Carrito
        public int CarritoId { get; set; }
        public Carrito Carrito { get; set; }

        // Foreign key a Producto
        public int ProductoId { get; set; }
        public Producto Producto { get; set; }

        public int Cantidad { get; set; }

    }
}
