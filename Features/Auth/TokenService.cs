using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

using Users.Models;

namespace Auth.Service;

public sealed class TokenService {
  private const int _ExpirationDuration = 30; // Expiration duration in minutes

  public string CreateToken(User user) {
    var expirationDate = DateTime.UtcNow.AddMinutes(_ExpirationDuration);
    var token = CreateJwtToken(CreateClaims(user), CreateSigninCredentials(),
                               expirationDate);
    var tokenHandler = new JwtSecurityTokenHandler();
    return tokenHandler.WriteToken(token);
  }

  private JwtSecurityToken CreateJwtToken(List<Claim> claims,
                                          SigningCredentials credentials,
                                          DateTime expirationDate) =>
      new("todoApp", "todoApp", claims, expires: expirationDate,
          signingCredentials: credentials);

  private List<Claim> CreateClaims(User user) {
    try {
      var claims = new List<Claim> {
        new Claim(JwtRegisteredClaimNames.Sub, "TokenForTodoApp"),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim(JwtRegisteredClaimNames.Iat,
                  DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)),
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Email, user.Email),
      };

      return claims;
    } catch (Exception e) {
      Console.WriteLine(e);
      throw;
    }
  }

  private SigningCredentials CreateSigninCredentials() {
    return new SigningCredentials(
        new SymmetricSecurityKey(Encoding.UTF8.GetBytes("!SomethingSecret!")),
        SecurityAlgorithms.HmacSha256);
  }
}
