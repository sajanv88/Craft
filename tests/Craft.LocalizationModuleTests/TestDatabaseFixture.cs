using Craft.LocalizationModule.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;

namespace Craft.LocalizationModuleTests;

public class TestDatabaseFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgreSqlContainer;
    private DbContextOptions<LocalizationDbContext> _dbContextOptions;
    
    public string ConnectionString { get; private set; }
    public LocalizationDbContext DbContext { get; private set; }

    public TestDatabaseFixture()
    {
        _postgreSqlContainer = new PostgreSqlBuilder()
            .WithImage("postgres:latest")
            .WithDatabase("testdb")
            .WithUsername("testuser")
            .WithPassword("testpassword")
            .WithCleanUp(true)
            .Build();
    }

    public async Task InitializeAsync()
    {
        Console.WriteLine("Starting PostgreSQL container...");
        // 1. Start the container first
        await _postgreSqlContainer.StartAsync();
        
        // 2. Get the connection string AFTER container is started
        ConnectionString = _postgreSqlContainer.GetConnectionString();
        Console.WriteLine($"Full connection string: {ConnectionString}");
        Console.WriteLine($"Container status: {_postgreSqlContainer.State}");
        
        // 3. Configure DbContext options
        _dbContextOptions = new DbContextOptionsBuilder<LocalizationDbContext>()
            .UseNpgsql(ConnectionString, options => 
            {
                options.EnableRetryOnFailure();
                options.CommandTimeout(30);
            })
            .EnableSensitiveDataLogging()
            .EnableDetailedErrors()
            .Options;

        // 4. Create the DbContext instance
        DbContext = new LocalizationDbContext(_dbContextOptions);
        
        // 5. Ensure database is created
        await DbContext.Database.EnsureCreatedAsync();
        Console.WriteLine("Database schema initialized");
    }

    public async Task DisposeAsync()
    {
        // Clean up in reverse order
        if (DbContext != null)
        {
            await DbContext.Database.EnsureDeletedAsync();
            await DbContext.DisposeAsync();
        }
        
        await _postgreSqlContainer.DisposeAsync();
    }

    // Helper method to create fresh DbContext instances
    public LocalizationDbContext CreateFreshDbContext()
    {
        return new LocalizationDbContext(_dbContextOptions);
    }
}
