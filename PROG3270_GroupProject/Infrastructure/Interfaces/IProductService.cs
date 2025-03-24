using PROG3270_GroupProject.Domain.Entities;

namespace PROG3270_GroupProject.Application.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task AddProductAsync(Product product);
        Task<Product> GetProductAsync(int productId);
    }
}
