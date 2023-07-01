namespace Todo.Models;

public class TodoItem : Entity {
  public string Title { get; set; } = null!;
  public string? Description { get; set; }

  public DateTime? CompletedAt { get; set; }
  
	public bool IsCompleted {
    get { return CompletedAt is not null; }
  }
}
