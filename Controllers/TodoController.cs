using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Todo.Context;
using Todo.Models;

namespace Todo.Controllers
{
    [Route("todos")]
    [ApiController]
    public class TodoController : ControllerBase
    {
        private readonly TodoContext _context;

        public TodoController(TodoContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItem>>> GetTodoItems()
        {
            if (_context.TodoItems == null)
            {
                return NotFound();
            }
            return await _context.TodoItems.OrderByDescending(i => i.CreatedAt).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItem>> GetTodoItem(long id)
        {
            if (_context.TodoItems == null)
            {
                return NotFound();
            }
            var todoItem = await _context.TodoItems.FindAsync(id);

            if (todoItem == null)
            {
                return NotFound();
            }

            return todoItem;
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult<TodoItem>> PPatchTodoItem(long id, TodoItemPatchDTO todoDTO)
        {
            if (await _context.TodoItems.FindAsync(id) is not TodoItem todoItem)
                return NotFound();

            todoItem.Title =  todoDTO.Title ?? todoItem.Title;
            todoItem.Description = todoDTO.Description ?? todoItem.Description;
            todoItem.IsCompleted = todoDTO.IsCompleted ?? todoItem.IsCompleted;
            todoItem.UpdatedAt = DateTime.UtcNow;

						await _context.SaveChangesAsync();
						return todoItem;
        }

        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<ActionResult<TodoItem>> PutTodoItem(long id, TodoItemUpdateDTO todoDTO)
        {
            Console.WriteLine($"Todo: {id} -> {todoDTO.Id}");
            if (id != todoDTO.Id)
                return BadRequest();

            if (await _context.TodoItems.FindAsync(id) is not TodoItem todoItem)
                return NotFound();

            todoItem.Title = todoDTO.Title;
            todoItem.Description = todoDTO.Description;
            todoItem.IsCompleted = todoDTO.IsCompleted;
            todoItem.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) when (!TodoItemExists(id))
            {
                return NotFound();
            }

            return todoItem;
        }

        // POST: api/Todo
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TodoItem>> PostTodoItem(TodoItem todoItem)
        {
            if (_context.TodoItems == null)
            {
                return Problem("Entity set 'TodoContext.TodoItems'  is null.");
            }

            todoItem.CreatedAt = DateTime.UtcNow;
            _context.TodoItems.Add(todoItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTodoItem", new { id = todoItem.Id }, todoItem);
        }

        // DELETE: api/Todo/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodoItem(long id)
        {
            if (_context.TodoItems == null)
            {
                return NotFound();
            }
            var todoItem = await _context.TodoItems.FindAsync(id);
            if (todoItem == null)
            {
                return NotFound();
            }

            _context.TodoItems.Remove(todoItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TodoItemExists(long id)
        {
            return (_context.TodoItems?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
