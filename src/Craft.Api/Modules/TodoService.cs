using Craft.Api.Domain;
using Craft.Api.Domain.Dto;
using Craft.Api.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Craft.Api.Modules;

public sealed class TodoService
{
    private readonly ApiDbContext _dbContext;

    public TodoService(ApiDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IResult> GetTodosAsync()
    {
        return Results.Ok(await _dbContext.Todos.AsNoTracking().ToListAsync());
    }

    public async Task<IResult> GetTodoAsync(Guid id)
    {
        var todo = await _dbContext.Todos.FindAsync(id);
        return todo is not null ? Results.Ok(todo) : Results.NotFound();
    }

    public async Task<IResult> CreateTodoAsync(CreateTodo newTodo)
    {
        var todo = new TodoEntity { Content = newTodo.Content };
        await _dbContext.Todos.AddAsync(todo);
        await _dbContext.SaveChangesAsync();
        return Results.Created($"/todos/{todo.Id}", todo);
    }

    public async Task<IResult> DeleteTodoAsync(Guid id)
    {
        var todo = await _dbContext.Todos.FindAsync(id);
        if (todo is null)
        {
            return Results.NotFound();
        }

        _dbContext.Todos.Remove(todo);
        await _dbContext.SaveChangesAsync();
        return Results.Accepted();
    }

    public async Task<IResult> UpdateTodoAsync(Guid id, UpdateTodo existingTodo)
    {
        var todo = await _dbContext.Todos.FindAsync(id);
        if (todo is null)
        {
            return Results.NotFound();
        }
        todo.Content = existingTodo.Content;
        todo.IsCompleted = existingTodo.IsCompleted;
        _dbContext.Todos.Update(todo);
        await _dbContext.SaveChangesAsync();
        return Results.NoContent();
    }
}
