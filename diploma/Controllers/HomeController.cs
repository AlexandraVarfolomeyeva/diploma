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

        private async Task<string> GetRole()
        {
            try
            {
                User usr = await GetCurrentUserAsync();
                if (usr != null)
                {
                    string role = _userManager.GetRolesAsync(usr).Result.First();
                    return role;
                }
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
            return "";
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


       public IEnumerable<BookView> FilterBooks(string searchString, string sortOrder, bool Stored, int Genre, string AuthorSearch)
        {
            IEnumerable<BookView> books = GetBooks();
            if (!String.IsNullOrEmpty(searchString))
            {
                //разбили до пробела на массив слов
                String[] words = searchString.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                List<BookView> newBooks = new List<BookView>();
                for (int i = 0; i < words.Length; i++)
                {  //для каждого слова
                    IEnumerable<BookAuthor> ba = _context.BookAuthor.Include(b => b.Book).Include(b => b.Author).Where(v => v.Author.Name.Contains(words[i], StringComparison.OrdinalIgnoreCase));
                    foreach (BookAuthor b in ba)
                    {
                        List<BookView> byAuthors = books.Where(f => f.Id == b.IdBook).ToList();
                        newBooks = newBooks.Concat(byAuthors).Distinct().ToList();
                    }
                    //пока по названию, но надо еще пройтись по авторам, и соединить
                    List<BookView> list = books.Where(s => s.Title.Contains(words[i], StringComparison.OrdinalIgnoreCase)).ToList();
                    newBooks = newBooks.Concat(list).Distinct().ToList();
                }
                books = newBooks;
            }

            if (!String.IsNullOrEmpty(AuthorSearch)) {
                List<BookView> newBooks = new List<BookView>();
                foreach (BookView book in books)
                {
                    foreach (string au in book.Authors)
                    {
                        if (au == AuthorSearch)
                        {
                            newBooks.Add(book);
                        }
                    }
                }
                books = newBooks;
            }
                if (Genre != 0)
            {
                List<BookView> newBooks = new List<BookView>();
                IEnumerable<BookGenre> bg = _context.BookGenre.Where(b => b.IdGenre == Genre);
                foreach (BookGenre b in bg)
                {
                    IEnumerable<BookView> book = books.Where(p => p.Id == b.IdBook);
                    if (book.Any())
                    {
                        newBooks.Add(book.First());
                    }
                }
                books = newBooks;
            }
            if (Stored)
            {
                books = books.Where(f => f.Stored > 0);
            }
            switch (sortOrder)
            {
                case "name_desc":
                    books = books.OrderByDescending(s => s.Title);
                    break;
                case "name":
                    books = books.OrderBy(s => s.Title);
                    break;
                case "cost_desc":
                    books = books.OrderByDescending(s => s.Cost);
                    break;
                case "cost":
                    books = books.OrderBy(s => s.Cost);
                    break;
                case "Date":
                    books = books.OrderBy(s => s.Year);
                    break;
                case "date_desc":
                    books = books.OrderByDescending(s => s.Year);
                    break;
                case "new_first":
                    books = books.OrderByDescending(s => s.Id);
                    break;
                default:
                    books = books.OrderByDescending(s => s.Id);
                    break;
            }
            return books;
        }

        [HttpPost]
        public IActionResult Search(string searchString, string sortOrder, bool Stored, int Genre)
        {
           return RedirectToAction("Index", "Home", new { page = 1, searchString = searchString, sortOrder = sortOrder, Stored = Stored, Genre = Genre });
        }


        [HttpGet]
        public IActionResult Index(BookListViewModel model, int? page, string searchString, string sortOrder, bool Stored, int Genre, string AuthorSearch)
        {
            int pageNumber = page ?? 1;
            if (model.CurrentOrder == null)
            { model.CurrentOrder = GetCurrentOrder().Result; }
            if (String.IsNullOrEmpty(model.UserName))
            {
                model.UserName = GetUserName().Result;
            }
            if (String.IsNullOrEmpty(sortOrder))
            {
                model.sortOrder = "new_first";
            }
            IEnumerable<BookView> books = FilterBooks(searchString, sortOrder, Stored, Genre, AuthorSearch);
            model.Books = books.ToPagedList(pageNumber, 12);
            
            model.Genre = Genre;
                model.searchString = searchString;
                model.sortOrder = sortOrder;
                model.Stored = Stored;
            model.AuthorSearch = AuthorSearch;
            ViewBag.Genres = _context.Genre;
            ViewBag.Username = GetUserName().Result;
            ViewBag.Role = GetRole().Result;
            return View(model);
        }

        public IActionResult About()
        {
            ViewBag.Username = GetUserName().Result;
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public ActionResult GetBasketView()
        {
            BookListViewModel bvm = new BookListViewModel()
            {
                UserName = GetUserName().Result,
                CurrentOrder = GetCurrentOrder().Result
            };
            ViewBag.Genres = _context.Genre.OrderBy(g => g.Name);
            return PartialView("_BasketDiv", bvm);
        }

        public IActionResult GetBookView(int page, string searchString, string sortOrder, bool Stored, int Genre, string AuthorSearch)
        {
            page = 1;
            IEnumerable<BookView> books = FilterBooks(searchString, sortOrder, Stored, Genre, AuthorSearch);
            BookListViewModel bvm = new BookListViewModel()
            {
                Stored = Stored,
                Books = books.ToPagedList(page, 12),
                UserName = GetUserName().Result,
                CurrentOrder = GetCurrentOrder().Result,
                Genre=Genre,
                page=page,
                AuthorSearch= AuthorSearch,
                searchString =searchString,
                sortOrder=sortOrder
            };
            return PartialView("_BookList", bvm);
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
            HttpContext.Request.Cookies.Any();
            return View();
        }
        
        public IActionResult PriceAndDelivery()
        {
            ViewBag.Username = GetUserName().Result;
            IEnumerable<City> cities = _context.City;
            return View(cities);
        }

         [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
