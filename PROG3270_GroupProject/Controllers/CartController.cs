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
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ProjectContext _context;

        public CartController(ProjectContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<Cart>> AddToCart([FromBody] CartCreateDto cartDto)
        {
            var cart = new Cart
            {
                UserId = cartDto.UserId,
                Date = DateTime.Now,
                Products = cartDto.Products.Select(p => new CartProduct 
                { 
                    ProductId = p.ProductId, 
                    Quantity = p.Quantity 
                }).ToList()
            };

            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();

            foreach (var product in cart.Products)
            {
                product.CartId = cart.Id;
            }
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCart), new { id = cart.Id }, cart);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Cart>> GetCart(int id)
        {
            var cart = await _context.Carts
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cart == null)
            {
                return NotFound();
            }

            return Ok(cart);
        }


        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<Cart>>> GetCartItemsByUser(int userId)
        {
            var carts = await _context.Carts
                .Include(c => c.Products)
                .Where(c => c.UserId == userId)
                .ToListAsync();

            if (carts == null || carts.Count == 0)
            {
                return NotFound();
            }

            return Ok(carts);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCart(int id, Cart updatedCart)
        {
            if (id != updatedCart.Id)
            {
                return BadRequest();
            }

            // Load existing cart with products
            var cart = await _context.Carts
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cart == null)
            {
                return NotFound();
            }

            cart.Date = DateTime.Now;
            cart.UserId = updatedCart.UserId;

            var updatedProductIds = updatedCart.Products.Select(p => p.ProductId).ToList();
            var productsToRemove = cart.Products.Where(p => !updatedProductIds.Contains(p.ProductId)).ToList();
            foreach (var prod in productsToRemove)
            {
                cart.Products.Remove(prod);
            }

            foreach (var updatedProduct in updatedCart.Products)
            {
                var existingProduct = cart.Products.FirstOrDefault(p => p.ProductId == updatedProduct.ProductId);
                if (existingProduct != null)
                {
                    existingProduct.Quantity = updatedProduct.Quantity;
                }
                else
                {
                    updatedProduct.CartId = cart.Id;
                    cart.Products.Add(updatedProduct);
                }
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{cartId}/product/{productId}")]
        public async Task<IActionResult> RemoveItemFromCart(int cartId, int productId)
        {
            var cart = await _context.Carts.FindAsync(cartId);
            if (cart == null)
            {
                return NotFound();
            }

            var product = cart.Products.FirstOrDefault(p => p.ProductId == productId);
            if (product == null)
            {
                return NotFound();
            }

            cart.Products.Remove(product);
            _context.Entry(cart).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCart(int id)
        {
            var cart = await _context.Carts.FindAsync(id);
            if (cart == null)
            {
                return NotFound();
            }

            _context.Carts.Remove(cart);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}