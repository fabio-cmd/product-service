using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private static List<Order> orders = new();

    [HttpGet]
    public IActionResult Get() => Ok(orders);

    [HttpPost]
    public IActionResult Post([FromBody] Order order)
    {
        order.Id = orders.Count + 1;
        orders.Add(order);
        return CreatedAtAction(nameof(Get), new { id = order.Id }, order);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var order = orders.FirstOrDefault(o => o.Id == id);
        if (order is null) return NotFound();
        orders.Remove(order);
        return NoContent();
    }
}
