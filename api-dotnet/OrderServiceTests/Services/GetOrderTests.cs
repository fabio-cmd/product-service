using FluentAssertions;
using Moq;
using OrderService.Dtos;
using OrderService.Services.Interfaces;
using OrderService.Services.Implementations;

namespace OrderService.Tests.Services;

public class GetOrderTests
{
    private readonly IOrderService _sut;

    public GetOrderTests()
    {
        var gateway = new Mock<IProductGateway>();
        gateway.Setup(g => g.ExistsAsync(It.IsAny<int>())).ReturnsAsync(true);

        _sut = new OrderServiceInMemory(gateway.Object);
    }

    [Fact]
    public async Task GetById_Should_Return_Order_When_Exists()
    {
        var created = await _sut.CreateAsync(new CreateOrderDto { ProductId = 2, Quantity = 5 });

        var fetched = await _sut.GetByIdAsync(created.Id);

        fetched.Should().NotBeNull()
               .And.BeEquivalentTo(created);
    }
}
