using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PROG3270_GroupProject.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PROG3270_GroupProject.Interfaces;

namespace PROG3270_GroupProject.Controllers
{
    [Route("carts")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly ILogger<CartController> _logger;

        public CartController(ICartService cartService, ILogger<CartController> logger)
        {
            _cartService = cartService ?? throw new ArgumentNullException(nameof(cartService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // GET /carts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cart>>> GetCarts()
        {
            _logger.LogInformation("GET request received for all carts");
            var carts = await _cartService.GetAllCartsAsync();
            return Ok(carts);
        }

        // GET /carts/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Cart>> GetCart(int id)
        {
            _logger.LogInformation($"GET request received for cart {id}");
            var cart = await _cartService.GetCartByIdAsync(id);

            if (cart == null)
            {
                _logger.LogWarning($"Cart with ID {id} not found");
                return NotFound();
            }

            return Ok(cart);
        }

        // GET /carts/user/{userId}
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<Cart>>> GetCartsByUser(int userId)
        {
            _logger.LogInformation($"GET request received for carts of user {userId}");
            var carts = await _cartService.GetCartsByUserIdAsync(userId);

            if (carts == null)
            {
                _logger.LogWarning($"No carts found for user ID {userId}");
                return NotFound();
            }

            return Ok(carts);
        }

        // POST /carts
        [HttpPost]
        public async Task<ActionResult<Cart>> AddCart([FromBody] CartCreateDto cartDto)
        {
            _logger.LogInformation($"POST request received to create cart for user {cartDto.UserId}");
            
            try
            {
                var cart = await _cartService.CreateCartAsync(cartDto);
                _logger.LogInformation($"Created cart with ID {cart.Id}");
                return CreatedAtAction(nameof(GetCart), new { id = cart.Id }, cart);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating cart");
                return StatusCode(500, "An error occurred while creating the cart");
            }
        }

        // PUT /carts/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCart(int id, [FromBody] Cart updatedCart)
        {
            _logger.LogInformation($"PUT request received to update cart {id}");
            
            if (id != updatedCart.Id)
            {
                _logger.LogWarning($"Cart ID mismatch: URL ID {id} vs. body ID {updatedCart.Id}");
                return BadRequest("The ID in the URL must match the ID in the request body");
            }

            try
            {
                var result = await _cartService.UpdateCartAsync(id, updatedCart);
                
                if (!result)
                {
                    _logger.LogWarning($"Cart with ID {id} not found for update");
                    return NotFound();
                }

                _logger.LogInformation($"Successfully updated cart {id}");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating cart {id}");
                return StatusCode(500, "An error occurred while updating the cart");
            }
        }

        // DELETE /carts/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCart(int id)
        {
            _logger.LogInformation($"DELETE request received for cart {id}");
            
            try
            {
                var result = await _cartService.DeleteCartAsync(id);
                
                if (!result)
                {
                    _logger.LogWarning($"Cart with ID {id} not found for deletion");
                    return NotFound();
                }

                _logger.LogInformation($"Successfully deleted cart {id}");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting cart {id}");
                return StatusCode(500, "An error occurred while deleting the cart");
            }
        }

        // DELETE /carts/{cartId}/product/{productId}
        [HttpDelete("{cartId}/product/{productId}")]
        public async Task<IActionResult> RemoveItemFromCart(int cartId, int productId)
        {
            _logger.LogInformation($"DELETE request received to remove product {productId} from cart {cartId}");
            
            try
            {
                var result = await _cartService.RemoveItemFromCartAsync(cartId, productId);
                
                if (!result)
                {
                    _logger.LogWarning($"Cart {cartId} or product {productId} not found");
                    return NotFound();
                }

                _logger.LogInformation($"Successfully removed product {productId} from cart {cartId}");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error removing product {productId} from cart {cartId}");
                return StatusCode(500, "An error occurred while removing the item from the cart");
            }
        }

        // POST /carts/{id}/calculate
        [HttpPost("{id}/calculate")]
        public async Task<ActionResult<decimal>> CalculateTotal(int id, [FromBody] CalculateTotalDto calculateDto)
        {
            _logger.LogInformation($"POST request to calculate total for cart {id}");
            
            try
            {
                var total = await _cartService.CalculateTotalAsync(id, calculateDto.IsRegisteredUser);
                return Ok(new { Total = total });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error calculating total for cart {id}");
                return StatusCode(500, "An error occurred while calculating the total");
            }
        }

        // POST /carts/{id}/checkout
        [HttpPost("{id}/checkout")]
        public async Task<ActionResult<OrderDto>> CheckoutCart(int id, [FromBody] CheckoutDto checkoutDto)
        {
            _logger.LogInformation($"POST request to checkout cart {id}");
            
            try
            {
                var cart = await _cartService.CheckoutCartAsync(id);
                
                if (cart == null)
                {
                    _logger.LogWarning($"Cart {id} not found or empty");
                    return NotFound("Cart not found or is empty");
                }

                // Calculate totals
                decimal subtotal = await _cartService.CalculateTotalAsync(id, false); // Calculate without discount
                decimal total = await _cartService.CalculateTotalAsync(id, checkoutDto.IsRegisteredUser); // Calculate with or without discount
                decimal discount = subtotal - total;

                // Create order response
                var order = new OrderDto
                {
                    CartId = cart.Id,
                    UserId = cart.UserId,
                    IsRegisteredUser = checkoutDto.IsRegisteredUser,
                    Subtotal = subtotal,
                    Discount = discount,
                    Total = total,
                    OrderDate = DateTime.Now,
                    Items = cart.Products
                };

                _logger.LogInformation($"Successfully checked out cart {id}");
                return Ok(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error checking out cart {id}");
                return StatusCode(500, "An error occurred during checkout");
            }
        }
    }
}