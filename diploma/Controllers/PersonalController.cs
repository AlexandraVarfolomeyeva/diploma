using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BLL.Interfaces;
using BLL.Models;
using DAL.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace diploma.Controllers
{
    public class PersonalController : Controller
    {
        private readonly IDBCrud _context;
        private readonly UserManager<User> _userManager;

        public PersonalController(IDBCrud context,
     UserManager<User> userManager, IHostingEnvironment appEnvironment)
        {
            _context = context;
            _userManager = userManager;
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


        private BookView GetViewByBook(BookModel book)
        {
            PublisherModel publisher = _context.GetPublisher(book.IdPublisher);
            List<string> au = new List<string>();
            List<string> ge = new List<string>();
            IEnumerable<BookAuthorModel> bookauthors = _context.GetAllBookAuthors().Where(d => d.IdBook == book.Id);
            IEnumerable<BookGenreModel> bookgenres = _context.GetAllBookGenres().Where(d => d.IdBook == book.Id);
            foreach (BookAuthorModel line in bookauthors)
            {
                AuthorModel author = _context.GetAuthor(line.IdAuthor);
                au.Add(author.Name);
            }
            foreach (BookGenreModel line in bookgenres)
            {
                GenreModel genre = _context.GetGenre(line.IdGenre);
                ge.Add(genre.Name);
            }
            BookView view = new BookView()
            {
                Content = book.Content,
                Cost = book.Cost,
                Id = book.Id,
                image = book.image,
                Stored = book.Stored,
                Title = book.Title,
                Rated=book.Rated,
                Score=book.Score,
                Weight=book.Weight,
                Year = book.Year,
                Publisher = publisher.Name,
                Authors=au.ToArray(),
                Genres= ge.ToArray()
             };
            return view;
        }

        private async Task<OrderView> GetCurrentOrder()
        {
            try
            {
                User usr = await _userManager.GetUserAsync(HttpContext.User);
                if (usr != null)
                {
                    string id = usr.Id;
                    OrderModel j = _context.GetAllOrderViews().Where(p => p.UserId == id && p.Active == 1).FirstOrDefault();
                        OrderView orderView = new OrderView
                                {
                                    Id = j.Id,
                                    Active = j.Active,
                                    DateDelivery = j.DateDelivery,
                                    Amount = j.Amount,
                                    DateOrder = j.DateOrder,
                                    SumOrder = j.SumOrder,
                                    SumDelivery = j.SumDelivery,
                                    Weight=j.Weight
                                };
                    List<BookOrderView> bo = new List<BookOrderView>();
                    int sum = 0;
                    int amount = 0;
                    int weight=0;
                    CityModel city = _context.GetCity(usr.IdCity);
                    foreach (BookOrderModel o in j.BookOrders)
                    {
                        amount += o.Amount;
                        BookOrderView n = new BookOrderView
                        {
                            Id = o.Id,
                            Amount = o.Amount
                        };
                        BookView item = _context.GetAllBookViews().Where(book=>book.Id==o.IdBook).FirstOrDefault();
                        sum += item.Cost * o.Amount;
                        weight+=item.Weight * o.Amount;
                        //n.Book = GetViewByBook(item);
                        bo.Add(n);
                    }
                    orderView.Amount = amount;
                    j.Amount = amount;
                    orderView.SumOrder = sum;
                    j.SumOrder = sum;
                    j.Weight = weight;
                    if (weight > 5000)
                    {
                        weight = ((weight - 5000) / 1000)+1;//за каждый последующий килограмм
                        j.SumDelivery = city.DeliverySum + (200 * weight);
                    }
                    _context.UpdateOrder(j);
                    orderView.BookOrders = bo;
                    orderView.City = city.Name;
                    orderView.SumDelivery = j.SumDelivery;
                    orderView.DateDelivery = DateTime.Now.AddDays(city.DeliveryTime);
                    return orderView;
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

        public ActionResult GetView(string viewName, string message)
        {
            switch (viewName)
            {
                case "_BasketTable":
                    {
                        OrderView j = GetCurrentOrder().Result;
                        return PartialView("_BasketTable", j);
                    }
                case "_AdminMsg":
                    {
                        return PartialView("_AdminMsg", message);
                    }
                case "_BasketHistory":
                    {
                        User usr = GetCurrentUserAsync().Result;
                        IEnumerable<OrderModel> orders = _context.GetAllOrderViews().Where(h=>h.UserId==usr.Id && h.Active!=1);
                        List<OrderModel> ActiveOrders = _context.GetAllOrderViews().Where(h => h.UserId == usr.Id && h.Active == 1).ToList();
                        while (ActiveOrders.Count > 1)
                        { //убираем дубляжи активных заказов
                            OrderModel o = ActiveOrders.Last();
                            _context.DeleteOrder(o.Id);
                            ActiveOrders.Remove(o);
                        }
                        return PartialView("_BasketHistory", orders);
                    }
                case "_DetailsOrder":
                    {
                        OrderModel order = _context.GetOrder(Convert.ToInt32(message));
                
                        User usr = GetCurrentUserAsync().Result;
                        CityModel city = _context.GetCity(usr.IdCity);

                        OrderView model = new OrderView()
                        {
                            Active = order.Active,
                            DateDelivery = order.DateDelivery,
                            Amount = order.Amount,
                            DateOrder = order.DateOrder,
                            SumOrder = order.SumOrder,
                            Id = order.Id,
                            Weight = order.Weight,
                            City = city.Name,
                            SumDelivery = city.DeliverySum
                        };
                        IEnumerable<BookOrderModel> bo = _context.GetAllBookOrders().Where(i=>i.IdOrder==order.Id);
                        List<BookOrderView> boo = new List<BookOrderView>();
                        foreach (BookOrderModel b in bo)
                        {
                            BookOrderView o = new BookOrderView()
                            {
                                Amount = b.Amount,
                                Id = b.Id,
                                Book = GetViewByBook(b.Book)
                            };
                            boo.Add(o);
                        }
                        model.BookOrders = boo;
                        return PartialView("_DetailsOrder", model);
                    }
                default:
                    {
                        return PartialView(viewName, null);
                    }
            }
        }


        [HttpPut]
        public ActionResult Decrease(int id)
        {
            try
            {
                BookOrderModel j = _context.GetBookOrder(id);
                BookAdd b = _context.GetBook(j.IdBook);
                j.Amount--;
                if (j.Amount > 0)
                {
                    _context.UpdateBookOrder(j);
                    OrderModel o = _context.GetOrder(j.IdOrder);
                    o.SumOrder -= b.Cost;
                    o.Amount -= 1;
                    int overweight = ((o.Weight - 5000) / 1000) + 1; //на сколько перевешивало раньше
                        o.Weight -= b.Weight;
                    if (o.Weight > 5000)
                    {
                        int overweight_new = ((o.Weight - 5000) / 1000) + 1;  //на сколько сейчас
                        overweight -= overweight_new; //на сколько уменьшилось
                        o.SumDelivery -= overweight * 200;
                    }
                    _context.UpdateOrder(o);
                 
                    return Ok();
                }
                else
                {
                    _context.DeleteBookOrder(j.Id);
                    OrderModel o = _context.GetOrder(j.IdOrder);
                    o.SumOrder -= b.Cost;
                      int overweight = ((o.Weight - 5000) / 1000) + 1; //на сколько перевешивало раньше
                        o.Weight -= b.Weight;
                    if (o.Weight > 5000)
                    {
                        int overweight_new = ((o.Weight - 5000) / 1000) + 1;  //на сколько сейчас
                        overweight -= overweight_new; //на сколько уменьшилось
                        o.SumDelivery -= overweight * 200;
                    }
                    o.Amount -= 1;
                    _context.UpdateOrder(o);
                   
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        [HttpPut]
        public ActionResult Increase(int id)
        {
            try
            {
                BookOrderModel j = _context.GetBookOrder(id);
                BookAdd b = _context.GetBook(j.IdBook);
                j.Amount++;
                if (j.Amount <= b.Stored)
                {
                    
                    OrderModel o = _context.GetOrder(j.IdOrder);
                    o.SumOrder += b.Cost;
                    o.Amount += 1;
                    int overweight = ((o.Weight - 5000) / 1000) + 1; //на сколько перевешивало раньше
                    o.Weight += b.Weight;
                    if (o.Weight > 5000)
                    {
                        int overweight_new = ((o.Weight - 5000) / 1000) + 1;  //на сколько сейчас
                        overweight_new -= overweight; //на сколько увеличилось
                        o.SumDelivery += overweight_new * 200;
                    }
                    _context.UpdateOrder(o);
                    _context.UpdateBookOrder(j);
                    return Ok(false);
                }
                else
                {
                    return Ok(true);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPut]
        public ActionResult CancelOrder(int id)
        {
            try
            {
                OrderModel j = _context.GetOrder(id);
                j.Active = 4;
                j.DateCancel = DateTime.Now.Date;
                _context.UpdateOrder(j);
                IEnumerable<BookOrderModel> bo = _context.GetAllBookOrders().Where(f => f.IdOrder == id);
                foreach (BookOrderModel b in bo)
                {
                    b.Book.Stored += b.Amount;
                    _context.UpdateBook(b.Book);
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }



        [HttpPut]
        public ActionResult DeleteItem(int id)
        {
            try {
                BookOrderModel j = _context.GetBookOrder(id);
                OrderModel o = _context.GetOrder(j.IdOrder);
                BookAdd b = _context.GetBook(j.IdBook);
                o.SumOrder -= j.Amount * b.Cost;
                int overweight = ((o.Weight - 5000) / 1000) + 1; //на сколько перевешивало раньше
                    o.Weight -= b.Weight;
                if (o.Weight > 5000)
                {
                    int overweight_new = ((o.Weight - 5000) / 1000) + 1;  //на сколько сейчас
                    overweight -= overweight_new; //на сколько уменьшилось
                    o.SumDelivery -= overweight * 200;
                }
                o.Amount -= j.Amount;
                _context.DeleteBookOrder(j.Id);
                return Ok();
            } catch (Exception ex)
            {
                Log.Write(ex);
                return BadRequest(ex);
            }
        }

        [HttpPut]
        public async Task<ActionResult> MakeOrder(int id)
        {
            try
            {
                OrderModel o = _context.GetOrder(id);
                o.Active = 0;
                o.DateOrder = DateTime.Now;
                _context.UpdateOrder(o);
                IEnumerable<BookOrderModel> bo = _context.GetAllBookOrders().Where(b => b.IdOrder == o.Id);
                foreach (BookOrderModel b in bo)
                {
                    BookAdd book = _context.GetBook(b.IdBook);
                    if (book.Stored >= b.Amount) {
                        book.Stored -= b.Amount;
                        _context.UpdateBook(book);
                    } else
                    {
                        Log.WriteSuccess("PersonalController.MakeOrder", "Не хватает книг на складе! "+ book.Title+ ". Id " + book.Id);
                        return BadRequest("Не хватает книг на складе!");
                    }
                }

                User user = await _userManager.GetUserAsync(HttpContext.User);
                CityModel city = _context.GetCity(user.IdCity);
                OrderModel order = new OrderModel()
                {
                    UserId = user.Id,
                    Active = 1,
                    Amount = 0,
                    Weight=0,
                    SumDelivery=city.DeliverySum,
                    DateDelivery = DateTime.Now.AddDays(city.DeliveryTime),
                    DateOrder = DateTime.Now,
                    SumOrder=0
                };
              
                Log.WriteSuccess(" PersonalController.MakeOrder", "Id user" + order.UserId);
                _context.CreateOrder(order); //добавление заказа в БД    
                Log.WriteSuccess(" PersonalController.MakeOrder", "Создан новый заказ.");
                return NoContent();
            } catch (Exception ex)
            {
                Log.Write(ex);
                return BadRequest(ex);
            }
        }

        [HttpDelete("{id}")]
        [Route("[controller]/DeleteAll/{id}")]
        public async Task<IActionResult> DeleteAll([FromRoute] int id)
        {//удаление заказа 
            try
            {
                if (!ModelState.IsValid)
                {
                    Log.WriteSuccess(" PersonalController.Delete", "Валидация внутри контроллера неудачна.");
                    return BadRequest(ModelState);
                }
                OrderModel item = _context.GetOrder(id);
                IEnumerable<BookOrderModel> lines = _context.GetAllBookOrders().Where(l => l.IdOrder == id);
                if (item == null)
                {
                    Log.WriteSuccess(" PersonalController.Delete", "Order не найден.");
                    //return RedirectToAction("Basket", new { item ,a="Заказ не найден!" });
                    return NotFound();
                }
                foreach (BookOrderModel i in lines) { 
                    _context.DeleteBookOrder(i.Id);
                }
                User usr = GetCurrentUserAsync().Result;
                CityModel city = _context.GetCity(usr.IdCity);
                item.Amount = 0;
                item.SumOrder = 0;
                item.Weight = 0;
                item.SumDelivery = city.DeliverySum;
                _context.UpdateOrder(item);
                OrderView j = await GetCurrentOrder();
                //return RedirectToAction("Basket", new { j });
                return NoContent();
            } catch (Exception ex)
            {
                Log.Write(ex);
                return BadRequest(ex);
                //return RedirectToAction("Basket", new { b=new OrderView(), a="Ошибка"+ex });
            }
        }

        public IActionResult Basket(string message)
        {
            ViewBag.Username = GetUserName().Result;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Info()
        {
            IEnumerable<CityModel> b = _context.GetAllCities();
            ViewBag.Cities = b;
            User usr = await _userManager.GetUserAsync(HttpContext.User);
            if (usr == null)
            {
                ViewBag.Username = "Войти";
            }
            else {
                ViewBag.Username = usr.UserName;
            }
            InfoViewModel user = new InfoViewModel()
            {
                Address = usr.Address,
                Fio = usr.Fio,
                Email = usr.Email,
                IdCity = usr.IdCity,
                PhoneNumber = usr.PhoneNumber,
                UserName = usr.UserName
            };

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Info(InfoViewModel model)
        {
            IEnumerable<CityModel> d = _context.GetAllCities();
            ViewBag.Cities = d;
            ViewBag.Username = GetUserName().Result;
            try
            {
                if (ModelState.IsValid)
                {
                    User usr = await _userManager.GetUserAsync(HttpContext.User);
                    usr.Address = model.Address;
                    usr.Email = model.Email;
                    usr.Fio = model.Fio;
                    usr.IdCity = model.IdCity;
                    usr.PhoneNumber = model.PhoneNumber;
                    usr.UserName = model.UserName;
                  
                    IdentityResult i = await _userManager.UpdateAsync(usr);
                    if (i.Succeeded)
                    { return RedirectToAction("Index", "Home"); }
                    else {
                        //return View(model);
                        return View(model);
                    }
                }
                else
                {
                    return View(model);
                }
            }catch (Exception ex)
            {
                Log.Write(ex);
                return View(model);
            }
        }
    }
}