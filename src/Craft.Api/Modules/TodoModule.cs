using Craft.Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace Craft.Api.Modules;

public sealed class TodoModule : CraftModule.CraftModule
{
    public override void PreInitialization(IServiceCollection services)
    {
        services.AddScoped<TodoService>();
    }

    public override IEndpointRouteBuilder AddRoutes(
        IEndpointRouteBuilder builder
    )
    {
        var app = builder.MapGroup("/todos");
        var scope = builder.ServiceProvider.CreateScope();
        var todoService =
            scope.ServiceProvider.GetRequiredService<TodoService>();

        app.MapGet("/", todoService.GetTodosAsync);
        app.MapGet("/{id}", todoService.GetTodoAsync);

        app.MapPut("/", todoService.CreateTodoAsync);
        app.MapDelete("/{id}", todoService.DeleteTodoAsync);

        app.MapPatch("/{id}", todoService.UpdateTodoAsync);

        return builder;
    }

    public override void ConfigureModelBuilder(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TodoEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Content).IsRequired();
            entity.Property(e => e.IsCompleted).HasDefaultValue(false);
        });

        Console.WriteLine(
            $"{nameof(TodoEntity)} configured in {nameof(TodoModule)}"
        );
    }

    public string GetModuleName() => nameof(TodoModule);
}
