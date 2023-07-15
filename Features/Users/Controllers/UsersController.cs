using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Application.Core.Context;
using Users.Models;

namespace Users.Controllers;

[ApiController]
[Authorize]
[Route("profile")]
public class UsersController : ControllerBase {
  private readonly ApplicationDbContext _context;

  public UsersController(ApplicationDbContext context) { _context = context; }

  [HttpGet]
  public async Task<ActionResult<UserAccessDTO>> GetProfile() {
    if (_context.Users == null) {
      return NotFound();
    }

    string? userId = HttpContext?.User.FindFirst("userID")?.Value;
    if (userId == null) {
      return Problem(null, null, StatusCodes.Status400BadRequest,
                     "\"userID\" is missing from token");
    }

		var user = await _context.Users.FindAsync(userId);
		Console.WriteLine($"User: {user}");
		if (user == null) {
			return NotFound();
		}

		return UserAccessDTO.FromEntity(user);
  }

  [HttpPatch()]
  public async Task<ActionResult<UserAccessDTO>>
  PatchUsersItem(UserPatchDTO userDTO) {
    string? userId = HttpContext?.User.FindFirst("userID")?.Value;
    if (userId == null) {
      return Problem(null, null, StatusCodes.Status400BadRequest,
                     "\"userID\" is missing from token");
    }


    if (await _context.Users.FindAsync(userId) is not User user)
      return NotFound();

    user.Email = userDTO.Email ?? user.Email;
		user.FirstName = userDTO.FirstName ?? user.FirstName;
		user.LastName = userDTO.LastName ?? user.LastName;
    user.UpdatedAt = DateTime.UtcNow;

    await _context.SaveChangesAsync();
    return UserAccessDTO.FromEntity(user);
  }
}
