using OrderService.Dtos;
using OrderService.Models;
using OrderService.Services.Interfaces;

namespace OrderService.Services.Implementations;

public class OrderServiceInMemory : IOrderService
{
    private int _seq = 1;
    private readonly List<Order> _db = new();
    private readonly IProductGateway _productGateway;

    public OrderServiceInMemory(IProductGateway productGateway)
    {
        _productGateway = productGateway;
    }

    public async Task<Order> CreateAsync(CreateOrderDto dto)
    {
        if (dto.Quantity <= 0)
            throw new InvalidOperationException("Quantity must be greater than zero.");

        if (!await _productGateway.ExistsAsync(dto.ProductId))
            throw new KeyNotFoundException("Product not found.");

        var order = new Order
        {
            Id = _seq++,
            ProductId = dto.ProductId,
            Quantity = dto.Quantity,
            Status = OrderStatus.Created
        };

        _db.Add(order);
        return order;
    }

    public Task<IEnumerable<Order>> GetAllAsync() =>
        Task.FromResult(_db.AsEnumerable());

    public Task<Order?> GetByIdAsync(int id) =>
        Task.FromResult(_db.FirstOrDefault(o => o.Id == id));

    public Task<bool> DeleteAsync(int id)
    {
        var order = _db.FirstOrDefault(o => o.Id == id);
        if (order is null) return Task.FromResult(false);
        _db.Remove(order);
        return Task.FromResult(true);
    }
}