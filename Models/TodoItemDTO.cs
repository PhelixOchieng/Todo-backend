namespace Todo.Models;

public class TodoItemPatchDTO
{
    public string? Title { get; set; }
    public bool? IsCompleted { get; set; }
    public string? Description { get; set; }
}
