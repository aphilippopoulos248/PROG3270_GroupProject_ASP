using PROG3270_GroupProject.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PROG3270_GroupProject.Interfaces
{
    public interface ICartRepository
    {
        Task<IEnumerable<Cart>> GetAllCartsAsync();
        Task<Cart> GetCartByIdAsync(int id);
        Task<IEnumerable<Cart>> GetCartsByUserIdAsync(int userId);
        Task<Cart> AddCartAsync(Cart cart);
        Task<bool> UpdateCartAsync(Cart cart);
        Task<bool> DeleteCartAsync(int id);
        Task<bool> RemoveItemFromCartAsync(int cartId, int productId);
    }
};