using PROG3270_GroupProject.Models;

namespace PROG3270_GroupProject.Interfaces
{
    public interface ICartService
    {
        Task<IEnumerable<Cart>> GetAllCartsAsync();
        Task<Cart> GetCartByIdAsync(int id);
        Task<IEnumerable<Cart>> GetCartsByUserIdAsync(int userId);
        Task<Cart> CreateCartAsync(CartCreateDto cartDto);
        Task<bool> UpdateCartAsync(int id, Cart cart);
        Task<bool> DeleteCartAsync(int id);
        Task<bool> RemoveItemFromCartAsync(int cartId, int productId);
        Task<decimal> CalculateTotalAsync(int cartId, bool isRegisteredUser);
        Task<Cart> CheckoutCartAsync(int cartId);
    }
};