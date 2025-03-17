using Craft.LocalizationModule.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;

namespace Craft.LocalizationModuleTests;

public class TestDatabaseFixture : IAsyncLifetime
{
    public PostgreSqlContainer PostgreSqlContainer { get; private set; }
    public string ConnectionString => PostgreSqlContainer.GetConnectionString();
    public LocalizationDbContext DbContext { get; private set; }


    public async Task InitializeAsync()
    {
        PostgreSqlContainer = new PostgreSqlBuilder()
            .WithImage("postgres:latest")
            .WithDatabase("testdb")
            .WithUsername("testuser")
            .WithPassword("testpassword")
            .WithCleanUp(true)
            .Build();

        await PostgreSqlContainer.StartAsync();
        
        var options = new DbContextOptionsBuilder<LocalizationDbContext>()
            .UseNpgsql(PostgreSqlContainer.GetConnectionString())
            .Options;

        DbContext = new LocalizationDbContext(options);
        await DbContext.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        await PostgreSqlContainer.StopAsync();
    }
}
