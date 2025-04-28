using OrderService.Services.Implementations;
using OrderService.Services.Interfaces;
using OrderService.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IProductGateway, ProductGatewayFake>();
builder.Services.AddScoped<IOrderService, OrderServiceInMemory>();

var conn = builder.Configuration.GetConnectionString("Oracle") ??
           builder.Configuration["ORACLE_CONN"];

           builder.Services.AddDbContext<OrderDbContext>(opt =>
opt.UseOracle(conn));

builder.Services.AddScoped<IProductGateway, ProductGatewayFake>();

if (builder.Environment.IsDevelopment())
    builder.Services.AddScoped<IOrderService, OrderServiceInMemory>();
else
    builder.Services.AddScoped<IOrderService, OrderServiceEfCore>();

builder.Services.AddHttpClient("ProductApi", client =>
{
    client.BaseAddress = new Uri("http://java-api:8080/");
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
