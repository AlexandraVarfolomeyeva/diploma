using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using diploma.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace diploma.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly BookingContext _context;
        public BooksController(BookingContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IEnumerable<Book> GetAll() //получение списка всех книг
        {
            try
            {
                return _context.Book.Include(p => p.BookOrders).Where(book => book.isDeleted == false);
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                return null;
            }

        }
   

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBook([FromRoute] int id)
        {//получение книги по id
            try
            {
                if (!ModelState.IsValid)
                {
                    Log.WriteSuccess(" BooksController.GetBook", "Валидация внутри контроллера неудачна.");
                    return BadRequest(ModelState);
                }

                var item = await _context.Book.SingleOrDefaultAsync(m => m.Id == id);

                if (item == null)
                {
                    Log.WriteSuccess(" BooksController.GetBook", "Книга не найдена.");
                    return NotFound();
                }
                return Ok(item);
            } catch(Exception ex)
            {
                Log.Write(ex);
                return BadRequest(ModelState);
            }
        }

        [HttpPost]
        //[Authorize(Roles = "admin")]
        public async Task<IActionResult> Create([FromBody] BookAdd item)
        {//создание новой книги возможно только администратором
            try
            {
                if (!ModelState.IsValid)
                {
                    Log.WriteSuccess(" BooksController.Create", "Валидация внутри контроллера неудачна.");
                    return BadRequest(ModelState);
                }
                Book book = new Book()
                {
                Year = item.Year,
                Title = item.Title,
                Stored = item.Stored,
                image = item.image,
                IdPublisher = item.Publisher,
                Cost = item.Cost,
                Content = item.Content,
                isDeleted=item.isDeleted
                };
                _context.Book.Add(book);
                for (int i = 0; i < item.idAuthors.Length; i++)
                {
                    BookAuthor bookauthor = new BookAuthor()
                    {
                            IdAuthor = item.idAuthors[i],
                            IdBook = book.Id
                    };
                    _context.BookAuthor.Add(bookauthor);
                }
                for (int i = 0; i < item.idGenres.Length; i++)
                {
                    BookGenre bookgenre = new BookGenre()
                    {
                        IdGenre = item.idGenres[i],
                        IdBook = book.Id
                    };
                    _context.BookGenre.Add(bookgenre);
                }
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetBook", new { id = book.Id }, book);
            } catch (Exception ex)
            {
                Log.Write(ex);
                return BadRequest(ModelState);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] Book book)
        {//обновление информации о существующей книге возможно только администратором
          try{  if (!ModelState.IsValid)
            {
                Log.WriteSuccess(" BooksController.Update", "Валидация внутри контроллера неудачна.");
                return BadRequest(ModelState);
            }
            var item = _context.Book.Find(id);
            if (item == null)
            {
                Log.WriteSuccess(" BooksController.Update", "Книга не найдена.");
                return NotFound();
            }
                item.BookOrders = book.BookOrders;
                item.Content = book.Content;
                item.Cost = book.Cost;
                item.image = book.image;
                item.Publisher = book.Publisher;
                item.Stored = book.Stored;
                item.BookAuthors = book.BookAuthors;
                item.Title = book.Title;
                item.Year = book.Year;
                item.isDeleted = book.isDeleted;
                item.IdPublisher = book.IdPublisher;
                item.BookGenres = book.BookGenres;
            _context.Book.Update(item);
            await _context.SaveChangesAsync();
            return NoContent();
        } catch (Exception ex)
            {
                Log.Write(ex);
                return BadRequest(ModelState);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {//удаление книги из БД возможно только администратором
            try
            {
                if (!ModelState.IsValid)
                {
                    Log.WriteSuccess(" BooksController.Delete", "Валидация внутри контроллера неудачна.");
                    return BadRequest(ModelState);
                }
                Book item = _context.Book.Find(id);
                if (item == null)
                {
                    Log.WriteSuccess(" BooksController.Delete", "Книга не найдена.");
                    return NotFound();
                }
                //_context.Book.Remove(item);
                IEnumerable <BookOrder> lines = _context.BookOrder.Where(l=>l.IdBook==id);
                foreach (BookOrder i in lines)
                {
                    Order order = _context.Order.Find(i.IdOrder);
                    if (order.Active == 1)
                    {
                        order.Amount -= i.Amount;
                        order.SumOrder -= item.Cost * i.Amount;
                        _context.Order.Update(order);
                        _context.BookOrder.Remove(i);
                    }
                }
                item.isDeleted = true;
                _context.Book.Update(item);
                await _context.SaveChangesAsync();
                return NoContent();
            } catch (Exception ex)
            {
                Log.Write(ex);
                return BadRequest(ModelState);
            }
        }
    }
}
