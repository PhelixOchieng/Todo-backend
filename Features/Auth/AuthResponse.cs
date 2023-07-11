namespace Auth;

public sealed class AuthResponse {
  public string Email { get; set; } = null!;
  public string Token { get; set; } = null!;
}
