using PROG3270_GroupProject.Application.Interfaces;
using PROG3270_GroupProject.Domain.Entities;
using PROG3270_GroupProject.Infrastructure.Interfaces;

namespace PROG3270_GroupProject.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _productRepository.GetAllAsync();
        }

        public async Task AddProductAsync(Product product)
        {
            await _productRepository.AddAsync(product);
        }

        public async Task<Product> GetProductAsync(int productId)
        {
            return await _productRepository.GetAsync(productId);
        }
    }
}
