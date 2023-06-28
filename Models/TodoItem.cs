namespace Todo.Models;

public class TodoItem
{
    public long Id { get; set; }
    public string title { get; set; } = null!;
    public string? description { get; set; }
}
