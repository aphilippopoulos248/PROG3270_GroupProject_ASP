using Microsoft.EntityFrameworkCore;
using PROG3270_GroupProject.Data;
using PROG3270_GroupProject.Interfaces;
using PROG3270_GroupProject.Models;

namespace PROG3270_GroupProject.Repositories;

public class CartRepository : ICartRepository
    {
        private readonly ProjectContext _context;
        private readonly ILogger<CartRepository> _logger;

        public CartRepository(ProjectContext context, ILogger<CartRepository> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<Cart>> GetAllCartsAsync()
        {
            _logger.LogInformation("Getting all carts");
            return await _context.Carts
                .Include(c => c.Products)
                .ToListAsync();
        }

        public async Task<Cart> GetCartByIdAsync(int id)
        {
            _logger.LogInformation($"Getting cart with ID: {id}");
            return await _context.Carts
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<Cart>> GetCartsByUserIdAsync(int userId)
        {
            _logger.LogInformation($"Getting carts for user ID: {userId}");
            return await _context.Carts
                .Include(c => c.Products)
                .Where(c => c.UserId == userId)
                .ToListAsync();
        }

        public async Task<Cart> AddCartAsync(Cart cart)
        {
            _logger.LogInformation($"Adding new cart for user ID: {cart.UserId}");
            
            try
            {
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();

                // Set CartId for each product after save
                foreach (var product in cart.Products)
                    product.CartId = cart.Id;

                await _context.SaveChangesAsync();
                
                _logger.LogInformation($"Successfully added cart with ID: {cart.Id}");
                return cart;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error adding cart for user ID: {cart.UserId}");
                throw;
            }
        }

        public async Task<bool> UpdateCartAsync(Cart cart)
        {
            _logger.LogInformation($"Updating cart with ID: {cart.Id}");
            
            try
            {
                var existingCart = await _context.Carts
                    .Include(c => c.Products)
                    .FirstOrDefaultAsync(c => c.Id == cart.Id);

                if (existingCart == null)
                {
                    _logger.LogWarning($"Cart not found with ID: {cart.Id}");
                    return false;
                }

                existingCart.Date = DateTime.Now;
                existingCart.UserId = cart.UserId;

                // Remove products not in the update list
                var updatedProductIds = cart.Products.Select(p => p.ProductId).ToList();
                var productsToRemove = existingCart.Products
                    .Where(p => !updatedProductIds.Contains(p.ProductId))
                    .ToList();
                
                foreach (var prod in productsToRemove)
                    existingCart.Products.Remove(prod);

                // Update existing products or add new ones
                foreach (var updatedProduct in cart.Products)
                {
                    var existingProduct = existingCart.Products
                        .FirstOrDefault(p => p.ProductId == updatedProduct.ProductId);
                    
                    if (existingProduct != null)
                        existingProduct.Quantity = updatedProduct.Quantity;
                    else
                    {
                        updatedProduct.CartId = existingCart.Id;
                        existingCart.Products.Add(updatedProduct);
                    }
                }

                await _context.SaveChangesAsync();
                _logger.LogInformation($"Successfully updated cart with ID: {cart.Id}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating cart with ID: {cart.Id}");
                throw;
            }
        }

        public async Task<bool> DeleteCartAsync(int id)
        {
            _logger.LogInformation($"Deleting cart with ID: {id}");
            
            try
            {
                var cart = await _context.Carts.FindAsync(id);
                if (cart == null)
                {
                    _logger.LogWarning($"Cart not found with ID: {id}");
                    return false;
                }

                _context.Carts.Remove(cart);
                await _context.SaveChangesAsync();
                
                _logger.LogInformation($"Successfully deleted cart with ID: {id}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting cart with ID: {id}");
                throw;
            }
        }

        public async Task<bool> RemoveItemFromCartAsync(int cartId, int productId)
        {
            _logger.LogInformation($"Removing product {productId} from cart {cartId}");
            
            try
            {
                var cart = await _context.Carts
                    .Include(c => c.Products)
                    .FirstOrDefaultAsync(c => c.Id == cartId);
                
                if (cart == null)
                {
                    _logger.LogWarning($"Cart not found with ID: {cartId}");
                    return false;
                }

                var product = cart.Products.FirstOrDefault(p => p.ProductId == productId);
                if (product == null)
                {
                    _logger.LogWarning($"Product {productId} not found in cart {cartId}");
                    return false;
                }

                cart.Products.Remove(product);
                await _context.SaveChangesAsync();
                
                _logger.LogInformation($"Successfully removed product {productId} from cart {cartId}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error removing product {productId} from cart {cartId}");
                throw;
            }
        }
    }