using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using diploma.Models;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using X.PagedList;


namespace diploma.Controllers
{
    public class HomeController : Controller
    {

        private readonly BookingContext _context;
        private readonly UserManager<User> _userManager;
        IHostingEnvironment _appEnvironment;

        public HomeController(BookingContext context,
     UserManager<User> userManager, IHostingEnvironment appEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _appEnvironment = appEnvironment;
        }

        private async Task<Order> GetCurrentOrder()
        {
            try
            {
                User usr = await _userManager.GetUserAsync(HttpContext.User);
            if (usr != null)
            {
                string id = usr.Id;
                IEnumerable<Order> orders = _context.Order.Where(p => p.UserId == id && p.Active == 1).Include(p=>p.BookOrders);
                return orders.FirstOrDefault();
            }
            else
            {
                return null;
            }
            }
            catch (Exception ex)
            {

                Log.Write(ex);
                return null;
            }
        }

        private Task<User> GetCurrentUserAsync() =>
      _userManager.GetUserAsync(HttpContext.User);

        private async Task<string> GetUserName()
        {
            try
            {
                User usr = await _userManager.GetUserAsync(HttpContext.User);
                if (usr == null)
                {
                    return "Войти";
                }
                else
                { return usr.UserName; }
            } catch (Exception ex)
            {
            
                Log.Write(ex);
                return "Войти";
            }
        }

        public IEnumerable<BookView> GetBooks()
        {
            IEnumerable<Book> books = _context.Book.Include(p => p.BookOrders).Where(d => d.isDeleted == false);
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
                IEnumerable<BookGenre> bookgenres = _context.BookGenre.Where(b => b.IdBook == item.Id);
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
            return bookViews;
        }


        [HttpGet]
        public IActionResult Index(int? page, string searchString, string sortOrder, bool? Stored, int? Genre)
        {
            int pageNumber = page ?? 1;
            Order order = GetCurrentOrder().Result;
            ViewBag.CurrentOrder = order; 
            ViewBag.Genres = _context.Genre;
            ViewBag.Username = GetUserName().Result;
            ViewBag.pageNumber = pageNumber;
            ViewBag.searchString = searchString;
            ViewBag.sortOrder = sortOrder;
            ViewBag.Stored = Stored;
            int genre = Genre ?? 0;
            ViewBag.Genre = genre;
            return View();
        }

        public IActionResult About()
        {
            ViewBag.Username = GetUserName().Result;
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public ActionResult GetView(string viewName)
        {
            if (viewName == "_BasketDiv")
            {
                Order model = GetCurrentOrder().Result;
                ViewBag.Genres = _context.Genre.OrderBy(g=>g.Name);
                return PartialView("_BasketDiv", model);
            } 
            return PartialView(viewName, null);
        }

        public IActionResult GetBookView(int page, string searchString, string sortOrder, bool Stored, int Genre)
        {
            if (page<1)
            {
                page = 1;
            }
            BookListViewModel model = new BookListViewModel()
            {
                Books = GetBooks(),
                CurrentOrder = GetCurrentOrder().Result,
                UserName = GetUserName().Result
            };
            if (!String.IsNullOrEmpty(searchString))
            { 
                //разбили до пробела на массив слов
                String[] words = searchString.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                List<BookView> newBooks = new List<BookView>();
                for (int i = 0; i < words.Length; i++)
                {  //для каждого слова
                    IEnumerable <BookAuthor> ba = _context.BookAuthor.Include(b=>b.Book).Include(b => b.Author).Where(v=>v.Author.Name.Contains(words[i], StringComparison.OrdinalIgnoreCase));
                    foreach (BookAuthor b in ba)
                    {
                        List<BookView> byAuthors = model.Books.Where(f => f.Id == b.IdBook).ToList();
                        newBooks = newBooks.Concat(byAuthors).Distinct().ToList();
                    }
                    //пока по названию, но надо еще пройтись по авторам, и соединить
                    List<BookView> list = model.Books.Where(s => s.Title.Contains(words[i], StringComparison.OrdinalIgnoreCase)).ToList();
                    newBooks = newBooks.Concat(list).Distinct().ToList();
                }
                model.Books = newBooks;
            }
            if (Genre != 0)
            {
                List<BookView> newBooks = new List<BookView>();
                IEnumerable<BookGenre> bg = _context.BookGenre.Where(b => b.IdGenre == Genre);
                foreach (BookGenre b in bg)
                {
                    BookView book = model.Books.Where(p => p.Id == b.IdBook).First();
                    if (book != null)
                    {
                        newBooks.Add(book);
                    }
                }
                model.Books = newBooks;
            }
            if (Stored)
            {
                model.Books = model.Books.Where(f => f.Stored > 0);
            }
            switch (sortOrder)
            {
                case "name_desc":
                    model.Books = model.Books.OrderByDescending(s => s.Title);
                    break;
                case "name":
                    model.Books = model.Books.OrderBy(s => s.Title);
                    break;
                case "cost_desc":
                    model.Books = model.Books.OrderByDescending(s => s.Cost);
                    break;
                case "cost":
                    model.Books = model.Books.OrderBy(s => s.Cost);
                    break;
                case "Date":
                    model.Books = model.Books.OrderBy(s => s.Year);
                    break;
                case "date_desc":
                    model.Books = model.Books.OrderByDescending(s => s.Year);
                    break;
                case "new_first":
                    model.Books = model.Books.OrderByDescending(s => s.Id);
                    break;
                default:
                    model.Books = model.Books.OrderByDescending(s => s.Id);
                    break;
            }
            
            model.Books =  model.Books.ToPagedList(page, 4);

            ViewBag.pageNumber = page;
            ViewBag.searchString = searchString;
            ViewBag.sortOrder = sortOrder;
            ViewBag.Stored = Stored;
            ViewBag.Genre = Genre;

            return PartialView("_BookList", model);
        }


        public IActionResult Contact()
        {
            ViewBag.Username = GetUserName().Result;
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            ViewBag.Username = GetUserName().Result;
            return View();
        }
       

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
