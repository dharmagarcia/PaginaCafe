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

        // Mostrar todos los productos
        public IActionResult Index()
        {
            var productos = _context.Productos.ToList();
            return View(productos);
        }

        // Acción llamada desde JS para agregar producto al carrito usando nombre
        [HttpPost]
        public async Task<IActionResult> ElegirPorNombre([FromBody] ElegirPorNombreRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Nombre))
                return BadRequest(new { mensaje = "Nombre inválido." });

            // Buscar o crear usuario
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Nombre == request.Nombre);
            if (usuario == null)
            {
                usuario = new Usuario
                {
                    Nombre = request.Nombre,
                    Contrasena = "default123", 
                    Email = $"{request.Nombre.ToLower()}@ejemplo.com"
                };
                _context.Usuarios.Add(usuario);
                await _context.SaveChangesAsync();
            }

            // Buscar producto
            var producto = await _context.Productos.FirstOrDefaultAsync(p => p.Id == request.Id);
            if (producto == null)
                return NotFound(new { mensaje = "Producto no encontrado." });

            // Obtener o crear carrito
            var carrito = await _context.Carrito
                .Include(c => c.CarritoItems)
                .FirstOrDefaultAsync(c => c.UsuarioId == usuario.Id);

            if (carrito == null)
            {
                carrito = new Carrito
                {
                    UsuarioId = usuario.Id,
                    FechaCreacion = DateTime.Now,
                    CarritoItems = new List<CarritoItem>()
                };
                _context.Carrito.Add(carrito);
            }

            // Agregar producto al carrito
            var item = carrito.CarritoItems.FirstOrDefault(ci => ci.ProductoId == request.Id);
            if (item != null)
            {
                item.Cantidad++;
            }
            else
            {
                carrito.CarritoItems.Add(new CarritoItem
                {
                    ProductoId = request.Id,
                    Cantidad = 1
                });
            }

            await _context.SaveChangesAsync();

            return Ok(new { mensaje = $"Agregado al carrito: {producto.Nombre}" });
        }


        // Mostrar el carrito de un usuario por su nombre
        [HttpGet]
        public async Task<IActionResult> ObtenerCarritoPorNombre(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
            {
                TempData["Mensaje"] = "Nombre de usuario inválido.";
                return RedirectToAction("Index");
            }

            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Nombre == nombre);
            if (usuario == null)
            {
              usuario = new Usuario
                        {
                     Nombre = nombre,
                     Contrasena = "default123", // ⚠️ temporal
                     Email = nombre + "@ejemplo.com" // ⚠️ temporal
                        };

                _context.Usuarios.Add(usuario);
                await _context.SaveChangesAsync();
            }

            var carrito = await _context.Carrito
                .Include(c => c.CarritoItems)
                .ThenInclude(ci => ci.Producto)
                .FirstOrDefaultAsync(c => c.UsuarioId == usuario.Id);

            if (carrito == null)
            {
                carrito = new Carrito
                {
                    UsuarioId = usuario.Id,
                    FechaCreacion = DateTime.Now,
                    CarritoItems = new List<CarritoItem>()
                };
                _context.Carrito.Add(carrito);
                await _context.SaveChangesAsync();
            }

            ViewBag.UsuarioId = usuario.Id;
            return View("VerCarrito", carrito);
        }

        // Realiza el pedido y vacía el carrito
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

        // ✅ Este método puede quedar si querés usar usuarioId directamente
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

            return View("VerCarrito", carrito);
        }

        // Request DTO
        public class ElegirPorNombreRequest
        {
            public int Id { get; set; }
            public string Nombre { get; set; }
        }
    }
}
