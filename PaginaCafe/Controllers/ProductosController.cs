using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PaginaCafe.Context;
using PaginaCafe.Models;

namespace PaginaCafe.Controllers
{
    public class ProductosController : Controller
    {
        private readonly CafeContext _context;

        public ProductosController(CafeContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var productos = _context.Productos.ToList();
            return View(productos);
        }

        [HttpPost]
        public async Task<IActionResult> Elegir([FromBody] ElegirRequest request)
        {
            var producto = await _context.Productos.FirstOrDefaultAsync(p => p.Id == request.Id);
            if (producto == null)
                return NotFound(new { mensaje = "Producto no encontrado" });

            int usuarioId = request.UsuarioId;

            // Obtener o crear carrito
            var carrito = await _context.Carrito
                .Include(c => c.CarritoItems)
                .FirstOrDefaultAsync(c => c.UsuarioId == usuarioId);

            if (carrito == null)
            {
                carrito = new Carrito
                {
                    UsuarioId = usuarioId,
                    FechaCreacion = DateTime.Now,
                    CarritoItems = new List<CarritoItem>()
                };
                _context.Carrito.Add(carrito);
            }

            var item = carrito.CarritoItems.FirstOrDefault(ci => ci.ProductoId == request.Id);
            if (item != null)
            {
                item.Cantidad++;
            }
            else
            {
                carrito.CarritoItems.Add(new CarritoItem
                {
                    ProductoId = producto.Id,
                    Cantidad = 1
                });
            }

            await _context.SaveChangesAsync();

            return Ok(new { mensaje = $"Agregado al carrito: {producto.Nombre}" });
        }

        public class ElegirRequest
        {
            public int Id { get; set; }
            public int UsuarioId { get; set; }
        }
        [HttpGet]
        public async Task<IActionResult> ObtenerCarrito(int usuarioId)
        {
            var carrito = await _context.Carrito
                .Include(c => c.CarritoItems)
                    .ThenInclude(ci => ci.Producto)
                .FirstOrDefaultAsync(c => c.UsuarioId == usuarioId);

            if (carrito == null)
            {
                carrito = new Carrito
                {
                    UsuarioId = usuarioId,
                    CarritoItems = new List<CarritoItem>()
                };
            }

            return View("VerCarrito", carrito); // Usa la vista compartida
        }


        [HttpPost]
        public async Task<IActionResult> RealizarPedido(int usuarioId)
        {
            try
            {
                var carrito = await _context.Carrito
                    .Include(c => c.CarritoItems)
                    .ThenInclude(ci => ci.Producto)
                    .FirstOrDefaultAsync(c => c.UsuarioId == usuarioId);

                if (carrito == null || !carrito.CarritoItems.Any())
                {
                    TempData["Mensaje"] = "Tu carrito está vacío.";
                    return RedirectToAction("ObtenerCarrito", new { usuarioId });
                }

                var pedido = new Pedido
                {
                    UsuarioId = usuarioId,
                    Fecha = DateTime.Now,
                    Total = carrito.CarritoItems.Sum(ci => ci.Cantidad * ci.Producto.Precio),
                    PedidoItems = carrito.CarritoItems.Select(ci => new PedidoItem
                    {
                        ProductoId = ci.ProductoId,
                        Cantidad = ci.Cantidad,
                        PrecioUnitario = ci.Producto.Precio
                    }).ToList()
                };

                _context.Pedidos.Add(pedido);
                _context.CarritoItems.RemoveRange(carrito.CarritoItems);
                await _context.SaveChangesAsync();

                TempData["Mensaje"] = "¡Pedido realizado con éxito!";
                return RedirectToAction("ObtenerCarrito", new { usuarioId });
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = "Error al realizar el pedido: " + ex.Message;
                return RedirectToAction("ObtenerCarrito", new { usuarioId });
            }
        }


    }
}
