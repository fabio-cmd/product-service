using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using OrderService.Dtos;
using OrderService.Services.Interfaces;

namespace OrderService.Tests.Integration;

[CollectionDefinition("OracleCollection")]
public class OracleCollection : ICollectionFixture<OrderOracleFixture> { }

[Collection("OracleCollection")]
public class OrderEfCoreTests
{
    private readonly IOrderService _sut;

    public OrderEfCoreTests(OrderOracleFixture fx)
    {
        _sut = fx.Services.GetRequiredService<IOrderService>();
    }

    [Fact]
    public async Task Create_And_Fetch_Order_Persisted_In_Oracle()
    {
        var dto = new CreateOrderDto { ProductId = 9, Quantity = 4 };

        var created = await _sut.CreateAsync(dto);
        var fetched = await _sut.GetByIdAsync(created.Id);

        fetched.Should().NotBeNull()
               .And.BeEquivalentTo(created);
    }
}
