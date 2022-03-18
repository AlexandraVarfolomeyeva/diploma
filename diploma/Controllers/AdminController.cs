using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BLL.Interfaces;
using BLL.Models;
using DAL.Entities;
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
        private readonly IDBCrud _context;
        private readonly UserManager<User> _userManager;
        IHostingEnvironment _appEnvironment;

        public AdminController(IDBCrud context,
     UserManager<User> userManager, IHostingEnvironment appEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _appEnvironment = appEnvironment;
        }
     
        
        #region private helping fuctions
       
        
        /// <summary>
        /// Получить текущего пользователя
        /// </summary>
        /// <returns>Пользователь</returns>
        private Task<User> GetCurrentUserAsync() =>
            _userManager.GetUserAsync(HttpContext.User);

        /// <summary>
        /// Получить имя текущего пользователя
        /// </summary>
        /// <returns>строка с именем пользователя</returns>
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

        /// <summary>
        /// Получить активный заказ текущего пользователя
        /// </summary>
        /// <returns>Модель текущего заказа</returns>
        private async Task<OrderModel> GetCurrentOrder()
        {
            try
            {
                User usr = await _userManager.GetUserAsync(HttpContext.User);
                if (usr != null)
                {
                    string id = usr.Id;
                    IEnumerable<OrderModel> orders = _context.GetAllOrders().Where(p => p.UserId == id && p.Active == 1);
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

        /// <summary>
        /// Получить роль текущего пользователя
        /// </summary>
        /// <returns>строкус именем роли</returns>
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

        /// <summary>
        /// Поиск книги по id и преобразование к нужному типу BookAdd
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private BookAdd FindBook(int id)
        {
            BookAdd b = _context.GetBook(id);
            if (b == null)
            {
                Log.WriteSuccess(" AdminController.GetBook", "Книга не найдена.");
                return null;
            }
            ViewBag.image = b.image;
            List<int> au = new List<int>();
            List<string> aus = new List<string>();
            List<int> ge = new List<int>();
            List<string> ges = new List<string>();
            IEnumerable<BookAuthorModel> bookauthors = _context.GetAllBookAuthors().Where(f => f.IdBook == b.Id);
            IEnumerable<BookGenreModel> bookgenres = _context.GetAllBookGenres().Where(f => f.IdBook == b.Id);
            foreach (BookAuthorModel line in bookauthors)
            {
                AuthorModel author = _context.GetAuthor(line.IdAuthor);
                au.Add(author.Id);
                aus.Add(author.Name);
            }
            b.idAuthors = au.ToArray();
            b.Authors = aus.ToArray();
            foreach (BookGenreModel line in bookgenres)
            {
                GenreModel genre = _context.GetGenre(line.IdGenre);
                ge.Add(genre.Id);
                ges.Add(genre.Name);
            }
            b.idGenres = ge.ToArray();
            b.Genres = ges.ToArray();
            return b;
        }

        /// <summary>
        /// Отфильтровать города
        /// </summary>
        /// <param name="search">название города или id</param>
        /// <param name="order">порядок сортировки</param>
        /// <returns></returns>
        private List<CityModel> FilterCities(string search, string order)
        {
            List<CityModel> model = _context.GetAllCities().ToList();
            if (!string.IsNullOrEmpty(search))
            {
                String[] words = search.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                List<CityModel> newmodel = new List<CityModel>();
                for (int i = 0; i < words.Length; i++)
                {  //для каждого слова
                    List<CityModel> found = model.Where(f => f.Name.Contains(words[i], StringComparison.OrdinalIgnoreCase)).ToList();
                    newmodel = newmodel.Concat(found).Distinct().ToList();

                    int id;
                    if (Int32.TryParse(words[i], out id))
                    {
                        found = model.Where(f => f.Id == id).ToList();
                        newmodel = newmodel.Concat(found).Distinct().ToList();
                    }
                }
                model = newmodel;
            }

            switch (order)
            {
                case "alphabet": model = model.OrderBy(d => d.Name).ToList(); break;
                case "alphabetDesc": model = model.OrderByDescending(d => d.Name).ToList(); break;
                case "time": model = model.OrderBy(d => d.DeliveryTime).ThenBy(b => b.Name).ToList(); break;
                case "timeDesc": model = model.OrderByDescending(d => d.DeliveryTime).ThenBy(b => b.Name).ToList(); break;
                case "cost": model = model.OrderBy(d => d.DeliverySum).ThenBy(b => b.Name).ToList(); break;
                case "costDesc": model = model.OrderByDescending(d => d.DeliverySum).ThenBy(b => b.Name).ToList(); break;
                default: model = model.OrderBy(d => d.Name).ToList(); break;
            }
            return model;
        }


        /// <summary>
        /// Отфильтровать заказы по заданным параметрам
        /// </summary>
        /// <param name="status">Статус</param>
        /// <param name="period">Период(день, неделя, месяц, год)</param>
        /// <param name="sort">Порядок сортировки</param>
        /// <param name="search">Слово(-а) для поиска</param>
        /// <returns>Список отсортированных заказов для представления администратору AdminOrderView</returns>
        private List<AdminOrderView> GetFiltered(int status, string period, string sort, string search)
        {
            IEnumerable<OrderModel> orders = _context.GetAllOrders().Where(n => n.Active != 1);
            List<AdminOrderView> modelList = new List<AdminOrderView>();
            foreach (OrderModel o in orders)
            {
                AddressModel ad = _context.GetAddress(o.AddressId);
                o.BookOrders = _context.GetAllBookOrders().Where(p => p.IdOrder == o.Id).ToList();
                List<BookOrderView> bol = new List<BookOrderView>();
                foreach (BookOrderModel b in o.BookOrders)
                {
                    BookAdd book = _context.GetBook(b.IdBook);
                    BookView bookview = new BookView()
                    {
                        Content = book.Content,
                        Cost = book.Cost,
                        Id = book.Id,
                        image = book.image,
                        Stored = book.Stored,
                        Publisher = _context.GetPublisher(book.Publisher).Name,
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
                o.User = _context.GetUser(o.UserId);
                CityModel city = _context.GetCity(ad.IdCity);
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
                    Address = ad.Name,
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
                    if (Int32.TryParse(words[i], out id))
                    {
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
                case "No_desc": modelList = modelList.OrderByDescending(f => f.Id).ToList(); break;
                case "Order": modelList = modelList.OrderBy(f => f.DateOrder).ToList(); break;
                case "Order_desc": modelList = modelList.OrderByDescending(f => f.DateOrder).ToList(); break;
                case "Delivery": modelList = modelList.OrderBy(f => f.DateDelivery).ToList(); break;
                case "Delivery_desc": modelList = modelList.OrderByDescending(f => f.DateDelivery).ToList(); break;
                default: modelList = modelList.OrderBy(f => f.Id).ToList(); break;
            }
            return modelList;
        }
        #endregion

        #region Orders list for admin
        [HttpPost]
        public IActionResult Search(int status, string period, string sort, string search)
        {
            return RedirectToAction("OrderList", "Admin", new { page = 1, status = status, period = period, sort = sort, search = search });
        }

        [HttpPut]
        [Authorize(Roles = "seller")]
        public IActionResult ChangeStatus(int id, int option)
        {
            try
            {
                OrderModel o = _context.GetOrder(id);
                if (o.Active == 3 && option != 3)
                {
                    UserModel usr = _context.GetUser(o.UserId);
                    CityModel city = _context.GetCity(_context.GetAddress(o.AddressId).IdCity);
                    o.DateDelivery = o.DateOrder.AddDays(city.DeliveryTime);
                }
                o.Active = option;
                switch (option)
                {
                    case 2: o.DateSent = DateTime.Now.Date; break;
                    case 3:
                        o.DateDelivery = DateTime.Now.Date;
                        UserModel user = _context.GetUser(o.UserId);
                        if (o.SumOrder >= 5000 && o.SumOrder < 10000 && user.Discount<5)
                        {
                            user.Discount = 5;
                            _context.UpdateUser(user);
                            OrderModel order = _context.GetAllOrders().Where(a=>a.Active==1 && a.UserId==user.Id).First();
                            IEnumerable<BookOrderModel> bo = _context.GetAllBookOrders().Where(d => d.IdOrder == order.Id);
                            foreach (BookOrderModel bod in bo)
                            {
                                BookModel book = _context.GetBookModel(bod.Id);
                                bod.Price = (book.Cost * (float)(100 - user.Discount)) / 100;
                                _context.UpdateBookOrder(bod);
                            }
                        }
                        else if (o.SumOrder >= 10000 && user.Discount < 7)
                        {
                            user.Discount = 7;
                            _context.UpdateUser(user);
                            OrderModel order = _context.GetAllOrders().Where(a => a.Active == 1 && a.UserId == user.Id).First();
                            IEnumerable<BookOrderModel> bo = _context.GetAllBookOrders().Where(d => d.IdOrder == order.Id);
                            foreach (BookOrderModel bod in bo)
                            {
                                BookModel book = _context.GetBookModel(bod.Id);
                                bod.Price = (book.Cost * (float)(100 - user.Discount)) / 100;
                                _context.UpdateBookOrder(bod);
                            }
                        }
                        break;
                }
                _context.UpdateOrder(o);
                return Ok(o.DateDelivery.ToString("D", CultureInfo.CreateSpecificCulture("ru-RU")));
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [Authorize(Roles = "seller")]
        public IActionResult OrderList(int? page, int status, string period, string sort, string search)
        {
            int pageNumber = page ?? 1;
            ViewBag.status = status;
            ViewBag.period = period;
            ViewBag.sort = sort;
            ViewBag.search = search;
            ViewBag.Username = GetUserName().Result;
            IEnumerable<AdminOrderView> model = GetFiltered(status, period, sort, search);
            return View(model.ToPagedList(pageNumber, 20));
        }

        #endregion

        #region Comments for User(watch, create) and Admin(delete)
        [HttpPost]
        public IActionResult Comments(CommentModel comment)
        {
            try
            {
                comment.DateComment = DateTime.Now;
                _context.CreateComment(comment);
                return CreatedAtAction("Comment", new { id = comment.Id }, comment);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        public IActionResult GetCommentsView(int id)
        {
            BookAdd book = FindBook(id);
            ViewBag.Username = GetUserName().Result;
            ViewBag.Role = GetRole().Result;
            BookDetails model = new BookDetails()
            {
                Book = book,
                Comments = _context.GetAllComments().Where(c => c.IdBook == book.Id)
            };
            return PartialView("_Comments", model);
        }

        [HttpDelete]
        [Authorize(Roles = "seller")]
        public IActionResult DeleteComment(int id)
        {
            try
            {
                _context.DeleteComment(id);
                return Ok();
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                return BadRequest(ex);
            }
        }

        #endregion

        #region Detailed information about book (for User)
        [HttpGet]
        //[Route("/Admin/Book/{id}")][FromRoute]
        public IActionResult Book(int id)
        {
            ViewBag.Username = GetUserName().Result;
            ViewBag.Genres = _context.GetAllGenres();
            ViewBag.Role = GetRole().Result;
            try
            {
                if (!ModelState.IsValid)
                {
                    Log.WriteSuccess(" BookViewsController.GetBook", "Валидация внутри контроллера неудачна.");
                    return BadRequest(ModelState);
                }

                BookAdd b = FindBook(id);
                BookDetails bd = new BookDetails()
                {
                    Book = b,
                    CurrentOrder = GetCurrentOrder().Result,
                    Comments = _context.GetAllComments().Where(c => c.IdBook == b.Id)
                };
                return View(bd);
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
            return View();
        }
        #endregion

        #region adding book for admin
        [HttpGet]
        [Authorize(Roles = "seller")]
        public IActionResult AddBook(string filename)
        {
            ViewBag.Username = GetUserName().Result;
            ViewBag.FileName = filename;
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "seller")]
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


        [HttpGet]
        [Authorize(Roles = "seller")]
        public IActionResult UploadPicture()
        {
            ViewBag.Username = GetUserName().Result;
            return View();
        }
        #endregion

        #region cities for admin
        public IActionResult Cities()
        {
            ViewBag.Username = GetUserName().Result;
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "seller")]
        public IActionResult Cities(CityModel city)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.CreateCity(city);
                    return CreatedAtAction("GetAuthor", new { id = city.Id }, city);
                }
                else
                {
                    Log.WriteSuccess("/Admin/Cities/[Post] ", "Модель не валидна!");
                    return Conflict(ModelState);
                }

            }
            catch (Exception ex)
            {
                Log.Write(ex);
                return BadRequest(ex);
            }
        }

        [HttpGet]
        public IActionResult City(int id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var city = _context.GetCity(id);
                    return Ok(city);
                }
                else
                {
                    Log.WriteSuccess("/Admin/Cities/[Get] ", "Модель не валидна!");
                    return Conflict(ModelState);
                }

            }
            catch (Exception ex)
            {
                Log.Write(ex);
                return BadRequest(ex);
            }
        }

        [HttpDelete]
        [Authorize(Roles = "seller")]
        public IActionResult DeleteCity(int id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.DeleteCity(id);
                    return Ok();
                }
                else
                {
                    Log.WriteSuccess("/Admin/Cities/[delete] ", "Модель не валидна!");
                    return Conflict(ModelState);
                }

            }
            catch (Exception ex)
            {
                Log.Write(ex);
                return BadRequest(ex);
            }
        }


        [HttpPut]
        [Authorize(Roles = "seller")]
        public IActionResult EditCity(int id, CityModel city)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    city.Id = id;
                    _context.UpdateCity(city);
                    return NoContent();
                }
                else
                {
                    Log.WriteSuccess("/Admin/Cities/[put] ", "Модель не валидна!");
                    return Conflict(ModelState);
                }

            }
            catch (Exception ex)
            {
                Log.Write(ex);
                return BadRequest(ex);
            }
        }

        [Authorize(Roles = "seller")]
        public IActionResult GetCitiesTable(string search, string order)
        {
            IEnumerable<CityModel> model = FilterCities(search, order);
            ViewBag.order = order;
            ViewBag.search = search;
            return PartialView("_CitiesTable", model);
        }
        #endregion
    }
}