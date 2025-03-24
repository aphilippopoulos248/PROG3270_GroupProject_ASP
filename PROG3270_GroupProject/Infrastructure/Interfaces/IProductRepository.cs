using PROG3270_GroupProject.Domain.Entities;

namespace PROG3270_GroupProject.Infrastructure.Interfaces
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllAsync();
        Task AddAsync(Product product);
        Task<Product> GetAsync(int productId);
    }
}
