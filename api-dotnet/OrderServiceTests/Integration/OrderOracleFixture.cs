using DotNet.Testcontainers.Builders;          // ContainerBuilder, Wait // Wait.ForUnixContainer
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OrderService.Data;
using OrderService.Services.Implementations;
using OrderService.Services.Interfaces;
using Testcontainers.Oracle;
using Xunit;

namespace OrderService.Tests.Integration;

public class OrderOracleFixture : IAsyncLifetime
{
  public OracleContainer Oracle { get; } =
    new OracleBuilder()
        .WithImage("gvenzl/oracle-free:23-slim")
        .WithPassword("oracle")
        .WithWaitStrategy(
            Wait.ForUnixContainer()
                .UntilMessageIsLogged("DATABASE IS READY TO USE!"))
        .Build();

    public IServiceProvider Services { get; private set; } = default!;

    public async Task InitializeAsync()
    {
        await Oracle.StartAsync();   // só retorna depois da mensagem acima

        var conn =
            $"User Id=system;Password=oracle;Data Source=localhost:{Oracle.GetMappedPublicPort(1521)}/FREEPDB1";

        var sc = new ServiceCollection();
        sc.AddDbContext<OrderDbContext>(o => o.UseOracle(conn));
        sc.AddScoped<IProductGateway, ProductGatewayFake>();
        sc.AddScoped<IOrderService, OrderServiceEfCore>();

        Services = sc.BuildServiceProvider();

        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
        await db.Database.EnsureCreatedAsync();
    }

    public Task DisposeAsync() => Oracle.DisposeAsync().AsTask();
}
