using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookShop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BookShop.Controllers
{
    [Route("api/[controller]")]
    public class ViewController : Controller
    {
        private readonly BookingContext _context;
        public ViewController(BookingContext context)
        {
            _context = context;
        }


        // GET: api/<controller>
        [HttpGet]
        public IEnumerable<BookView> GetAllView() //получение списка всех книг
        {
            try
            {
                IEnumerable<Book> books = _context.Book.Include(p => p.BookOrders).Where(d=>d.isDeleted==false);
                BookView[] bookViews = new BookView[books.Count()];
                int i = 0;
                foreach (Book item in books)
                {
                    bookViews[i] = new BookView()
                    {
                            Id = item.Id,
                            image = item.image,
                            Stored = item.Stored,
                            Title = item.Title,
                            Year = item.Year,
                            Cost = item.Cost,
                            Content = item.Content
                    };
                    Publisher publisher = _context.Publisher.Find(item.IdPublisher);
                    List<string> au = new List<string>();
                    List<string> ge = new List<string>();
                    bookViews[i].Publisher = publisher.Name;
                    IEnumerable<BookAuthor> bookauthors = _context.BookAuthor.Where(b => b.IdBook == item.Id);
                    IEnumerable<BookGenre> bookgenres = _context.BookGenre.Where(b => b.IdBook==item.Id);
                    foreach (BookAuthor line in bookauthors)
                    {
                            Author author = _context.Author.Find(line.IdAuthor);
                            au.Add(author.Name);
                    }
                    bookViews[i].Authors = au.ToArray();
                    foreach (BookGenre line in bookgenres)
                    {
                            Genre genre = _context.Genre.Find(line.IdGenre);
                            ge.Add(genre.Name);
                    }
                    bookViews[i].Genres = ge.ToArray();
                    i++;
                }
                IEnumerable<BookView> views = bookViews;
                return views;
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                return null;
            }

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookView([FromRoute] int id)
        {//получение книги по id
            try
            {
                if (!ModelState.IsValid)
                {
                    Log.WriteSuccess(" BookViewsController.GetBook", "Валидация внутри контроллера неудачна.");
                    return BadRequest(ModelState);
                }

                var item = await _context.Book.SingleOrDefaultAsync(m => m.Id == id);

                if (item == null)
                {
                    Log.WriteSuccess(" BookViewsController.GetBook", "Книга не найдена.");
                    return NotFound();
                }
                BookAdd b = new BookAdd()
                {
                    Id = item.Id,
                    Content=item.Content,
                    isDeleted=item.isDeleted,
                    Cost=item.Cost,
                    image=item.image,
                    Publisher=item.IdPublisher,
                    Stored=item.Stored,
                    Title=item.Title,
                    Year = item.Year
                };
                List<int> au = new List<int>();
                List<string> aus = new List<string>();
                List<int> ge = new List<int>();
                List<string> ges = new List<string>();
                IEnumerable<BookAuthor> bookauthors = _context.BookAuthor.Where(f => f.IdBook == item.Id);
                IEnumerable<BookGenre> bookgenres = _context.BookGenre.Where(f => f.IdBook == item.Id);
                foreach (BookAuthor line in bookauthors)
                {
                    Author author = _context.Author.Find(line.IdAuthor);
                    au.Add(author.Id);
                    aus.Add(author.Name);
                }
                b.idAuthors = au.ToArray();
                b.Authors = aus.ToArray();
                foreach (BookGenre line in bookgenres)
                {
                    Genre genre = _context.Genre.Find(line.IdGenre);
                    ge.Add(genre.Id);
                    ges.Add(genre.Name);
                }
                b.idGenres = ge.ToArray();
                  b.Genres = ges.ToArray();
                return Ok(b);
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                return BadRequest(ModelState);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] BookAdd book)
        {//обновление информации о существующей книге возможно только администратором
            try
            {
                if (!ModelState.IsValid)
                {
                    Log.WriteSuccess(" BookViewsController.Update", "Валидация внутри контроллера неудачна.");
                    return BadRequest(ModelState);
                }
                var item = _context.Book.Find(id);
                if (item == null)
                {
                    Log.WriteSuccess(" BookViewsController.Update", "Книга не найдена.");
                    return NotFound();
                }
                item.Content = book.Content;
                item.Cost = book.Cost;
                item.image = book.image;
                item.Stored = book.Stored;
                item.Title = book.Title;
                item.Year = book.Year;
                item.isDeleted = book.isDeleted;
                item.IdPublisher = book.Publisher;

                IEnumerable<BookAuthor> bookauthors = _context.BookAuthor.Where(b => b.IdBook == item.Id);
                IEnumerable<BookGenre> bookgenres = _context.BookGenre.Where(b => b.IdBook == item.Id);
                foreach (BookAuthor line in bookauthors)
                {
                        IEnumerable<int> el = book.idAuthors.Where(a => a == line.IdAuthor);
                    if (el.Any())//found the same 
                    {
                        book.idAuthors = book.idAuthors.Where(val => val != el.FirstOrDefault()).ToArray();
                    }
                    else {
                        _context.BookAuthor.Remove(line);
                         }
                }
                while (book.idAuthors.Any())//found the new 
                {
                    int el = book.idAuthors[0];
                    BookAuthor z = new BookAuthor() {
                        IdBook = id,
                        IdAuthor = el
                    };
                    _context.BookAuthor.Add(z);
                    book.idAuthors = book.idAuthors.Where(val => val != el).ToArray();
                }

                foreach (BookGenre line in bookgenres)
                {
                    IEnumerable<int> el = book.idGenres.Where(a => a == line.IdGenre);
                    if (el.Any())//found the same 
                    {
                        book.idGenres = book.idGenres.Where(val => val != el.FirstOrDefault()).ToArray();
                    }
                    else
                    {
                        _context.BookGenre.Remove(line);
                    }
                }

                while (book.idGenres.Any())//found the new 
                {
                    int el = book.idGenres[0];
                    BookGenre z = new BookGenre()
                    {
                        IdBook = id,
                        IdGenre = el
                    };
                    _context.BookGenre.Add(z);
                    book.idGenres = book.idGenres.Where(val => val != el).ToArray();
                }

                _context.Book.Update(item);
                await _context.SaveChangesAsync();
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
