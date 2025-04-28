using Microsoft.AspNetCore.Mvc;
using System.Text.Json;



namespace OrderService.Controllers;

[ApiController]
[Route("api/[controller]")]


public class OrderIntegrationController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;

    public OrderIntegrationController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [HttpPost("create-from-products")]
    public async Task<IActionResult> CreateOrderFromProducts()
    {
        var client = _httpClientFactory.CreateClient("ProductApi");

        var response = await client.GetAsync("produtos");
        if (!response.IsSuccessStatusCode)
        {
            return StatusCode((int)response.StatusCode, "Erro ao buscar produto ");
        }

        var responseBody = await response.Content.ReadAsStringAsync();

        var products = JsonSerializer.Deserialize<List<Product>>(responseBody, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (products == null || products.Count == 0)
        {
            return BadRequest("Nenhum produto retornado.");
        }


        var order = new OrderDTO
        {
            Id = Guid.NewGuid().GetHashCode(),
            ProductNames = products.Select(p => p.Nome).Where(nome => nome != null).Cast<string>().ToList()
        };

        return Ok(order);
    }

    public class Product
    {
        public string? Nome { get; set; }
        public string? Categoria { get; set; }
        public decimal Preco { get; set; }
    }


    public class OrderDTO
    {
        public int Id { get; set; }
        public List<string> ProductNames { get; set; } = new List<string>();
    }

}

