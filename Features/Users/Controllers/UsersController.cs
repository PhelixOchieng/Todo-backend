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

  // [HttpPatch("{id}")]
  // public async Task<ActionResult<UsersItem>>
  // PatchUsersItem(long id, TodoItemPatchDTO todoDTO) {
  //   if (await _context.UsersItems.FindAsync(id) is not TodoItem todoItem)
  //     return NotFound();
  //
  //   if (todoDTO.IsCompleted is true && !todoItem.IsCompleted)
  //     todoItem.CompletedAt = DateTime.UtcNow;
  //   else if (todoDTO.IsCompleted is false)
  //     todoItem.CompletedAt = null;
  //
  //   todoItem.Title = todoDTO.Title ?? todoItem.Title;
  //   todoItem.Description = todoDTO.Description ?? todoItem.Description;
  //   todoItem.UpdatedAt = DateTime.UtcNow;
  //
  //   await _context.SaveChangesAsync();
  //   return todoItem;
  // }
	//
}
