using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PROG3270_GroupProject.Application.Interfaces;
using PROG3270_GroupProject.Domain.Entities;

namespace PROG3270_GroupProject.API.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _productService.GetAllProductsAsync();
            return Ok(products);
        }

        [HttpPost]
        public async Task<IActionResult> Add(Product product)
        {
            await _productService.AddProductAsync(product);
            return CreatedAtAction(nameof(GetAll), new { id = product.ProductID }, product);
        }
    }
}
