namespace OrderService.Models;

public enum OrderStatus { Created, Cancelled, Shipped }

public class Order
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public OrderStatus Status { get; set; }
}
