namespace Todo.Models;

public sealed class TodoItemCreateDTO {
  public string Title { get; set; } = null!;
  public string? Description { get; set; }
}

public sealed class TodoItemUpdateDTO
{
    public long Id { get; set; }
    public string Title { get; set; } = null!;
    public bool IsCompleted { get; set; }
    public string? Description { get; set; }
}

public sealed class TodoItemPatchDTO
{
    public string? Title { get; set; }
    public bool? IsCompleted { get; set; }
    public string? Description { get; set; }
}
