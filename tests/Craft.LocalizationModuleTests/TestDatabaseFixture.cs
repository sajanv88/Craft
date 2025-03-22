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
        Console.WriteLine("Starting PostgreSQL Testcontainer...");

        
        PostgreSqlContainer = new PostgreSqlBuilder()
            .WithImage("postgres:latest")
            .WithDatabase("testdb")
            .WithUsername("testuser")
            .WithPassword("testpassword")
            .WithCleanUp(true)
            .Build();

        await PostgreSqlContainer.StartAsync();
        
        Console.WriteLine("PostgreSQL Testcontainer started at: " + PostgreSqlContainer.GetConnectionString());

        var options = new DbContextOptionsBuilder<LocalizationDbContext>()
            .UseNpgsql(PostgreSqlContainer.GetConnectionString())
            .Options;

        DbContext = new LocalizationDbContext(options);
        await DbContext.Database.EnsureCreatedAsync();
        
        Console.WriteLine("Database schema initialized.");

    }

    public async Task DisposeAsync()
    {
        await PostgreSqlContainer.StopAsync();
    }
}
