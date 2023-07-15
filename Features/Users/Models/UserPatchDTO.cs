namespace Users.Models;

public sealed class UserPatchDTO {
  public string? UserName { get; set; } = null!;
  public string? Email { get; set; } = null!;
  public string FirstName { get; set; } = null!;
  public string LastName { get; set; } = null!;
}
