using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BLL.Interfaces;
using BLL.Models;
using DAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace diploma.Controllers
{
   [Produces("application/json")]
    public class AccountController : Controller
    {
            private readonly UserManager<User> _userManager;
            private readonly SignInManager<User> _signInManager;
        private readonly IDBCrud _context;

        public AccountController(UserManager<User> userManager,
            SignInManager<User> signInManager, IDBCrud context)
            {

                _context = context; // получаем контекст базы данных
                _userManager = userManager;
                _signInManager = signInManager;
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

        public IActionResult LoginForm(string message, IEnumerable<string> error)
        {
            if (message != "" && message != null)
            {
                ViewBag.Message = message;
                ViewBag.Error = error;
            }
            else
            {
                ViewBag.Message = "";
                ViewBag.Error = "";
            }
            ViewBag.Username = GetUserName().Result;
            return View();
        }
        [HttpGet]
        public ActionResult Register()
        {
            IEnumerable<CityModel> b = _context.GetAllCities().OrderBy(d=>d.Name);
            ViewBag.Cities = b;
            ViewBag.Username = GetUserName().Result;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Route("api/Account/Register")][ FromBody ]
        public async Task<IActionResult> Register(
            RegisterViewModel model)
            {
            IEnumerable<CityModel> d = _context.GetAllCities();
            ViewBag.Cities = d;
            ViewBag.Username = GetUserName().Result;
            try
                {
                    if (ModelState.IsValid)
                    {//добавление нового пользователя при регистрации
                        User user = new User
                        {
                            Fio = model.Fio,
                            Email = model.Email,
                            UserName = model.UserName,
                            PhoneNumber = model.PhoneNumber,
                            PhoneNumberConfirmed = true
                        };
                   
                        // Добавление нового пользователя
                        var result = await _userManager.CreateAsync(user,
                        model.Password);
                        await _signInManager.SignOutAsync();
                        await _signInManager.SignInAsync(user, false);// установка куки
                        if (result.Succeeded)//если успешно
                        {
                            Log.WriteSuccess("AccountController.Register", "Пользователь добавлен и вошел в систему.");
                            IdentityResult x = await _userManager.AddToRoleAsync(user, "user");//роль - пользователь

                            var msg = new
                            {
                                message = "Добавлен новый пользователь: " + user.UserName
                            };
                        AddressModel address = new AddressModel()
                        {
                            IdCity = model.IdCity,
                            IdUser = user.Id,
                            Name = model.Address,
                            isDeleted=false
                        };
                        CityModel b = d.Where(c => c.Id == model.IdCity).First();
                        OrderModel order = new OrderModel()
                            {
                                UserId = user.Id,
                                Amount = 0,
                                Active = 1,
                                SumDelivery = 0,
                                SumOrder = 0,
                                DateDelivery = DateTime.Now.AddDays(b.DeliveryTime),
                                DateOrder = DateTime.Now,
                                Weight=0
                            };
                            _context.CreateAddress(address);
                            _context.CreateOrder(order); //добавление заказа в БД
                            Log.WriteSuccess(" OrdersController.Create", "добавление заказа в БД");
                        //return Ok(msg);
                        return RedirectToAction("Index", "Home");
                    }
                        else
                        {//вывод ошибок при неудаче
                            foreach (var error in result.Errors)
                            {
                                ModelState.AddModelError(string.Empty,
                                error.Description);
                            }
                            Log.WriteSuccess("AccountController.Register", "Пользователь не добавлен.");
                            var errorMsg = new
                            {
                                message = "Пользователь не добавлен.",
                                error = ModelState.Values.SelectMany(e =>
                                e.Errors.Select(er => er.ErrorMessage))
                            };
                        return View(model);
                    }
                    }
                    else
                    {//если неверно введены данные
                        Log.WriteSuccess("AccountController.Register", "Неверные входные данные.");
                        var errorMsg = new
                        {
                            message = "Неверные входные данные.",
                            error = ModelState.Values.SelectMany(e =>
                            e.Errors.Select(er => er.ErrorMessage))
                        };
                    return View(model);
                }
                }
                catch (Exception ex)
                {
                    Log.Write(ex);
                    var errorMsg = new
                    {
                        message = "Неверные входные данные.",
                        error = ModelState.Values.SelectMany(e =>
                        e.Errors.Select(er => er.ErrorMessage))
                    };
                 return View(model);
            }
            }



        [HttpGet]
        public ActionResult Concern()
        {
            return PartialView("_Concern");
        }

        [HttpGet]
        public ActionResult ErrorMsg()
        {
            return PartialView();
        }

            [HttpPost]
            [ValidateAntiForgeryToken]
            ////[Route("api/Account/Login")]
            //[FromBody] 
            public async Task<IActionResult> LogIn(LoginViewModel model)
            {//вход в систему
                if (ModelState.IsValid)
                {
                    var result =
                    await _signInManager.PasswordSignInAsync(model.User, model.Password,
                    model.RememberMe, false);
                    if (result.Succeeded)//если успешно
                    {
                        Log.WriteSuccess("AccountController.Login", "Выполнен вход пользователем: " + model.User);
                        var msg = new
                        {
                            message = "Выполнен вход пользователем: " +
                        model.User
                        };
                       return RedirectToAction("Index","Home");
                    }
                    else
                    {//если неудачно
                        ModelState.AddModelError("", "Неправильный логин и (или) пароль");
                        Log.WriteSuccess("AccountController.Login", "Неправильный логин и (или) пароль.");
                    MessageError msg = new MessageError()
                    {
                            message = "Вход не выполнен.",
                            error = ModelState.Values.SelectMany(e =>
                            e.Errors.Select(er => er.ErrorMessage))
                        };
                   return RedirectToAction("LoginForm", "Account", new { msg.message, msg.error });
                }
                }
                else
                {
                    Log.WriteSuccess("AccountController.Login", "Вход не выполнен.");
                MessageError msg = new MessageError()
                {
                        message = "Вход не выполнен.",
                        error = ModelState.Values.SelectMany(e =>
                        e.Errors.Select(er => er.ErrorMessage))
                    };
                return RedirectToAction("LoginForm", "Account", new { msg.message, msg.error });
            }
            }


        [HttpPost]
        //[Route("/api/Account/LogOff")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOff()
            {//выход из системы
             // Удаление куки
                await _signInManager.SignOutAsync();
                Log.WriteSuccess("AccountController.LogOff", "Выполнен выход.");
                var msg = new
                {
                    message = "Выполнен выход."
                };
                return Ok(msg);
            }


            public string id = "";
            public string role;
            public IList<string> x;
            [HttpPost]
            [Route("api/Account/isAuthenticated")]
            //[ValidateAntiForgeryToken]
            public async Task<IActionResult> LogisAuthenticatedOff()
            {//сообщение об авторизации пользователем
                User usr;
                usr = await GetCurrentUserAsync(); //получение текущего пользователя
                if (usr != null) id = usr.Id;
                var message = usr == null ? "" : usr.UserName;
                var msg = new
                {
                    message
                };
                return Ok(msg);
            }
  
            [HttpGet]
            [Route("api/Account/CurrentUserInfo")]
            public async Task<IActionResult> GetCurrentUserInfo()
            {//получение id текущего пользователя
                try
                {
                    User usr = await _userManager.GetUserAsync(HttpContext.User);
                    if (usr == null)//если ничего не получили -- не найдено
                    {
                        Log.WriteSuccess(" AccountController.CurrentUserInfo ", "ничего не получили.");
                        return NotFound();
                    }
                    return Ok(usr);
                }
                catch (Exception ex)
                {
                    Log.Write(ex);
                    return BadRequest();
                }
            }

            [HttpGet]
            [Route("api/Account/GetRole")]
            public async Task<string> GetUserRole()
            {//получение id текущего пользователя

                try
                {
                    User usr = await GetCurrentUserAsync();
                    if (usr != null)
                    {
                        x = await _userManager.GetRolesAsync(usr);
                        role = x.FirstOrDefault();
                    }
                }
                catch (Exception ex)
                {
                    Log.Write(ex);
                }
                return role;
            }
        }

    }
