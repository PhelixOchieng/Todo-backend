namespace Todo.Models;

public class TodoItemUpdateDTO
{
    public long Id { get; set; }
    public string Title { get; set; } = null!;
    public bool IsCompleted { get; set; }
    public string? Description { get; set; }
}

public class TodoItemPatchDTO
{
    public string? Title { get; set; }
    public bool? IsCompleted { get; set; }
    public string? Description { get; set; }
}
