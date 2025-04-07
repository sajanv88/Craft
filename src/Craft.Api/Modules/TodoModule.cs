using Craft.Api.Domain;
using Craft.MultitenancyModule.Filters;
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
        var app = builder.MapGroup("/api/todos").AddEndpointFilter<TenantEndpointFilter>();
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

    public string GetModuleName() => nameof(TodoModule);
}
