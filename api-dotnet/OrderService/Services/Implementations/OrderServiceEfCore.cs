using Microsoft.EntityFrameworkCore;
using OrderService.Dtos;
using OrderService.Models;
using OrderService.Services.Interfaces;
using OrderService.Data;

namespace OrderService.Services.Implementations;

public class OrderServiceEfCore : IOrderService
{
    private readonly OrderDbContext _db;
    private readonly IProductGateway _gateway;

    public OrderServiceEfCore(OrderDbContext db, IProductGateway gateway)
    {
        _db = db;
        _gateway = gateway;
    }

    public async Task<Order> CreateAsync(CreateOrderDto dto)
    {
        if (dto.Quantity <= 0)
            throw new InvalidOperationException("Quantity must be greater than zero.");

        if (!await _gateway.ExistsAsync(dto.ProductId))
            throw new KeyNotFoundException("Product not found.");

        var order = new Order
        {
            ProductId = dto.ProductId,
            Quantity  = dto.Quantity,
            Status    = OrderStatus.Created
        };

        _db.Orders.Add(order);
        await _db.SaveChangesAsync();
        return order;
    }

    public async Task<IEnumerable<Order>> GetAllAsync() =>
        await _db.Orders.AsNoTracking().ToListAsync();

    public Task<Order?> GetByIdAsync(int id) =>
        _db.Orders.AsNoTracking().FirstOrDefaultAsync(o => o.Id == id);

    public async Task<bool> DeleteAsync(int id)
    {
        var order = await _db.Orders.FindAsync(id);
        if (order is null) return false;
        _db.Remove(order);
        await _db.SaveChangesAsync();
        return true;
    }
}
