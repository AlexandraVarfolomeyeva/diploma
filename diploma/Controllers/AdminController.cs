using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using diploma.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace diploma.Controllers
{
    public class AdminController : Controller
    {


        private readonly BookingContext _context;
        private readonly UserManager<User> _userManager;
        IHostingEnvironment _appEnvironment;

        public AdminController(BookingContext context,
     UserManager<User> userManager, IHostingEnvironment appEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _appEnvironment = appEnvironment;
        }

        private List<AdminOrderView> GetFiltered(int status, string period, string sort, string search)
        {
            IEnumerable<Order> orders = _context.Order.Include(n => n.BookOrders).Where(n => n.Active != 1).Include(n => n.User);
            List<AdminOrderView> modelList = new List<AdminOrderView>();
            foreach (Order o in orders)
            {
                City city = _context.City.Find(o.User.IdCity);
                List<BookOrderView> bol = new List<BookOrderView>();
                foreach (BookOrder b in o.BookOrders)
                {
                    Book book = _context.Book.Find(b.IdBook);
                    BookView bookview = new BookView()
                    {
                        Content = book.Content,
                        Cost = book.Cost,
                        Id = book.Id,
                        image = book.image,
                        Stored = book.Stored,
                        Publisher = _context.Publisher.Find(book.IdPublisher).Name,
                        Title = book.Title,
                        Year = book.Year
                    };
                    BookOrderView bo = new BookOrderView()
                    {
                        Amount = b.Amount,
                        Id = b.Id,
                        Book = bookview
                    };
                    bol.Add(bo);
                }
                AdminOrderView c = new AdminOrderView()
                {
                    Active = o.Active,
                    Amount = o.Amount,
                    DateDelivery = o.DateDelivery,
                    DateOrder = o.DateOrder,
                    Id = o.Id,
                    SumOrder = o.SumOrder,
                    City = city.Name,
                    SumDelivery = city.DeliverySum,
                    Address = o.User.Address,
                    Email = o.User.Email,
                    FIO = o.User.Fio,
                    Phone = o.User.PhoneNumber,
                    BookOrders = bol
                };
                modelList.Add(c);
            }
            if (!String.IsNullOrEmpty(search))
            {
                String[] words = search.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                List<AdminOrderView> newmodel = new List<AdminOrderView>();
                for (int i = 0; i < words.Length; i++)
                {  //для каждого слова
                    List<AdminOrderView> model = modelList.Where(f => f.FIO.Contains(words[i], StringComparison.OrdinalIgnoreCase)).ToList();
                    newmodel = newmodel.Concat(model).Distinct().ToList();

                    model = modelList.Where(f => f.Address.Contains(words[i], StringComparison.OrdinalIgnoreCase)).ToList();
                    newmodel = newmodel.Concat(model).Distinct().ToList();

                    model = modelList.Where(f => f.City.Contains(words[i], StringComparison.OrdinalIgnoreCase)).ToList();
                    newmodel = newmodel.Concat(model).Distinct().ToList();

                    int id;
                    if (Int32.TryParse(words[i], out id)) {
                        model = modelList.Where(f => f.Id == id).ToList();
                        newmodel = newmodel.Concat(model).Distinct().ToList();
                    }
                }
                modelList = newmodel;
            }
            if (status != 1)
            {
                modelList = modelList.Where(f => f.Active == status).ToList();
            }
            switch (period)
            {
                case "all": break;
                case "day": modelList = modelList.Where(f => f.DateOrder.Date == DateTime.Today).ToList(); break;
                case "week": modelList = modelList.Where(f => f.DateOrder.Date.CompareTo(DateTime.Today.AddDays(-7)) >= 0).ToList(); break;
                case "month": modelList = modelList.Where(f => f.DateOrder.Date.CompareTo(DateTime.Today.AddMonths(-1)) >= 0).ToList(); break;
                case "year": modelList = modelList.Where(f => f.DateOrder.Date.CompareTo(DateTime.Today.AddYears(-1)) >= 0).ToList(); break;
                default: break;
            }
            switch (sort)
            {
                case "No": modelList = modelList.OrderBy(f => f.Id).ToList(); break;
                case "No_desc": modelList.OrderByDescending(f => f.Id).ToList(); break;
                case "Order": modelList.OrderBy(f => f.DateOrder).ToList(); break;
                case "Order_desc": modelList.OrderByDescending(f => f.DateOrder).ToList(); break;
                case "Delivery": modelList.OrderBy(f => f.DateDelivery).ToList(); break;
                case "Delivery_desc": modelList.OrderByDescending(f => f.DateDelivery).ToList(); break;
                default: modelList = modelList.OrderBy(f => f.Id).ToList(); break;
            }
            return modelList;
        }

        [HttpPost]
        public IActionResult Search(int status, string period, string sort, string search)
        {
            return RedirectToAction("OrderList", "Admin", new { page = 1, status = status, period = period, sort = sort, search = search });
        }

        [Authorize(Roles = "admin")]
        public IActionResult OrderList(int? page, int status, string period, string sort, string search)
        {
            int pageNumber = page ?? 1;
            ViewBag.status = status;
            ViewBag.period = period;
            ViewBag.sort = sort;
            ViewBag.search = search;
            IEnumerable<AdminOrderView> model = GetFiltered(status, period, sort, search);
            return View(model.ToPagedList(pageNumber, 3));
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
            }
            catch (Exception ex)
            {

                Log.Write(ex);
                return "Войти";
            }
        }


        private async Task<Order> GetCurrentOrder()
        {
            try
            {
                User usr = await _userManager.GetUserAsync(HttpContext.User);
                if (usr != null)
                {
                    string id = usr.Id;
                    IEnumerable<Order> orders = _context.Order.Where(p => p.UserId == id && p.Active == 1).Include(p => p.BookOrders);
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


        [HttpGet]
        //[Route("/Admin/Book/{id}")][FromRoute]
        public async Task<IActionResult> Book(int id)
        {
            ViewBag.Username = GetUserName().Result;

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
                    Content = item.Content,
                    isDeleted = item.isDeleted,
                    Cost = item.Cost,
                    image = item.image,
                    Publisher = item.IdPublisher,
                    Stored = item.Stored,
                    Title = item.Title,
                    Year = item.Year
                };
                ViewBag.image = item.image;
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
                BookDetails bd = new BookDetails()
                {
                    Book = b,
                    CurrentOrder = GetCurrentOrder().Result
                };
                return View(bd);
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
            return View();
        }


        [HttpGet]
        [Authorize(Roles = "admin")]
        public IActionResult AddBook(string filename)
        {
            ViewBag.Username = GetUserName().Result;
            ViewBag.FileName = filename;
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public IActionResult UploadPicture()
        {
            ViewBag.Username = GetUserName().Result;
            return View();
        }


        [HttpPut]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> ChangeStatus(int id, int option)
        {
            try
            {
                Order o = _context.Order.Find(id);
                if (o.Active == 3 && option != 3)
                {
                    User usr = _context.User.Find(o.UserId);
                    City city = _context.City.Find(usr.IdCity);
                    o.DateDelivery = o.DateOrder.AddDays(city.DeliveryTime);
                }
                o.Active = option;
                if (option == 3)
                { o.DateDelivery = DateTime.Now.Date; }
                _context.Order.Update(o);
                await _context.SaveChangesAsync();
                return Ok(o.DateDelivery.ToString("D", CultureInfo.CreateSpecificCulture("ru-RU")));
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }


        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> UploadPicture(IFormFile file)
        {
            //Task<ActionResult>
            if (file != null)
                try
                {
                    string format;
                    String[] words = file.FileName.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                    format = words[words.Length - 1];
                    string b = DateTime.Now.ToFileTime() + "." + format;
                    //Path.GetFileName(file.FileName)
                    string path = Path.Combine(_appEnvironment.WebRootPath + "\\img\\", b);
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                    //RedirectToAction("AddBook","Admin", new { filename = b });
                    return RedirectToAction("AddBook", new { filename = b });
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "ERROR:" + ex.Message.ToString();
                }
            else
            {
                ViewBag.Message = "You have not specified a file.";
            }
            //return BadRequest();
            return View();
            //return RedirectToAction("Index");
        }

    }
}