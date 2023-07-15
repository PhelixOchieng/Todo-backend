using Users.Models;

namespace Todo.Models;

public sealed class TodoItemShallowAccessDTO {
  public long Id { get; set; }
  public string Title { get; set; } = null!;
  public string? Description { get; set; }
  public DateTime? CompletedAt { get; set; }
  public DateTime? CreatedAt { get; set; }
  public DateTime? UpdatedAt { get; set; }

  public static TodoItemShallowAccessDTO
  FromEntity(TodoItem todo) => new TodoItemShallowAccessDTO {
    Id = todo.Id,
    Title = todo.Title,
    Description = todo.Description,
    CompletedAt = todo.CompletedAt,
    CreatedAt = todo.CreatedAt,
    UpdatedAt = todo.UpdatedAt,
  };
}

public sealed class TodoItemDetailsAccessDTO {
  public long Id { get; set; }
  public string Title { get; set; } = null!;
  public string? Description { get; set; }
  public DateTime? CompletedAt { get; set; }
  public DateTime? CreatedAt { get; set; }
  public DateTime? UpdatedAt { get; set; }

  public UserShallowAccessDTO CreatedBy { get; set; } = null!;

  public static TodoItemDetailsAccessDTO
  FromEntity(TodoItem todo) => new TodoItemDetailsAccessDTO {
    Id = todo.Id,
    Title = todo.Title,
    Description = todo.Description,
    CompletedAt = todo.CompletedAt,
    CreatedAt = todo.CreatedAt,
    UpdatedAt = todo.UpdatedAt,
		CreatedBy = UserShallowAccessDTO.FromEntity(todo.CreatedBy)
  };
}
