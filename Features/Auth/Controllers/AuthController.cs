using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

using Application.Core.Context;
using Auth.Service;
using Auth;
using Users.Models;

namespace Application.Auth.Controllers;

[Route("auth")]
[ApiController]
public class AuthController : ControllerBase {
  private readonly ApplicationDbContext _context;
  private readonly UserManager<User> _userManager;
  private readonly TokenService _tokenService;

  public AuthController(UserManager<User> userManager,
                        ApplicationDbContext context, TokenService service) {
    _context = context;
    _userManager = userManager;
    _tokenService = service;
  }

  [HttpPost("login")]
  public async Task<ActionResult<AuthResponse>>
  Login([FromBody] AuthRequest request) {
    if (!ModelState.IsValid)
      return BadRequest(ModelState);

    var managedUser = await _userManager.FindByEmailAsync(request.Email);
    if (managedUser is null) {
			var problem = new ProblemDetails();
			problem.Title = "Invalid email and/or password";
			return Unauthorized(problem);
    }

    bool isPasswordValid =
        await _userManager.CheckPasswordAsync(managedUser, request.Password);
    if (!isPasswordValid) {
			var problem = new ProblemDetails();
			problem.Title = "Invalid email and/or password";
			return Unauthorized(problem);
		}

    User? user = await _context.Users.SingleOrDefaultAsync(
        user => user.Email == request.Email);
    if (user is null)
      return Unauthorized();

    string accessToken = _tokenService.CreateToken(user);
    await _context.SaveChangesAsync();

    return Ok(new AuthResponse {
      Email = user.Email,
      Token = accessToken,
    });
  }

  [HttpPost("signup")]
  public async Task<ActionResult<UserAccessDTO>> Signup(UserCreateDTO userDTO) {
    if (!ModelState.IsValid)
      return BadRequest(ModelState);

    var user =
        new User { UserName = userDTO.UserName, Email = userDTO.Email,
                   FirstName = userDTO.FirstName, LastName = userDTO.LastName,
                   CreatedAt = DateTime.UtcNow };
    var result = await _userManager.CreateAsync(user, userDTO.Password);
    if (result.Succeeded) {
      return CreatedAtAction(nameof(Signup), new { email = user.Email },
                             UserAccessDTO.FromEntity(user));
    }

    Console.WriteLine($"\nSomething: {ModelState} {result.Errors}");
    foreach (var error in result.Errors) {
      ModelState.AddModelError(error.Code, error.Description);
      Console.WriteLine($"Error: {error.Code}");
    }
    return BadRequest(ModelState);
  }
}
