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


        #region helpers

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

        private IEnumerable<AddressView> GetAddresses()
        {
            User usr = _userManager.GetUserAsync(HttpContext.User).Result;
            IEnumerable<AddressModel> addresses = _context.GetAllAddresses().Where(p => p.IdUser == usr.Id);
            List<AddressView> addr = new List<AddressView>();
            foreach (AddressModel address in addresses)
            {
                CityModel c = _context.GetCity(address.IdCity);
                AddressView add = new AddressView()
                {
                    Address = address.Name,
                    Id = address.Id,
                    DeliveryTime = c.DeliveryTime,
                    DeliverySum = c.DeliverySum,
                    City = c.Name,
                    IdCity = address.IdCity,
                    IdUser = address.IdUser
                };
                addr.Add(add);
            }
            return addr;
        }

        private async Task<OrderView> GetCurrentOrder()
        {
            try
            {
                User usr = await _userManager.GetUserAsync(HttpContext.User);
                if (usr != null)
                {
                    string id = usr.Id;
                    OrderModel j = _context.GetAllOrders().Where(p => p.UserId == id && p.Active == 1).FirstOrDefault();
                  
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
                    IEnumerable<BookOrderModel> bookOrders = _context.GetAllBookOrders().Where(b=>b.IdOrder==j.Id);
                    List<BookOrderView> bo = new List<BookOrderView>();
                    float sum = 0;
                    int amount = 0;
                    int weight=0;
                    foreach (BookOrderModel o in bookOrders)
                    {
                        amount += o.Amount;
                        BookOrderView n = new BookOrderView
                        {
                            Id = o.Id,
                            Amount = o.Amount,
                            Price = o.Price
                        };
                        BookView item = _context.GetAllBookViews().Where(book=>book.Id==o.IdBook).FirstOrDefault();
                        sum += o.Price * o.Amount;
                        weight+=item.Weight * o.Amount;
                        n.Book = item;
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
                        j.SumDelivery = (200 * weight);
                    }
                    _context.UpdateOrder(j);
                    orderView.BookOrders = bo;
                    orderView.SumDelivery = j.SumDelivery;
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

        #endregion

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
                        IEnumerable<OrderModel> orders = _context.GetAllOrders().Where(h=>h.UserId==usr.Id && h.Active!=1);
                        List<OrderModel> ActiveOrders = _context.GetAllOrders().Where(h => h.UserId == usr.Id && h.Active == 1).ToList();
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
                        AddressModel address = _context.GetAddress(order.AddressId);
                        CityModel city =  _context.GetCity(address.IdCity);
                        AddressView addr = new AddressView()
                        {
                            Id = address.Id,
                            IdUser = address.IdUser,
                            Address = address.Name,
                            IdCity = address.IdCity,
                            City = city.Name,
                            DeliverySum = city.DeliverySum,
                            DeliveryTime = city.DeliveryTime
                        };
                        OrderView model = new OrderView()
                        {
                            Active = order.Active,
                            DateDelivery = order.DateDelivery,
                            Amount = order.Amount,
                            DateOrder = order.DateOrder,
                            SumOrder = order.SumOrder,
                            Id = order.Id,
                            Weight = order.Weight,
                            SumDelivery = order.SumDelivery,
                            Address = addr
                        };

                        IEnumerable<BookOrderModel> bo = _context.GetAllBookOrders().Where(i=>i.IdOrder==order.Id);
                        List<BookOrderView> boo = new List<BookOrderView>();
                        foreach (BookOrderModel b in bo)
                        {
                            BookModel book = _context.GetBookModel(b.IdBook);
                            BookOrderView o = new BookOrderView()
                            {
                                Amount = b.Amount,
                                Price = b.Price,
                                Id = b.Id,
                                Book = GetViewByBook(book)
                            };
                            boo.Add(o);
                        }
                        model.BookOrders = boo;
                        return PartialView("_DetailsOrder", model);
                    }
                case "_AddressesTable":
                    {
                        User usr = GetCurrentUserAsync().Result;
                       
                        IEnumerable<AddressModel> addresses = _context.GetAllAddresses().Where(h => h.IdUser == usr.Id);

                        OrderModel order = _context.GetAllOrders().Where(d => d.UserId == usr.Id && d.Active == 1).First();
                        if (order != null)
                        {
                            order.AddressId = addresses.First().Id;
                            _context.UpdateOrder(order);
                        }
                        List<AddressView> address = new List<AddressView>();
                        foreach (AddressModel a in addresses)
                        {

                            CityModel city = _context.GetCity(a.IdCity);
                            AddressView add = new AddressView()
                            {
                                City=city.Name,
                                DeliverySum=city.DeliverySum,
                                DeliveryTime=city.DeliveryTime,
                                Address=a.Name,
                                Id=a.Id,
                                IdCity=a.IdCity,
                                IdUser=a.IdUser
                            };
                            address.Add(add);
                        }
                        return PartialView("_AddressesTable", address);
                    }
                case "_EditAddress":
                    {
                        User usr = GetCurrentUserAsync().Result;
                        IEnumerable<AddressModel> addresses = _context.GetAllAddresses().Where(h => h.IdUser == usr.Id);
                        List<AddressView> address = new List<AddressView>();
                        foreach (AddressModel a in addresses)
                        {

                            CityModel city = _context.GetCity(a.IdCity);
                            AddressView add = new AddressView()
                            {
                                City = city.Name,
                                DeliverySum = city.DeliverySum,
                                DeliveryTime = city.DeliveryTime,
                                Address = a.Name,
                                Id = a.Id,
                                IdCity = a.IdCity,
                                IdUser = a.IdUser
                            };
                            address.Add(add);
                        }
                        return PartialView("_EditAddress", address);
                    }
                default:
                    {
                        return PartialView(viewName, null);
                    }
            }
        }
        
        #region basket
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
                    o.SumOrder -= j.Price;
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
                    o.SumOrder -= j.Price;
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
                    o.SumOrder += j.Price;
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
                    b.Book = _context.GetBookModel(b.IdBook);
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
        public IActionResult ChooseAddress(int id)
        {
            try
            {
                AddressModel address = _context.GetAddress(id);
                if (address != null) {
                    OrderModel order = _context.GetAllOrders().Where(a => a.UserId == address.IdUser && a.Active == 1).First();
                    if (order != null)
                    {
                        order.AddressId = id;
                        _context.UpdateOrder(order);
                        return Ok();
                    }
                    else
                    {
                        Log.WriteSuccess("/Personal/ChooseAddress/[put] ", "Заказ не найден!");
                        return Conflict();
                    }
                }
                else
                {
                        Log.WriteSuccess("/Personal/ChooseAddress/[put] ", "Адрес не найден!");
                        return Conflict();
                }
            }
            catch (Exception ex)
            {
                Log.Write(ex);
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
                o.SumOrder -= j.Amount * j.Price;
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
        public ActionResult MakeOrder(int id)
        {
            try//0
            {
                OrderModel o = _context.GetOrder(id);//1
                IEnumerable<BookOrderModel> bo = _context.GetAllBookOrders().Where(b => b.IdOrder == o.Id);
                foreach (BookOrderModel b in bo)//2
                {
                    BookAdd book = _context.GetBook(b.IdBook);//3
                    if (book.Stored >= b.Amount) {//4
                        book.Stored -= b.Amount;//5
                        _context.UpdateBook(book);
                    } else
                    {//6
                        Log.WriteSuccess("PersonalController.MakeOrder", "Не хватает книг на складе! "+ book.Title+ ". Id " + book.Id);
                        return BadRequest("Не хватает книг на складе!");//7
                    }//8
                }

                User user = _userManager.GetUserAsync(HttpContext.User).Result;//9
                AddressModel ad = _context.GetAddress(o.AddressId);
                CityModel city = _context.GetCity(ad.IdCity);
                o.Active = 0;
                o.SumDelivery += city.DeliverySum;
                o.DateOrder = DateTime.Now;
                _context.UpdateOrder(o);
                OrderModel order = new OrderModel()
                {
                    UserId = user.Id,
                    Active = 1,
                    Amount = 0,
                    Weight=0,
                    SumDelivery=0,
                    DateDelivery = DateTime.Now.AddDays(city.DeliveryTime),
                    DateOrder = DateTime.Now,
                    SumOrder=0
                };
                Log.WriteSuccess(" PersonalController.MakeOrder", "Id user" + order.UserId);
                _context.CreateOrder(order); //добавление заказа в БД    
                Log.WriteSuccess(" PersonalController.MakeOrder", "Создан новый заказ.");
                return NoContent();//10
            } catch (Exception ex)//11
            {
                Log.Write(ex);//12
                return BadRequest(ex);//13
            }//14
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
               item.Amount = 0;
                item.SumOrder = 0;
                item.Weight = 0;
                item.SumDelivery = 0;
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

        #endregion

        #region private information about user

        [HttpGet]
        public IEnumerable<CityModel> GetCities()
        {
            try
            {
                return _context.GetAllCities();
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                return null;
            }
        }

        [HttpPost]
        public IActionResult AddAddress(AddressModel address)
        {
            try
            {
                address.IdUser = _userManager.GetUserAsync(HttpContext.User).Result.Id;
                address.isDeleted = false;
                if (ModelState.IsValid)
                {
                    _context.CreateAddress(address);
                    return CreatedAtAction("GetAuthor", new { id = address.Id }, address);
                }
                else
                {
                    Log.WriteSuccess("/Admin/AddAddress/[Post] ", "Модель не валидна!");
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
        public AddressModel GetAddress(int id)
        {
            try {
                return _context.GetAddress(id);
            } catch (Exception ex)
            {
                Log.Write(ex);
                return null;
            }
        }

        [HttpPut]
        public IActionResult EditAddress(int id,AddressModel address)
        {
            try
            {
                address.IdUser = _userManager.GetUserAsync(HttpContext.User).Result.Id;
                if (ModelState.IsValid)
                {
                    address.Id = id;
                    _context.UpdateAddress(address);
                    return NoContent();
                }
                else
                {
                    Log.WriteSuccess("/Personal/EditAddress/[put] ", "Модель не валидна!");
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
        public IActionResult DeleteAddress(int id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string usrId = _userManager.GetUserAsync(HttpContext.User).Result.Id;
                    if (_context.GetAllAddresses().Where(d => d.IdUser == usrId && d.isDeleted == false).Count() > 1)
                    {
                        _context.DeleteAddress(id);
                        return Ok();
                    } else
                    {
                        return NoContent();
                    }
                }
                else
                {
                    Log.WriteSuccess("/Personal/DeleteAddress/[delete] ", "Модель не валидна!");
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
                Fio = usr.Fio,
                Email = usr.Email,
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
                    usr.Email = model.Email;
                    usr.Fio = model.Fio;
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
        
        public async Task<IActionResult> ChangePassword()
        {
            User user = await _userManager.GetUserAsync(HttpContext.User);
            ViewBag.Username = user.UserName;
            if (user == null)
            {
                return NotFound();
            }
            ChangePasswordViewModel model = new ChangePasswordViewModel { Id = user.Id, UserName = user.UserName };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _userManager.GetUserAsync(HttpContext.User);
                if (user != null)
                {
                    IdentityResult result =
                        await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Info");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Пользователь не найден");
                }
            }
            return View(model);
        }
        #endregion


    }




}