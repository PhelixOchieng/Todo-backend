using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Application.Core.Context;
using Application.Core.Request;
using Application.Core.ApiClient;
using Todo.Models;
using Todo.Microservices;

namespace Todo.Controllers;

[ApiController]
[Authorize]
[Route("todos")]
public class TodoController : ControllerBase {
  private readonly ApplicationDbContext _context;

  public TodoController(ApplicationDbContext context) { _context = context; }

  [HttpGet]
  public async Task<ActionResult<IEnumerable<TodoItemShallowAccessDTO>>>
  GetTodoItems([FromQuery] SSFQueryParams ssfParams) {
    if (_context.TodoItems == null) {
      return NotFound();
    }

    string? userId = HttpContext?.User.FindFirst("userID")?.Value;
    if (userId == null) {
      return Problem(null, null, StatusCodes.Status400BadRequest,
                     "\"userID\" is missing from token");
    }

    int? lastItemId = ssfParams.LastItemId;
    var query = _context.TodoItems.Where(
        i => i.UserId == userId &&
             (lastItemId != null ? i.Id < lastItemId : true));

    if (!string.IsNullOrEmpty(ssfParams.Search)) {
      string searchQuery = ssfParams.Search.ToLower();
      ApiClient api = new ApiClient("http://localhost:8000", "api");
      var response = await api.GetAsJsonAsync<
          TodoMsApiResponse<List<MsDbTodo>>>(
          $"/search?q={searchQuery}&user_id={userId}&limit={ssfParams.PageSize}&page={1}");
      if (response == null) {
        // Use exact match search instead
        query = query.OrderByDescending(i => i.Id).Where(
            i => i.Title.ToLower().Contains(searchQuery) ||
                 (i.Description != null
                      ? i.Description.ToLower().Contains(searchQuery)
                      : true));
      } else {
        var msTodos = response.Data;
        var msTodoIds = msTodos.Select(todo => todo.Id);
        var todos = await query.Where(i => msTodoIds.Contains(i.Id))
                        .Select(e => TodoItemShallowAccessDTO.FromEntity(e))
                        .Take(ssfParams.PageSize)
                        .ToListAsync();

        // TODO: Optimize this workflow
        var responseTodos = new List<TodoItemShallowAccessDTO>();
        foreach (MsDbTodo msTodo in msTodos) {
          Console.WriteLine($"Todo: {msTodos}");
          var todo = todos.Find(todo => todo.Id == msTodo.Id)!;
          responseTodos.Add(todo);
        }

        return responseTodos;
      }
    }

    return await query.OrderByDescending(todo => todo.Id)
        .Select(e => TodoItemShallowAccessDTO.FromEntity(e))
        .Take(ssfParams.PageSize)
        .ToListAsync();
  }

  // To protect from overposting attacks, see
  // https://go.microsoft.com/fwlink/?linkid=2123754
  [HttpPost]
  public async Task<ActionResult<TodoItemDetailsAccessDTO>>
  PostTodoItem(TodoItemCreateDTO todoDTO) {
    if (_context.TodoItems == null) {
      return Problem("Entity set 'TodoContext.TodoItems'  is null.");
    }

    string? userId = HttpContext?.User.FindFirst("userID")?.Value;
    if (userId == null) {
      return Problem(null, null, StatusCodes.Status400BadRequest,
                     "\"userID\" is missing from token");
    }

    var user = await _context.Users.FindAsync(userId);
    if (user == null) {
      return NotFound("The user was not found with the provided id");
    }

    TodoItem todo =
        new TodoItem() { Title = todoDTO.Title,
                         Description = todoDTO.Description, CreatedBy = user,
                         CreatedAt = DateTime.UtcNow };
    _context.TodoItems.Add(todo);
    await _context.SaveChangesAsync();

    return CreatedAtAction(nameof(PostTodoItem), new { id = todo.Id },
                           TodoItemDetailsAccessDTO.FromEntity(todo));
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<TodoItemDetailsAccessDTO>>
  GetTodoItem(long id) {
    if (_context.TodoItems == null) {
      return NotFound();
    }

    var todo = await _context.TodoItems.FindAsync(id);
    if (todo == null) {
      return NotFound();
    }

    var createdBy = await _context.Users.FindAsync(todo.UserId);
    if (createdBy == null) {
      throw new InvalidDataException(
          $"The user that created this todo, '{todo.Id}' doesnt exist");
    }

    Console.WriteLine($"CreatedBy: {todo.CreatedBy}");
    return TodoItemDetailsAccessDTO.FromEntity(todo);
  }

  [HttpPatch("{id}")]
  public async Task<ActionResult<TodoItemDetailsAccessDTO>>
  PatchTodoItem(long id, TodoItemPatchDTO todoDTO) {
    if (await _context.TodoItems.FindAsync(id) is not TodoItem todo)
      return NotFound();

    if (todoDTO.IsCompleted is true && !todo.IsCompleted)
      todo.CompletedAt = DateTime.UtcNow;
    else if (todoDTO.IsCompleted is false)
      todo.CompletedAt = null;

    todo.Title = todoDTO.Title ?? todo.Title;
    todo.Description = todoDTO.Description ?? todo.Description;
    todo.UpdatedAt = DateTime.UtcNow;

    var createdBy = await _context.Users.FindAsync(todo.UserId);
    if (createdBy == null) {
      throw new InvalidDataException(
          $"The user that created this todo, '{todo.Id}' doesnt exist");
    }
    todo.CreatedBy = createdBy;

    await _context.SaveChangesAsync();
    return TodoItemDetailsAccessDTO.FromEntity(todo);
  }

  // DELETE: api/Todo/5
  [HttpDelete("{id}")]
  public async Task<IActionResult> DeleteTodoItem(long id) {
    if (_context.TodoItems == null) {
      return NotFound();
    }
    var todoItem = await _context.TodoItems.FindAsync(id);
    if (todoItem == null) {
      return NotFound();
    }

    _context.TodoItems.Remove(todoItem);
    await _context.SaveChangesAsync();

    return NoContent();
  }

  private bool TodoItemExists(long id) {
    return (_context.TodoItems?.Any(e => e.Id == id)).GetValueOrDefault();
  }
}
