using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using diploma.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace diploma.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class BookOrderController : ControllerBase
    {
        private readonly BookingContext _context;
        public BookOrderController(BookingContext context)
        {
            _context = context;
            
        }

        [HttpGet]
        public IEnumerable<BookOrder> GetAll()
        {//получение всех строк заказа
            try {  return _context.BookOrder;} catch (Exception ex)
            {
                Log.Write(ex);
                return null;
            }
           
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookOrder([FromRoute] int id)
        {//получение конкретной строки заказа по id
            try
            {
                if (!ModelState.IsValid)
                {
                    Log.WriteSuccess(" BookOrderController.GetBookOrder", "Валидация внутри контроллера неудачна.");
                    return BadRequest(ModelState);
                }

                var item = await _context.BookOrder.SingleOrDefaultAsync(m => m.Id == id);

                if (item == null)
                {
                    Log.WriteSuccess("BookOrderController.GetBookOrder", "Элемент BookOrder не найден.");
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
        [Authorize(Roles = "user")]
        public async Task<IActionResult> Create([FromBody] BookOrderForm item)
        {//создание новой строки заказа
            try
            {
                if (!ModelState.IsValid)
                {
                    Log.WriteSuccess("BookOrderController.Create", "Валидация внутри контроллера неудачна.");
                    return BadRequest(ModelState);
                }
            
                IEnumerable<BookOrder> books = _context.BookOrder.Where(a => a.IdBook == item.IdBook);
                foreach (BookOrder book  in books)
                {
                    book.Amount++;
                    _context.BookOrder.Update(book);
                }
                BookOrder bookorder = new BookOrder()
                    {
                        IdBook = item.IdBook,
                        IdOrder = item.IdOrder,
                        Amount = 1
                    };
                if (!books.Any()) {
               
                    _context.BookOrder.Add(bookorder);
                }
                Order order = _context.Order.Find(item.IdOrder);
                order.SumOrder += item.Sum;
                order.Amount++;
                _context.Order.Update(order);
                await _context.SaveChangesAsync();
                Log.WriteSuccess("BookOrderController.Create", "Добавлена новая строка заказа.");
                return  CreatedAtAction("GetBookOrder", new { id = bookorder.Id }, bookorder);
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                return BadRequest(ModelState);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {//удаление существующей строки заказа
            try
            {
                if (!ModelState.IsValid)
                {
                    Log.WriteSuccess("BookOrderController.Delete", "Валидация внутри контроллера неудачна.");
                    return BadRequest(ModelState);
                }
                var item = _context.BookOrder.Find(id);
                if (item == null)
                {
                    Log.WriteSuccess("BookOrderController.Delete", "Элемент не найден.");
                    return NotFound();
                }
                _context.BookOrder.Remove(item);
                await _context.SaveChangesAsync();
                Log.WriteSuccess("BookOrderController.Delete", "Элемент удален.");
                return NoContent();
            } catch (Exception ex)
            {
                Log.Write(ex);
                return BadRequest(ModelState);
            }
        }
    }
}
