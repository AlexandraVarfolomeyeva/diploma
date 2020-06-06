using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BLL.Interfaces;
using BLL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace diploma.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class BookOrderController : Controller
    {//Base
        private readonly IDBCrud _context;
        public BookOrderController(IDBCrud context)
        {
            _context = context;
            
        }

        [HttpGet]
        public IEnumerable<BookOrderModel> GetAll()
        {//получение всех строк заказа
            try {  return _context.GetAllBookOrders();
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                return null;
            }
           
        }

        [HttpGet("{id}")]
        public IActionResult GetBookOrder([FromRoute] int id)
        {//получение конкретной строки заказа по id
            try
            {
                if (!ModelState.IsValid)
                {
                    Log.WriteSuccess(" BookOrderController.GetBookOrder", "Валидация внутри контроллера неудачна.");
                    return BadRequest(ModelState);
                }

                var item = _context.GetBookOrder(id);

                if (item == null)
                {
                    Log.WriteSuccess("BookOrderController.GetBookOrder", "Элемент BookOrder не найден.");
                    return NotFound();
                }
                return Ok(item);
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                return BadRequest(ModelState);
            }
        }


        [HttpPost]
        [Authorize(Roles = "user")]
        public IActionResult Create([FromBody] BookOrderForm item)
        {//создание новой строки заказа
            try //0
            {
                if (!ModelState.IsValid)//1
                {
                    Log.WriteSuccess("BookOrderController.Create", "Валидация внутри контроллера неудачна.");//2
                    return BadRequest(ModelState);
                }
            
                BookOrderModel book = _context.GetAllBookOrders().Where(a => a.IdBook == item.IdBook && a.IdOrder == item.IdOrder).FirstOrDefault();//3
                if (book != null)//4
                {
                    book.Amount++;//5
                    _context.UpdateBookOrder(book);
                } else { 
                BookOrderModel bookorder = new BookOrderModel()//6
                    {
                        IdBook = item.IdBook,
                        IdOrder = item.IdOrder,
                        Amount = 1
                    };
                    _context.CreateBookOrder(bookorder);
                }
                BookAdd b = _context.GetBook(item.IdBook);//7
                OrderModel order = _context.GetOrder(item.IdOrder);
                order.SumOrder += b.Cost;
                int overweight = ((order.Weight - 5000) / 1000) + 1;
                order.Weight += b.Weight;
                if (order.Weight > 5000)//8
                {
                    int overweight_new = ((order.Weight - 5000) / 1000) + 1;//9
                    overweight_new -= overweight;
                    order.SumDelivery += overweight_new * 200;
                }
                order.Amount++;//10
                _context.UpdateOrder(order);
                Log.WriteSuccess("BookOrderController.Create", "Добавлена новая строка заказа.");
                return Ok();
            }
            catch (Exception ex)//11
            {
                Log.Write(ex);//12
                return BadRequest(ex);
            }
        }//13

        [HttpDelete("{id}")]
        [Authorize(Roles = "user")]
        public IActionResult Delete([FromRoute] int id)
        {//удаление существующей строки заказа
            try
            {
                if (!ModelState.IsValid)
                {
                    Log.WriteSuccess("BookOrderController.Delete", "Валидация внутри контроллера неудачна.");
                    return BadRequest(ModelState);
                }
                _context.DeleteBookOrder(id);
                Log.WriteSuccess("BookOrderController.Delete", "Элемент удален.");
                return NoContent();
            } catch (Exception ex)
            {
                Log.Write(ex);
                return BadRequest(ModelState);
            }
        }
    }
}
