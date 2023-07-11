using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Application.Core.Context;
using Todo.Models;

namespace Todo.Controllers;

[ApiController]
[Authorize]
[Route("todos")]
public class TodoController : ControllerBase {
  private readonly ApplicationDbContext _context;

  public TodoController(ApplicationDbContext context) { _context = context; }

  [HttpGet]
  public async Task<ActionResult<IEnumerable<TodoItem>>> GetTodoItems() {
    if (_context.TodoItems == null) {
      return NotFound();
    }
    return await _context.TodoItems.OrderByDescending(i => i.CreatedAt)
        .ToListAsync();
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<TodoItem>> GetTodoItem(long id) {
    if (_context.TodoItems == null) {
      return NotFound();
    }
    var todoItem = await _context.TodoItems.FindAsync(id);

    if (todoItem == null) {
      return NotFound();
    }

    return todoItem;
  }

  [HttpPatch("{id}")]
  public async Task<ActionResult<TodoItem>>
  PatchTodoItem(long id, TodoItemPatchDTO todoDTO) {
    if (await _context.TodoItems.FindAsync(id) is not TodoItem todoItem)
      return NotFound();

    if (todoDTO.IsCompleted is true && !todoItem.IsCompleted)
      todoItem.CompletedAt = DateTime.UtcNow;
    else if (todoDTO.IsCompleted is false)
      todoItem.CompletedAt = null;

    todoItem.Title = todoDTO.Title ?? todoItem.Title;
    todoItem.Description = todoDTO.Description ?? todoItem.Description;
    todoItem.UpdatedAt = DateTime.UtcNow;
		
    await _context.SaveChangesAsync();
    return todoItem;
  }

  // To protect from overposting attacks, see
  // https://go.microsoft.com/fwlink/?linkid=2123754
  [HttpPost]
  public async Task<ActionResult<TodoItem>> PostTodoItem(TodoItem todoItem) {
    if (_context.TodoItems == null) {
      return Problem("Entity set 'TodoContext.TodoItems'  is null.");
    }

    todoItem.CreatedAt = DateTime.UtcNow;
    _context.TodoItems.Add(todoItem);
    await _context.SaveChangesAsync();

    return CreatedAtAction("GetTodoItem", new { id = todoItem.Id }, todoItem);
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
