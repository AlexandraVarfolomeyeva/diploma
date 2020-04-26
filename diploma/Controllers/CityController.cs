using BookShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CityController : ControllerBase
    {
        // GET: /<controller>/
        private readonly BookingContext _context;

        public CityController(BookingContext context)
        {
            _context = context; // получаем контекст базы данных
        }

        [HttpGet]
        public IEnumerable<City> GetAll() //получить все заказы
        {
            try
            {//возвращаем список всех заказов для текущего пользователя
                Log.WriteSuccess("City.GetAll", "возвращаем список всех авторов.");
                IEnumerable<City> b = _context.City;
                return b;
            }
            catch (Exception ex)
            {//если что-то пошло не так, выводим исключение в консоль
                Console.WriteLine("Возникла ошибка при получении списка всех авторов.");
                Log.Write(ex);
                return null;
            }
        }

        [HttpGet("{id}")]
        //получить автора по его id
        public async Task<IActionResult> GetCity([FromRoute] int id)
        {
            try
            {
                //получить заказ по id заказа
                if (!ModelState.IsValid)
                {
                    Log.WriteSuccess(" GenresController.GetCity", "Валидация внутри контроллера неудачна.");
                    return BadRequest(ModelState);
                }
                var city = await _context.City.SingleOrDefaultAsync(m => m.Id == id);
                if (city == null)//если ничего не получили -- не найдено
                {
                    Log.WriteSuccess(" CityController.GetCity ", "ничего не получили.");
                    return NotFound();
                }
                return Ok(city);//возвращаем автора
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                return BadRequest();
            }
        }
    }
}
