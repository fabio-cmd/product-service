using OrderService.Services.Interfaces;

namespace OrderService.Services.Implementations;

public class ProductGatewayFake : IProductGateway
{
    public Task<bool> ExistsAsync(int productId) => Task.FromResult(true);
}
