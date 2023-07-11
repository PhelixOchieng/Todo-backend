using Microsoft.AspNetCore.Identity;

namespace Users.Models;

public sealed class User : IdentityUser {
  public string FirstName { get; set; } = null!;
  public string LastName { get; set; } = null!;

  public DateTime CreatedAt { get; set; }
  public DateTime? UpdatedAt { get; set; }
}
