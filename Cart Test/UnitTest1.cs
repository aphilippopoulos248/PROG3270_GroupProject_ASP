using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PROG3270_GroupProject.Controllers;
using PROG3270_GroupProject.Data;
using PROG3270_GroupProject.Models;


namespace PROG3270_GroupProject.Tests
{
    public class CartControllerTests
    {
        //This will initialize firstly the data necessary to execute and test processes. 
        private ProjectContext GetContextWithData()
        {
            var options = new DbContextOptionsBuilder<ProjectContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new ProjectContext(options);

            // Seed test data
            context.Carts.Add(new Cart {
                Id = 1,
                UserId = 1,
                Date = DateTime.Now,
                Products = new List<CartProduct> 
                { 
                    new CartProduct { CartId = 1, ProductId = 101, Quantity = 2 } 
                }
            });
            context.Carts.Add(new Cart {
                Id = 2,
                UserId = 2,
                Date = DateTime.Now,
                Products = new List<CartProduct> 
                { 
                    new CartProduct { CartId = 2, ProductId = 102, Quantity = 1 } 
                }
            });
            context.SaveChanges();
            return context;
        }

        [Fact]
        public async Task GetCarts_ReturnsAllCarts()
        {
            var context = GetContextWithData();
            var controller = new CartController(context);

            var result = await controller.GetCarts();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var carts = Assert.IsAssignableFrom<IEnumerable<Cart>>(okResult.Value);
            Assert.Equal(2, carts.Count());
        }

        [Fact]
        public async Task GetCart_ReturnsNotFound_WhenCartDoesNotExist()
        {
            var context = GetContextWithData();
            var controller = new CartController(context);

            var result = await controller.GetCart(999);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetCart_ReturnsCart_WhenCartExists()
        {
            var context = GetContextWithData();
            var controller = new CartController(context);

            var result = await controller.GetCart(1);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var cart = Assert.IsType<Cart>(okResult.Value);
            Assert.Equal(1, cart.Id);
        }

        [Fact]
        public async Task GetCartsByUser_ReturnsNotFound_WhenNoCartsForUser()
        {
            var context = GetContextWithData();
            var controller = new CartController(context);

            var result = await controller.GetCartsByUser(999);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetCartsByUser_ReturnsCarts_WhenCartsExistForUser()
        {
            var context = GetContextWithData();
            var controller = new CartController(context);

            var result = await controller.GetCartsByUser(1);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var carts = Assert.IsAssignableFrom<IEnumerable<Cart>>(okResult.Value);
            Assert.Single(carts);
        }

        [Fact]
        public async Task AddCart_CreatesNewCart()
        {
            var options = new DbContextOptionsBuilder<ProjectContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var context = new ProjectContext(options);
            var controller = new CartController(context);
            var cartDto = new CartCreateDto
            {
                UserId = 3,
                Products = new List<CartProductCreateDto>
                {
                    new CartProductCreateDto { ProductId = 201, Quantity = 3 }
                }
            };

            var result = await controller.AddCart(cartDto);

            var createdAtResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var cart = Assert.IsType<Cart>(createdAtResult.Value);
            Assert.Equal(3, cart.UserId);
            Assert.Single(cart.Products);
            Assert.Equal(201, cart.Products.First().ProductId);
        }

        [Fact]
        public async Task UpdateCart_ReturnsBadRequest_WhenIdMismatch()
        {
            var context = GetContextWithData();
            var controller = new CartController(context);
            var updatedCart = new Cart
            {
                Id = 999, // mismatched ID
                UserId = 1,
                Date = DateTime.Now,
                Products = new List<CartProduct>()
            };

            var result = await controller.UpdateCart(1, updatedCart);

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task UpdateCart_ReturnsNotFound_WhenCartDoesNotExist()
        {
            var context = GetContextWithData();
            var controller = new CartController(context);
            var updatedCart = new Cart
            {
                Id = 999,
                UserId = 1,
                Date = DateTime.Now,
                Products = new List<CartProduct>()
            };

            var result = await controller.UpdateCart(999, updatedCart);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task UpdateCart_UpdatesCartSuccessfully()
        {
            var context = GetContextWithData();
            var controller = new CartController(context);
            var updatedCart = new Cart
            {
                Id = 1,
                UserId = 10,
                Date = DateTime.Now,
                Products = new List<CartProduct>
                {
                    new CartProduct { ProductId = 101, Quantity = 5 },
                    new CartProduct { ProductId = 202, Quantity = 1 }
                }
            };

            var result = await controller.UpdateCart(1, updatedCart);

            Assert.IsType<NoContentResult>(result);
            var cart = await context.Carts.Include(c => c.Products).FirstOrDefaultAsync(c => c.Id == 1);
            Assert.Equal(10, cart.UserId);
            Assert.Equal(2, cart.Products.Count);
            var prod101 = cart.Products.FirstOrDefault(p => p.ProductId == 101);
            Assert.NotNull(prod101);
            Assert.Equal(5, prod101.Quantity);
        }

        [Fact]
        public async Task DeleteCart_ReturnsNotFound_WhenCartDoesNotExist()
        {
            var context = GetContextWithData();
            var controller = new CartController(context);

            var result = await controller.DeleteCart(999);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteCart_DeletesCartSuccessfully()
        {
            var context = GetContextWithData();
            var controller = new CartController(context);

            var result = await controller.DeleteCart(1);

            Assert.IsType<NoContentResult>(result);
            var cart = await context.Carts.FindAsync(1);
            Assert.Null(cart);
        }

        [Fact]
        public async Task RemoveItemFromCart_ReturnsNotFound_WhenCartDoesNotExist()
        {
            var context = GetContextWithData();
            var controller = new CartController(context);

            var result = await controller.RemoveItemFromCart(999, 101);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task RemoveItemFromCart_ReturnsNotFound_WhenProductDoesNotExist()
        {
            var context = GetContextWithData();
            var controller = new CartController(context);

            var result = await controller.RemoveItemFromCart(1, 999);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task RemoveItemFromCart_RemovesProductSuccessfully()
        {
            var context = GetContextWithData();
            var controller = new CartController(context);

            var result = await controller.RemoveItemFromCart(1, 101);

            Assert.IsType<NoContentResult>(result);
            var cart = await context.Carts.Include(c => c.Products).FirstOrDefaultAsync(c => c.Id == 1);
            Assert.Empty(cart.Products);
        }
    }
}
