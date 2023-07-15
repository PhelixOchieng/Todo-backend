using Application.Core.Models;
using Users.Models;

namespace Todo.Models;

public class TodoItem : Entity {
  public string Title { get; set; } = null!;
  public string? Description { get; set; }

  public DateTime? CompletedAt { get; set; }

	public string UserId { get; set; } = null!;
	public User CreatedBy { get; set; } = null!;
  
	public bool IsCompleted {
    get { return CompletedAt is not null; }
  }
}
