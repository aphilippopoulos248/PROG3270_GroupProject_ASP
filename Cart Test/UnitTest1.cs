using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using PROG3270_GroupProject.Controllers;
using PROG3270_GroupProject.Interfaces;
using PROG3270_GroupProject.Models;

namespace Cart_Test
{
    public class CartControllerTests
    {
        private readonly Mock<ICartService> _mockCartService;
        private readonly Mock<ILogger<CartController>> _mockLogger;
        private readonly CartController _controller;
        private readonly List<Cart> _testCarts;

        public CartControllerTests()
        {
            _mockCartService = new Mock<ICartService>();
            _mockLogger = new Mock<ILogger<CartController>>();

            _controller = new CartController(_mockCartService.Object, _mockLogger.Object);

            _testCarts = new List<Cart>
            {
                new Cart
                {
                    Id = 1,
                    UserId = 1,
                    Date = DateTime.Now,
                    Products = new List<CartProduct>
                    {
                        new CartProduct { CartId = 1, ProductId = 101, Quantity = 2 }
                    }
                },
                new Cart
                {
                    Id = 2,
                    UserId = 2,
                    Date = DateTime.Now,
                    Products = new List<CartProduct>
                    {
                        new CartProduct { CartId = 2, ProductId = 102, Quantity = 1 }
                    }
                }
            };
        }

        [Fact]
        public async Task GetCarts_ReturnsAllCarts()
        {
            _mockCartService.Setup(service => service.GetAllCartsAsync())
                .ReturnsAsync(_testCarts);

            var result = await _controller.GetCarts();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var carts = Assert.IsAssignableFrom<IEnumerable<Cart>>(okResult.Value);
            Assert.Equal(2, carts.Count());

            _mockCartService.Verify(service => service.GetAllCartsAsync(), Times.Once);
        }

        [Fact]
        public async Task GetCart_ReturnsNotFound_WhenCartDoesNotExist()
        {
            _mockCartService.Setup(service => service.GetCartByIdAsync(999))
                .ReturnsAsync((Cart)null);

            var result = await _controller.GetCart(999);

            Assert.IsType<NotFoundResult>(result.Result);

            _mockCartService.Verify(service => service.GetCartByIdAsync(999), Times.Once);
        }

        [Fact]
        public async Task GetCart_ReturnsCart_WhenCartExists()
        {
            var testCart = _testCarts.First();
            _mockCartService.Setup(service => service.GetCartByIdAsync(1))
                .ReturnsAsync(testCart);

            var result = await _controller.GetCart(1);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var cart = Assert.IsType<Cart>(okResult.Value);
            Assert.Equal(1, cart.Id);

            _mockCartService.Verify(service => service.GetCartByIdAsync(1), Times.Once);
        }

        [Fact]
        public async Task GetCartsByUser_ReturnsNotFound_WhenNoCartsForUser()
        {
            _mockCartService.Setup(service => service.GetCartsByUserIdAsync(999))
                .ReturnsAsync((IEnumerable<Cart>)null);

            var result = await _controller.GetCartsByUser(999);

            Assert.IsType<NotFoundResult>(result.Result);

            _mockCartService.Verify(service => service.GetCartsByUserIdAsync(999), Times.Once);
        }

        [Fact]
        public async Task GetCartsByUser_ReturnsCarts_WhenCartsExistForUser()
        {
            var userCarts = _testCarts.Where(c => c.UserId == 1).ToList();
            _mockCartService.Setup(service => service.GetCartsByUserIdAsync(1))
                .ReturnsAsync(userCarts);

            var result = await _controller.GetCartsByUser(1);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var carts = Assert.IsAssignableFrom<IEnumerable<Cart>>(okResult.Value);
            Assert.Single(carts);

            _mockCartService.Verify(service => service.GetCartsByUserIdAsync(1), Times.Once);
        }

        [Fact]
        public async Task AddCart_CreatesNewCart()
        {
            var cartDto = new CartCreateDto
            {
                UserId = 3,
                Products = new List<CartProductCreateDto>
                {
                    new CartProductCreateDto { ProductId = 201, Quantity = 3 }
                }
            };

            var newCart = new Cart
            {
                Id = 3,
                UserId = 3,
                Date = DateTime.Now,
                Products = new List<CartProduct>
                {
                    new CartProduct { CartId = 3, ProductId = 201, Quantity = 3 }
                }
            };

            _mockCartService.Setup(service => service.CreateCartAsync(It.IsAny<CartCreateDto>()))
                .ReturnsAsync(newCart);

            var result = await _controller.AddCart(cartDto);

            var createdAtResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var cart = Assert.IsType<Cart>(createdAtResult.Value);
            Assert.Equal(3, cart.UserId);
            Assert.Single(cart.Products);
            Assert.Equal(201, cart.Products.First().ProductId);

            _mockCartService.Verify(service => service.CreateCartAsync(It.IsAny<CartCreateDto>()), Times.Once);
        }

        [Fact]
        public async Task UpdateCart_ReturnsNotFound_WhenCartDoesNotExist()
        {
            var updatedCart = new Cart
            {
                Id = 999,
                UserId = 1
            };

            _mockCartService.Setup(service => service.UpdateCartAsync(999, updatedCart))
                .ReturnsAsync(false);

            var result = await _controller.UpdateCart(999, updatedCart);

            Assert.IsType<NotFoundResult>(result);

            _mockCartService.Verify(service => service.UpdateCartAsync(999, updatedCart), Times.Once);
        }

        [Fact]
        public async Task UpdateCart_UpdatesCartSuccessfully()
        {
            var updatedCart = new Cart
            {
                Id = 1,
                UserId = 10,
                Products = new List<CartProduct>
                {
                    new CartProduct { ProductId = 101, Quantity = 5 },
                    new CartProduct { ProductId = 202, Quantity = 1 }
                }
            };

            _mockCartService.Setup(service => service.UpdateCartAsync(1, updatedCart))
                .ReturnsAsync(true);

            var result = await _controller.UpdateCart(1, updatedCart);

            Assert.IsType<NoContentResult>(result);

            _mockCartService.Verify(service => service.UpdateCartAsync(1, updatedCart), Times.Once);
        }

        [Fact]
        public async Task DeleteCart_ReturnsNotFound_WhenCartDoesNotExist()
        {
            _mockCartService.Setup(service => service.DeleteCartAsync(999))
                .ReturnsAsync(false);

            var result = await _controller.DeleteCart(999);

            Assert.IsType<NotFoundResult>(result);

            _mockCartService.Verify(service => service.DeleteCartAsync(999), Times.Once);
        }

        [Fact]
        public async Task DeleteCart_DeletesCartSuccessfully()
        {
            _mockCartService.Setup(service => service.DeleteCartAsync(1))
                .ReturnsAsync(true);

            var result = await _controller.DeleteCart(1);

            Assert.IsType<NoContentResult>(result);

            _mockCartService.Verify(service => service.DeleteCartAsync(1), Times.Once);
        }

        [Fact]
        public async Task RemoveItemFromCart_ReturnsNotFound_WhenCartDoesNotExist()
        {
            _mockCartService.Setup(service => service.RemoveItemFromCartAsync(999, 101))
                .ReturnsAsync(false);

            var result = await _controller.RemoveItemFromCart(999, 101);

            Assert.IsType<NotFoundResult>(result);

            _mockCartService.Verify(service => service.RemoveItemFromCartAsync(999, 101), Times.Once);
        }

        [Fact]
        public async Task RemoveItemFromCart_RemovesProductSuccessfully()
        {
            _mockCartService.Setup(service => service.RemoveItemFromCartAsync(1, 101))
                .ReturnsAsync(true);

            var result = await _controller.RemoveItemFromCart(1, 101);

            Assert.IsType<NoContentResult>(result);

            _mockCartService.Verify(service => service.RemoveItemFromCartAsync(1, 101), Times.Once);
        }

        [Fact]
        public async Task CheckoutCart_ReturnsOrder_WhenCartExists()
        {
            var checkoutDto = new CheckoutDto { IsRegisteredUser = true };
            var testCart = _testCarts.First();

            _mockCartService.Setup(service => service.CheckoutCartAsync(1))
                .ReturnsAsync(testCart);

            _mockCartService.Setup(service => service.CalculateTotalAsync(1, false))
                .ReturnsAsync(100.00m); // Subtotal without discount

            _mockCartService.Setup(service => service.CalculateTotalAsync(1, true))
                .ReturnsAsync(90.00m); // Total with discount

            var result = await _controller.CheckoutCart(1, checkoutDto);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var order = Assert.IsType<OrderDto>(okResult.Value);
            Assert.Equal(1, order.CartId);
            Assert.Equal(1, order.UserId);
            Assert.Equal(100.00m, order.Subtotal);
            Assert.Equal(10.00m, order.Discount);
            Assert.Equal(90.00m, order.Total);

            // Verify service was called
            _mockCartService.Verify(service => service.CheckoutCartAsync(1), Times.Once);
            _mockCartService.Verify(service => service.CalculateTotalAsync(1, false), Times.Once);
            _mockCartService.Verify(service => service.CalculateTotalAsync(1, true), Times.Once);
        }
    }
}