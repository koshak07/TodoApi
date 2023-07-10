using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

namespace TodoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoItemsController : ControllerBase
    {
        private readonly TodoContext _context;
        private IWebHostEnvironment _appEnvironment;
        private readonly long _fileSizeLimit;

        public TodoItemsController(TodoContext context, IWebHostEnvironment appEnvironment, IConfiguration config)
        {
            _context = context;
            _appEnvironment = appEnvironment;
            _fileSizeLimit = config.GetValue<long>("FileSizeLimit");
        }

        // GET: api/TodoItems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItem>>> GetTodoItems()
        {
            if (_context.TodoItems == null)
            {
                return NotFound();
            }
            return await _context.TodoItems.ToListAsync();
        }

        // GET: api/TodoItems/5
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

        // PUT: api/TodoItems/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTodoItem(long id, TodoItem todoItem)
        {
            if (id != todoItem.Id)
            {
                return BadRequest();
            }

            _context.Entry(todoItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TodoItemExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/TodoItems
        [HttpPost]
        [RequestSizeLimit(10000)]
        
       

            public async Task<ActionResult<TodoItem>> PostTodoItem([FromForm] TodoItem todoItem, List<IFormFile> uploadedFile)
        {
            
            if (uploadedFile != null)
            {
                foreach (var item in uploadedFile)
                {
                    string path = "Files/" + item.FileName;
                    // сохраняем файл в папку Files в каталоге
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await item.CopyToAsync(fileStream);
                    }
                    FileModel file = new FileModel { Name = item.FileName, Path = path };

                    todoItem.FileModels.Add(file);
                }
                foreach (var item in todoItem.FileModels)
                {
                    // путь к папке Files
               

            _context.TodoItems.Add(todoItem);
                }
                

            }

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTodoItem), new { id = todoItem.Id }, todoItem);
        }

        // DELETE: api/TodoItems/5
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