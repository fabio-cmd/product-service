using FluentAssertions;
using Moq;
using OrderService.Dtos;
using OrderService.Services.Interfaces;
using OrderService.Services.Implementations;

namespace OrderService.Tests.Services;

public class DeleteOrderTests
{
    private readonly IOrderService _sut;

    public DeleteOrderTests()
    {
        var gateway = new Mock<IProductGateway>();
        gateway.Setup(g => g.ExistsAsync(It.IsAny<int>())).ReturnsAsync(true);

        _sut = new OrderServiceInMemory(gateway.Object);
    }

    [Fact]
    public async Task Delete_Should_Remove_Order_And_Return_True()
    {
        var order = await _sut.CreateAsync(new CreateOrderDto { ProductId = 3, Quantity = 1 });

        var removed = await _sut.DeleteAsync(order.Id);
        var fetch = await _sut.GetByIdAsync(order.Id);

        removed.Should().BeTrue();
        fetch.Should().BeNull();
    }
}
