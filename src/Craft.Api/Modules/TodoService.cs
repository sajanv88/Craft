using Craft.Api.Domain;
using Craft.Api.Domain.Dto;
using Craft.Api.Infrastructure;
using Craft.MultitenancyModule.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Craft.Api.Modules;

public sealed class TodoService
{
    private readonly IDynamicDbContextAccessor _dbContext;

    public TodoService(IDynamicDbContextAccessor dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IResult> GetTodosAsync()
    {
        var dbContext = (ApiDbContext) _dbContext.GetDbContext();
        return Results.Ok(await dbContext.Todos.ToListAsync());
    }

    public async Task<IResult> GetTodoAsync(Guid id)
    {
        var dbContext = (ApiDbContext)  _dbContext.GetDbContext();

        var todo = await dbContext.Todos.FindAsync(id);
        return todo is not null ? Results.Ok(todo) : Results.NotFound();
    }

    public async Task<IResult> CreateTodoAsync(CreateTodo newTodo)
    {
        var dbContext = (ApiDbContext) _dbContext.GetDbContext();

        var todo = new TodoEntity { Content = newTodo.Content, IsCompleted = false};
        await dbContext.Todos.AddAsync(todo);
        await dbContext.SaveChangesAsync();
        return Results.Created($"/todos/{todo.Id}", todo);
    }

    public async Task<IResult> DeleteTodoAsync(Guid id)
    {
        var dbContext = (ApiDbContext)  _dbContext.GetDbContext();

        var todo = await dbContext.Todos.FindAsync(id);
        if (todo is null)
        {
            return Results.NotFound();
        }

        dbContext.Todos.Remove(todo);
        await dbContext.SaveChangesAsync();
        return Results.Accepted();
    }

    public async Task<IResult> UpdateTodoAsync(Guid id, UpdateTodo existingTodo)
    {
        var dbContext = (ApiDbContext) _dbContext.GetDbContext();

        var todo = await dbContext.Todos.FindAsync(id);
        if (todo is null)
        {
            return Results.NotFound();
        }
        todo.Content = existingTodo.Content;
        todo.IsCompleted = existingTodo.IsCompleted;
        dbContext.Todos.Update(todo);
        await dbContext.SaveChangesAsync();
        return Results.NoContent();
    }
}
