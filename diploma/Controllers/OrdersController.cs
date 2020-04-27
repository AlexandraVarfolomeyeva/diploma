using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using diploma.BLL;
using diploma.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace diploma.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly BookingContext _context;
        private readonly UserManager<User> _userManager;

        public OrdersController(UserManager<User> userManager, BookingContext context)
        {
            _userManager = userManager;
            _context = context; // получаем контекст базы данных
        }
      
        private Task<User> GetCurrentUserAsync() =>
_userManager.GetUserAsync(HttpContext.User);

        [HttpGet]
       public async Task<IEnumerable<Order>> GetAllAsync() //получить все заказы
        {
            User usr = await _userManager.GetUserAsync(HttpContext.User);
            //  string id = IDEvent().Result; //получаем id текущего пользователя из AccountController
            string id = usr.Id;
            try
            {//возвращаем список всех заказов для текущего пользователя
                if (id !="")
                Log.WriteSuccess(" OrdersController.GetAll", "возвращаем список всех заказов для текущего пользователя.");
                else
                    Log.WriteSuccess(" OrdersController.GetAll", "Пользователь не определен.");
                return _context.Order.Include(p => p.BookOrders).Where(p => p.UserId == id);
            
            } catch (Exception ex)
            {//если что-то пошло не так, выводим исключение в консоль
                Console.WriteLine("Возникла ошибка при получении списка всех заказов.");
                Log.Write(ex);
                return null;
            }
        }
             
        

        [HttpGet("{id}")]
        //получить заказ по его id
        public async Task<IActionResult> GetOrder([FromRoute] int id)
        {
            try
            {
                //получить заказ по id заказа
                if (!ModelState.IsValid)
                {
                    Log.WriteSuccess(" OrdersController.GetOrder", "Валидация внутри контроллера неудачна.");
                    return BadRequest(ModelState);
                }
                var order = await _context.Order.SingleOrDefaultAsync(m => m.Id == id);
                if (order == null)//если ничего не получили -- не найдено
                {
                    Log.WriteSuccess(" OrdersController.GetOrder", "ничего не получили.");
                    return NotFound();
                }
                return Ok(order);//возвращием заказ
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Order order)
        {//создать новый заказ
         //получаем данные о заказе во входных параметрах
            try
            {
                if (!ModelState.IsValid)
                {
                    Log.WriteSuccess(" OrdersController.Create", "Валидация внутри контроллера неудачна.");
                    return BadRequest(ModelState);
                }
                User user = await _userManager.GetUserAsync(HttpContext.User);
                Log.WriteSuccess(" OrdersController.Create", "Данные валидны.");
                Log.WriteSuccess(" OrdersController.Create", "Id user"+ order.UserId);
                order.DateDelivery = DateTime.Now;
                order.DateDelivery = DateTime.Now.AddDays(user.City.DeliveryTime);
                order.Active = 1;
                order.Amount = 0;
                order.SumOrder = 0;
                _context.Order.Add(order); //добавление заказа в БД
                await _context.SaveChangesAsync();//асинхронное сохранение изменений
                Log.WriteSuccess(" OrdersController.Create", "добавление заказа "+ order.Id + " в БД");
                return CreatedAtAction("GetOrder", new { id = order.Id }, order);
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                return BadRequest();
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] Order order)
        {//обновить существующий заказ
            try {
               if (!ModelState.IsValid)
            {
                    Log.WriteSuccess(" OrdersController.Update", "Валидация внутри контроллера неудачна.");
                    return BadRequest(ModelState);
            }
            var item = _context.Order.Find(id);
            if (item == null)
            {
                    Log.WriteSuccess(" OrdersController.Update", "Элемент для обновления не найден в БД.");
                    return NotFound();
            }
            item.BookOrders = order.BookOrders;
            item.DateDelivery = order.DateDelivery;
            item.DateOrder = order.DateOrder;
//item.SumDelivery = order.SumDelivery;
            item.SumOrder = order.SumOrder;
                item.Active = order.Active;
            _context.Order.Update(item);
            await _context.SaveChangesAsync();
             Log.WriteSuccess(" OrdersController.Update", "обновление заказа " + order.Id + " в БД.");
             //   GetDiscountAsync();
             return NoContent();
        }
            catch (Exception ex)
            {
                Log.Write(ex);
                return BadRequest();
            }
}

        [HttpDelete("{id}")]
     
        [Authorize(Roles = "user")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {//удаление заказа
            try
            {
                if (!ModelState.IsValid)
                {
                    Log.WriteSuccess(" OrdersController.Delete", "Валидация внутри контроллера неудачна.");
                    return BadRequest(ModelState);
                }
                var item = _context.Order.Find(id);
                if (item == null)
                {
                    Log.WriteSuccess(" OrdersController.Delete", "Элемент для удаления не найден в БД.");
                    return NotFound();
                }
                _context.Order.Remove(item);
                await _context.SaveChangesAsync();
                Log.WriteSuccess(" OrdersController.Delete", "удаление заказа " + id + " в БД.");
                return NoContent();
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("api/Orders/GetDiscount")]
        public async Task<IEnumerable<Order>> GetDiscountAsync()
        {
            BuyService b = new BuyService(10);
            IEnumerable<Order> g = await GetAllAsync();
            return b.GetDiscount(g, 0);
        }
    }
}
