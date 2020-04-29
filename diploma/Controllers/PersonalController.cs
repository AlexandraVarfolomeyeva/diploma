using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using diploma.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace diploma.Controllers
{
    public class PersonalController : Controller
    {
        private readonly BookingContext _context;
        private readonly UserManager<User> _userManager;

        public PersonalController(BookingContext context,
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

        private async Task<OrderView> GetCurrentOrder()
        {
            try
            {
                User usr = await _userManager.GetUserAsync(HttpContext.User);
                if (usr != null)
                {
                    string id = usr.Id;
                    Order j = _context.Order.Include(p => p.BookOrders).Where(p => p.UserId == id && p.Active == 1).FirstOrDefault();
                        OrderView orderView = new OrderView
                                {
                                    Id = j.Id,
                                    Active = j.Active,
                                    DateDelivery = j.DateDelivery,
                                    Amount = j.Amount,
                                    DateOrder = j.DateOrder,
                                    SumOrder = j.SumOrder
                                };
                    List<BookOrderView> bo = new List<BookOrderView>();
                    int sum = 0;
                    int amount = 0;
                    foreach (BookOrder o in j.BookOrders)
                    {
                        amount += o.Amount;
                        BookOrderView n = new BookOrderView
                        {
                            Id = o.Id,
                            Amount = o.Amount
                        };
                        Book item = _context.Book.Where(book => book.isDeleted == false && book.Id==o.IdBook).FirstOrDefault();

                        BookView b = new BookView()
                        {
                            Id = item.Id,
                            Content = item.Content,
                            Cost = item.Cost,
                            image = item.image,
                            Stored = item.Stored,
                            Title = item.Title,
                            Year = item.Year
                        };
                        sum += item.Cost*o.Amount;
                        Publisher publisher = _context.Publisher.Find(item.IdPublisher);
                        List<string> au = new List<string>();
                        List<string> ge = new List<string>();
                        b.Publisher = publisher.Name;
                        IEnumerable<BookAuthor> bookauthors = _context.BookAuthor.Where(d => d.IdBook == item.Id);
                        IEnumerable<BookGenre> bookgenres = _context.BookGenre.Where(d => d.IdBook == item.Id);
                        foreach (BookAuthor line in bookauthors)
                        {
                            Author author = _context.Author.Find(line.IdAuthor);
                            au.Add(author.Name);
                        }
                        b.Authors = au.ToArray();
                        foreach (BookGenre line in bookgenres)
                        {
                            Genre genre = _context.Genre.Find(line.IdGenre);
                            ge.Add(genre.Name);
                        }
                        b.Genres = ge.ToArray();
                        n.Book = b;
                        bo.Add(n);
                    }
                    orderView.Amount = amount;
                    j.Amount = amount;
                    orderView.SumOrder = sum;
                    j.SumOrder = sum;
                    _context.Order.Update(j);
                    await _context.SaveChangesAsync();
                    orderView.BookOrders = bo;
                   City city = _context.City.Where(a => a.Id == usr.IdCity).FirstOrDefault();
                    orderView.City = city.Name;
                    orderView.SumDelivery = city.DeliverySum;
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

        public ActionResult GetView()
        {
            OrderView j = GetCurrentOrder().Result;
            return PartialView("BasketTable", j);
        }



        [HttpDelete("{id}")]
        [Route("[controller]/DeleteAll/{id}")]
        public async Task<IActionResult> DeleteAll([FromRoute] int id)
        {//удаление книги из БД возможно только администратором
            try
            {
                if (!ModelState.IsValid)
                {
                    Log.WriteSuccess(" PersonalController.Delete", "Валидация внутри контроллера неудачна.");
                    return BadRequest(ModelState);
                }
                Order item = _context.Order.Find(id);
                IEnumerable<BookOrder> lines = _context.BookOrder.Where(l => l.IdOrder == id);
                if (item == null)
                {
                    Log.WriteSuccess(" PersonalController.Delete", "Order не найден.");
                    //return RedirectToAction("Basket", new { item ,a="Заказ не найден!" });
                    return NotFound();
                }
                foreach (BookOrder i in lines) { 
                    _context.BookOrder.Remove(i);
                }
                item.Amount = 0;
                item.SumOrder = 0;
              _context.Order.Update(item);
                await _context.SaveChangesAsync();
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
            IEnumerable<City> b = _context.City;
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
            IEnumerable<City> d = _context.City;
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
                return View(model);
            }
        }
    }
}