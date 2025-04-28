namespace OrderService.Services.Interfaces;

public interface IProductGateway
{
    Task<bool> ExistsAsync(int productId);
}
