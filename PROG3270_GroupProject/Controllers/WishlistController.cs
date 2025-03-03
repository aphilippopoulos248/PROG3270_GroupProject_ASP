// Controllers/WishlistController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PROG3270_GroupProject.Data;
using PROG3270_GroupProject.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PROG3270_GroupProject.Controllers
{
    [Route("api/wishlists")]
    [ApiController]
    public class WishlistController : ControllerBase
    {
        private readonly ProjectContext _context;

        public WishlistController(ProjectContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Wishlist>>> GetWishlists()
        {
            var wishlists = await _context.Wishlists.Include(w => w.Items).ToListAsync();
            return Ok(wishlists);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Wishlist>> GetWishlist(int id)
        {
            var wishlist = await _context.Wishlists.Include(w => w.Items)
                                                   .FirstOrDefaultAsync(w => w.Id == id);
            if (wishlist == null)
                return NotFound();

            return Ok(wishlist);
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<Wishlist>>> GetWishlistsByUser(int userId)
        {
            var wishlists = await _context.Wishlists.Include(w => w.Items)
                                                    .Where(w => w.UserId == userId)
                                                    .ToListAsync();
            if (wishlists == null || wishlists.Count == 0)
                return NotFound();

            return Ok(wishlists);
        }

        [HttpPost]
        public async Task<ActionResult<Wishlist>> AddWishlist([FromBody] Wishlist wishlist)
        {
            wishlist.Items ??= new List<WishlistItem>();
            _context.Wishlists.Add(wishlist);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetWishlist), new { id = wishlist.Id }, wishlist);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateWishlist(int id, [FromBody] Wishlist updatedWishlist)
        {
            if (id != updatedWishlist.Id)
                return BadRequest();

            var wishlist = await _context.Wishlists.Include(w => w.Items)
                                                   .FirstOrDefaultAsync(w => w.Id == id);
            if (wishlist == null)
                return NotFound();

            wishlist.UserId = updatedWishlist.UserId;

            var updatedProductIds = updatedWishlist.Items.Select(i => i.ProductId).ToList();
            var itemsToRemove = wishlist.Items.Where(i => !updatedProductIds.Contains(i.ProductId)).ToList();
            foreach (var item in itemsToRemove)
                wishlist.Items.Remove(item);

            foreach (var updatedItem in updatedWishlist.Items)
            {
                if (!wishlist.Items.Any(i => i.ProductId == updatedItem.ProductId))
                {
                    updatedItem.WishlistId = wishlist.Id;
                    wishlist.Items.Add(updatedItem);
                }
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWishlist(int id)
        {
            var wishlist = await _context.Wishlists.FindAsync(id);
            if (wishlist == null)
                return NotFound();

            _context.Wishlists.Remove(wishlist);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{wishlistId}/item/{productId}")]
        public async Task<IActionResult> RemoveItemFromWishlist(int wishlistId, int productId)
        {
            var wishlist = await _context.Wishlists.Include(w => w.Items)
                                                   .FirstOrDefaultAsync(w => w.Id == wishlistId);
            if (wishlist == null)
                return NotFound();

            var item = wishlist.Items.FirstOrDefault(i => i.ProductId == productId);
            if (item == null)
                return NotFound();

            wishlist.Items.Remove(item);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
