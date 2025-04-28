using FluentAssertions;
using Moq;
using OrderService.Dtos;
using OrderService.Services.Interfaces;
using OrderService.Services.Implementations;

namespace OrderService.Tests.Services;

public class QuantityRuleTests
{
    [Fact]
    public async Task Should_Throw_When_Quantity_Is_Invalid()
    {
        var gateway = new Mock<IProductGateway>();
        gateway.Setup(g => g.ExistsAsync(It.IsAny<int>())).ReturnsAsync(true);

        var sut = new OrderServiceInMemory(gateway.Object);
        var dto = new CreateOrderDto { ProductId = 1, Quantity = 0 };

        await sut.Invoking(s => s.CreateAsync(dto))
                 .Should().ThrowAsync<InvalidOperationException>();
    }
}
