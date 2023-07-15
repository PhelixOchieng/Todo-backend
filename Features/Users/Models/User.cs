using Microsoft.AspNetCore.Identity;
using Todo.Models;

namespace Users.Models;

public sealed class User : IdentityUser {
  public string FirstName { get; set; } = null!;
  public string LastName { get; set; } = null!;
	public ICollection<TodoItem> TodoItems { get; }= new List<TodoItem>();

  public DateTime CreatedAt { get; set; }
  public DateTime? UpdatedAt { get; set; }
}
