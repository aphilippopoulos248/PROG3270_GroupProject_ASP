using PROG3270_GroupProject.Interfaces;
using PROG3270_GroupProject.Models;

namespace PROG3270_GroupProject.Services;

public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly ILogger<CartService> _logger;
        private const decimal REGISTERED_USER_DISCOUNT = 0.10m; // 10% discount for registered users

        public CartService(ICartRepository cartRepository, ILogger<CartService> logger)
        {
            _cartRepository = cartRepository ?? throw new ArgumentNullException(nameof(cartRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<Cart>> GetAllCartsAsync()
        {
            _logger.LogInformation("Getting all carts");
            return await _cartRepository.GetAllCartsAsync();
        }

        public async Task<Cart> GetCartByIdAsync(int id)
        {
            _logger.LogInformation($"Getting cart with ID: {id}");
            return await _cartRepository.GetCartByIdAsync(id);
        }

        public async Task<IEnumerable<Cart>> GetCartsByUserIdAsync(int userId)
        {
            _logger.LogInformation($"Getting carts for user ID: {userId}");
            return await _cartRepository.GetCartsByUserIdAsync(userId);
        }

        public async Task<Cart> CreateCartAsync(CartCreateDto cartDto)
        {
            _logger.LogInformation($"Creating new cart for user ID: {cartDto.UserId}");
            
            try
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

                return await _cartRepository.AddCartAsync(cart);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating cart for user ID: {cartDto.UserId}");
                throw;
            }
        }

        public async Task<bool> UpdateCartAsync(int id, Cart cart)
        {
            if (id != cart.Id)
            {
                _logger.LogWarning($"Cart ID mismatch: {id} vs {cart.Id}");
                return false;
            }

            _logger.LogInformation($"Updating cart with ID: {id}");
            return await _cartRepository.UpdateCartAsync(cart);
        }

        public async Task<bool> DeleteCartAsync(int id)
        {
            _logger.LogInformation($"Deleting cart with ID: {id}");
            return await _cartRepository.DeleteCartAsync(id);
        }

        public async Task<bool> RemoveItemFromCartAsync(int cartId, int productId)
        {
            _logger.LogInformation($"Removing product {productId} from cart {cartId}");
            return await _cartRepository.RemoveItemFromCartAsync(cartId, productId);
        }

        public async Task<decimal> CalculateTotalAsync(int cartId, bool isRegisteredUser)
        {
            _logger.LogInformation($"Calculating total for cart {cartId}, registered user: {isRegisteredUser}");
            
            var simulatedProductPrices = new Dictionary<int, decimal>
            {
                { 1, 29.99m },
                { 2, 49.99m },
                { 3, 19.99m },
                { 4, 99.99m },
                { 5, 14.99m }
            };
            
            var cart = await _cartRepository.GetCartByIdAsync(cartId);
            if (cart == null || cart.IsEmpty)
            {
                _logger.LogWarning($"Cart {cartId} is null or empty");
                return 0;
            }

            decimal subtotal = 0;
            foreach (var item in cart.Products)
            {
                if (simulatedProductPrices.TryGetValue(item.ProductId, out decimal price))
                {
                    subtotal += price * item.Quantity;
                }
                else
                {
                    // Default price if not in our simulation
                    subtotal += 10.00m * item.Quantity;
                }
            }

            if (isRegisteredUser)
            {
                decimal discount = subtotal * REGISTERED_USER_DISCOUNT;
                decimal total = subtotal - discount;
                _logger.LogInformation($"Applied {REGISTERED_USER_DISCOUNT*100}% discount to cart {cartId}. Subtotal: ${subtotal}, Discount: ${discount}, Total: ${total}");
                return total;
            }

            _logger.LogInformation($"Cart {cartId} total: ${subtotal} (no discount applied)");
            return subtotal;
        }

        public async Task<Cart> CheckoutCartAsync(int cartId)
        {
            _logger.LogInformation($"Processing checkout for cart: {cartId}");
            
            try
            {
                var cart = await _cartRepository.GetCartByIdAsync(cartId);
                if (cart == null || cart.IsEmpty)
                {
                    _logger.LogWarning($"Cannot checkout cart {cartId}: cart is null or empty");
                    return null;
                }
                
                _logger.LogInformation($"Checkout completed for cart: {cartId}");
                
                return cart;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error during checkout for cart: {cartId}");
                throw;
            }
        }
    }