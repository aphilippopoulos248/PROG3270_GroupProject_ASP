// Controllers/CartController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PROG3270_GroupProject.Data;
using PROG3270_GroupProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PROG3270_GroupProject.Controllers
{
    // Use the same route as Fake Store API: /carts
    [Route("carts")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ProjectContext _context;

        public CartController(ProjectContext context)
        {
            _context = context;
        }

        // GET /carts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cart>>> GetCarts()
        {
            var carts = await _context.Carts
                .Include(c => c.Products)
                .ToListAsync();
            return Ok(carts);
        }

        // GET /carts/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Cart>> GetCart(int id)
        {
            var cart = await _context.Carts
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cart == null)
                return NotFound();

            return Ok(cart);
        }

        // GET /carts/user/{userId}
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<Cart>>> GetCartsByUser(int userId)
        {
            var carts = await _context.Carts
                .Include(c => c.Products)
                .Where(c => c.UserId == userId)
                .ToListAsync();

            if (carts == null || carts.Count == 0)
                return NotFound();

            return Ok(carts);
        }

        // POST /carts
        [HttpPost]
        public async Task<ActionResult<Cart>> AddCart([FromBody] CartCreateDto cartDto)
        {
            // Create new Cart instance (ignore any client-supplied ID)
            var cart = new Cart
            {
                UserId = cartDto.UserId,
                Date = DateTime.Now,
                Products = cartDto.Products
                    .Select(p => new CartProduct 
                    { 
                        ProductId = p.ProductId, 
                        Quantity = p.Quantity 
                    })
                    .ToList()
            };

            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();

            // Set CartId for each product after save (if needed)
            foreach (var product in cart.Products)
                product.CartId = cart.Id;

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCart), new { id = cart.Id }, cart);
        }

        // PUT /carts/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCart(int id, [FromBody] Cart updatedCart)
        {
            if (id != updatedCart.Id)
                return BadRequest();

            var cart = await _context.Carts
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cart == null)
                return NotFound();

            cart.Date = DateTime.Now;
            cart.UserId = updatedCart.UserId;

            // Remove products not in the update list.
            var updatedProductIds = updatedCart.Products.Select(p => p.ProductId).ToList();
            var productsToRemove = cart.Products.Where(p => !updatedProductIds.Contains(p.ProductId)).ToList();
            foreach (var prod in productsToRemove)
                cart.Products.Remove(prod);

            // Update existing products or add new ones.
            foreach (var updatedProduct in updatedCart.Products)
            {
                var existingProduct = cart.Products.FirstOrDefault(p => p.ProductId == updatedProduct.ProductId);
                if (existingProduct != null)
                    existingProduct.Quantity = updatedProduct.Quantity;
                else
                {
                    updatedProduct.CartId = cart.Id;
                    cart.Products.Add(updatedProduct);
                }
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE /carts/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCart(int id)
        {
            var cart = await _context.Carts.FindAsync(id);
            if (cart == null)
                return NotFound();

            _context.Carts.Remove(cart);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE /carts/{cartId}/product/{productId}
        [HttpDelete("{cartId}/product/{productId}")]
        public async Task<IActionResult> RemoveItemFromCart(int cartId, int productId)
        {
            var cart = await _context.Carts
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == cartId);
            if (cart == null)
                return NotFound();

            var product = cart.Products.FirstOrDefault(p => p.ProductId == productId);
            if (product == null)
                return NotFound();

            cart.Products.Remove(product);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
