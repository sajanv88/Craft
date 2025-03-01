namespace Craft.Api.Domain;

public sealed class TodoEntity
{
    public Guid Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public bool IsCompleted { get; set; } = false;
}
