using diploma.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace diploma.Controllers
{
        [Route("api/[controller]")]
        [ApiController]
        public class BookGenreController : ControllerBase
        {
            private readonly BookingContext _context;
            public BookGenreController(BookingContext context)
            {
                _context = context;

            }

            [HttpGet]
            public IEnumerable<BookGenre> GetAll()
            {//получение всех строк заказа
                try { return _context.BookGenre; }
                catch (Exception ex)
                {
                    Log.Write(ex);
                    return null;
                }

            }

            [HttpGet("{id}")]
            public async Task<IActionResult> GetBookGenre([FromRoute] int id)
            {//получение конкретной строки заказа по id
                try
                {
                    if (!ModelState.IsValid)
                    {
                        Log.WriteSuccess(" BookGenreController.GetBookGenre", "Валидация внутри контроллера неудачна.");
                        return BadRequest(ModelState);
                    }

                    var item = await _context.BookGenre.SingleOrDefaultAsync(m => m.Id == id);

                    if (item == null)
                    {
                        Log.WriteSuccess("BookGenreController.GetBookGenre", "Элемент BookGenre не найден.");
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
            public async Task<IActionResult> Create([FromBody] BookGenre item)
            {//создание новой строки заказа
                try
                {
                    if (!ModelState.IsValid)
                    {
                        Log.WriteSuccess("BookGenreController.Create", "Валидация внутри контроллера неудачна.");
                        return BadRequest(ModelState);
                    }

                    _context.BookGenre.Add(item);
                    await _context.SaveChangesAsync();
                    Log.WriteSuccess("BookGenreController.Create", "Добавлена новая строка заказа.");
                    return CreatedAtAction("GetBookGenre", new { id = item.Id }, item);
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
                        Log.WriteSuccess("BookGenreController.Delete", "Валидация внутри контроллера неудачна.");
                        return BadRequest(ModelState);
                    }
                    var item = _context.BookGenre.Find(id);
                    if (item == null)
                    {
                        Log.WriteSuccess("BookGenreController.Delete", "Элемент не найден.");
                        return NotFound();
                    }
                    _context.BookGenre.Remove(item);
                    await _context.SaveChangesAsync();
                    Log.WriteSuccess("BookGenreController.Delete", "Элемент удален.");
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
