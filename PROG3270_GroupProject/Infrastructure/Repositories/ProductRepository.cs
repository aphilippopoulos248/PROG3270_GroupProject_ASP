using PROG3270_GroupProject.Domain.Entities;
using System;
using PROG3270_GroupProject.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using PROG3270_GroupProject.Infrastructure.Interfaces;
using Newtonsoft.Json;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using PROG3270_GroupProject.Models;


namespace PROG3270_GroupProject.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ProjectContext _context;
        private readonly HttpClient _httpClient;

        public ProductRepository(ProjectContext context, HttpClient httpClient)
        {
            _context = context;
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            var response = await _httpClient.GetStringAsync("https://fakestoreapi.com/products");
            var products = JsonConvert.DeserializeObject<List<Product>>(response);
            return products;
        }

        public async Task AddAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }

        public async Task<Product> GetAsync(int productId)
        {
            var response = await _httpClient.GetStringAsync($"https://fakestoreapi.com/products/{productId}");
            Console.WriteLine(response); 
            var product = JsonConvert.DeserializeObject<Product>(response);
            return product;
        }

    }
}
