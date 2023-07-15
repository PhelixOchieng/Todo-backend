namespace Users.Models;

public sealed class UserAccessDTO {
  public string Id { get; set; } = null!;
  public string? UserName { get; set; } = null!;
  public string? Email { get; set; } = null!;
  public string FirstName { get; set; } = null!;
  public string LastName { get; set; } = null!;
  public DateTime CreatedAt { get; set; }
  public DateTime? UpdatedAt { get; set; } = null!;

  public static UserAccessDTO FromEntity(User user) => new UserAccessDTO {
    Id = user.Id,
    UserName = user.UserName,
    Email = user.Email,
    FirstName = user.FirstName,
    LastName = user.LastName,
    CreatedAt = user.CreatedAt,
    UpdatedAt = user.UpdatedAt,
  };
}

public sealed class UserShallowAccessDTO {
  public string Id { get; set; } = null!;
  public string? UserName { get; set; } = null!;

  public static UserShallowAccessDTO
  FromEntity(User user) => new UserShallowAccessDTO {
    Id = user.Id,
    UserName = user.UserName,
  };
}
