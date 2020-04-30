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
                IEnumerable<Order> orders = _context.Order.Where(p => p.UserId == id && p.Active == 1);
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




        [HttpGet]
        public IActionResult Index(int? page)
        {
            int pageSize = 4;
            int pageNumber = (page ?? 1);
            Order order = GetCurrentOrder().Result;
            ViewBag.CurrentOrder = order; //
            ViewBag.Username = GetUserName().Result;
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
                ViewBag.Books = bookViews;
           
            //return View(bookViews.ToPagedList(pageNumber, pageSize));
            return View();
        }

        public IActionResult About()
        {
            ViewBag.Username = GetUserName().Result;
            ViewData["Message"] = "Your application description page.";

            return View();
        }



        public ActionResult GetView()
        {
            Order model = GetCurrentOrder().Result;
            return PartialView("_BasketDiv",model);
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
