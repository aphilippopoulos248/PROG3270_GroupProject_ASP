using PROG3270_GroupProject.Domain.Entities;
using System;
using PROG3270_GroupProject.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using PROG3270_GroupProject.Infrastructure.Interfaces;


namespace PROG3270_GroupProject.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ProjectContext _context;

        public ProductRepository(ProjectContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _context.Products.ToListAsync();
        }

        public async Task AddAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }
    }
}
