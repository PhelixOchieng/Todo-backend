namespace Todo.Models;

public class TodoItem
{
    public long Id { get; set; }
    public string Title { get; set; } = null!;
		public bool IsCompleted { get; set; } = false;
    public string? Description { get; set; }

		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
		public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
