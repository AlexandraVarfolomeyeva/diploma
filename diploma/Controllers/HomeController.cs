using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using X.PagedList;
using BLL.Models;
using BLL.Interfaces;
using DAL.Entities;

namespace diploma.Controllers
{
    public class HomeController : Controller
    {

        private readonly IDBCrud _context;
        private readonly UserManager<User> _userManager;
        IHostingEnvironment _appEnvironment;

        public HomeController(IDBCrud context,
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

        private async Task<OrderModel> GetCurrentOrder()
        {
            try
            {
                User usr = await _userManager.GetUserAsync(HttpContext.User);
                if (usr != null)
                {
                    string id = usr.Id;
                    IEnumerable<OrderModel> orders = _context.GetAllOrders();
                    orders = orders.Where(p => p.UserId == id && p.Active == 1);
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
            return _context.GetAllBookViews();
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
                    IEnumerable<BookAuthorModel> ba = _context.GetAllBookAuthors();
                    foreach (BookAuthorModel b in ba)
                    {
                        AuthorModel author = _context.GetAuthor(b.IdAuthor);
                        if (author.Name.Contains(words[i], StringComparison.OrdinalIgnoreCase))
                        {
                            List<BookView> byAuthors = books.Where(f => f.Id == b.IdBook).ToList();
                            newBooks = newBooks.Concat(byAuthors).Distinct().ToList();
                        }

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
                IEnumerable<BookGenreModel> bg = _context.GetAllBookGenres().Where(b => b.IdGenre == Genre);
                foreach (BookGenreModel b in bg)
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
            ViewBag.Genres = _context.GetAllGenres();
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
            ViewBag.Genres = _context.GetAllGenres().OrderBy(g => g.Name);
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
                Genre = Genre,
                page = page,
                AuthorSearch = AuthorSearch,
                searchString = searchString,
                sortOrder = sortOrder
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
            IEnumerable<CityModel> cities = _context.GetAllCities();
            return View(cities);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
