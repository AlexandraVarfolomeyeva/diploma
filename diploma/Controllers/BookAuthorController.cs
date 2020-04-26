using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using diploma.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace diploma.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookAuthorController : ControllerBase
    {
        private readonly BookingContext _context;
        public BookAuthorController(BookingContext context)
        {
            _context = context;

        }

        [HttpGet]
        public IEnumerable<BookAuthor> GetAll()
        {//получение всех строк заказа
            try { return _context.BookAuthor; }
            catch (Exception ex)
            {
                Log.Write(ex);
                return null;
            }

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookAuthor([FromRoute] int id)
        {//получение конкретной строки заказа по id
            try
            {
                if (!ModelState.IsValid)
                {
                    Log.WriteSuccess(" BookAuthorController.GetBookAuthor", "Валидация внутри контроллера неудачна.");
                    return BadRequest(ModelState);
                }

                var item = await _context.BookAuthor.SingleOrDefaultAsync(m => m.Id == id);

                if (item == null)
                {
                    Log.WriteSuccess("BookAuthorController.GetBookAuthor", "Элемент BookAuthor не найден.");
                    return NotFound();
                }
                return Ok(item);
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                return BadRequest(ModelState);
            }
        }


        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create([FromBody] BookAuthor item)
        {//создание новой строки заказа
         //  string id = IDEvent().Result;//получили id пользователя
            try
            {
                if (!ModelState.IsValid)
                {
                    Log.WriteSuccess("BookAuthorController.Create", "Валидация внутри контроллера неудачна.");
                    return BadRequest(ModelState);
                }

                _context.BookAuthor.Add(item);
                await _context.SaveChangesAsync();
                Log.WriteSuccess("BookAuthorController.Create", "Добавлена новая строка заказа.");
                return CreatedAtAction("GetBookAuthor", new { id = item.Id }, item);
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                return BadRequest(ModelState);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {//удаление существующей строки заказа
            try
            {
                if (!ModelState.IsValid)
                {
                    Log.WriteSuccess("BookAuthorController.Delete", "Валидация внутри контроллера неудачна.");
                    return BadRequest(ModelState);
                }
                var item = _context.BookAuthor.Find(id);
                if (item == null)
                {
                    Log.WriteSuccess("BookAuthorController.Delete", "Элемент не найден.");
                    return NotFound();
                }
                _context.BookAuthor.Remove(item);
                await _context.SaveChangesAsync();
                Log.WriteSuccess("BookAuthorController.Delete", "Элемент удален.");
                return NoContent();
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                return BadRequest(ModelState);
            }
        }
    }
}
