using FluentAssertions;
using Moq;
using OrderService.Dtos;
using OrderService.Models;
using OrderService.Services.Interfaces;
using OrderService.Services.Implementations;

namespace OrderService.Tests.Services
{
    public class CreateOrderTests
    {
        private readonly IOrderService _sut;

        public CreateOrderTests()
        {

            var gateway = new Mock<IProductGateway>();
            gateway.Setup(g => g.ExistsAsync(It.IsAny<int>())).ReturnsAsync(true);

            _sut = new OrderServiceInMemory(gateway.Object);
        }
        [Fact]
        public async Task Should_Create_Order_With_Created_Status()
        {
            var dto = new CreateOrderDto { ProductId = 1, Quantity = 2 };

            var order = await _sut.CreateAsync(dto);

            order.Status.Should().Be(OrderStatus.Created);
            order.Id.Should().BeGreaterThan(0);
        }
    }
}
