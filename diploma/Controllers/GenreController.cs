using BLL.Interfaces;
using BLL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace diploma.Controllers
{ 
    [Route("api/[controller]")]
    [ApiController]
    public class GenreController : ControllerBase
    {
        private readonly IDBCrud _context;


        public GenreController(IDBCrud context)
        {
            _context = context; // получаем контекст базы данных
        }

        [HttpGet]
        public IEnumerable<GenreModel> GetAll() //получить все заказы
        {
            try
            {//возвращаем список всех заказов для текущего пользователя
                Log.WriteSuccess("Genres.GetAll", "возвращаем список всех авторов.");
                return _context.GetAllGenres().OrderBy(b=>b.Name);
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
        public IActionResult GetGenre([FromRoute] int id)
        {
            try
            {
                //получить заказ по id заказа
                if (!ModelState.IsValid)
                {
                    Log.WriteSuccess(" GenresController.GetAuthor", "Валидация внутри контроллера неудачна.");
                    return BadRequest(ModelState);
                }
                var genre =  _context.GetGenre(id);
                if (genre == null)//если ничего не получили -- не найдено
                {
                    Log.WriteSuccess(" GenreController.GetGenre ", "ничего не получили.");
                    return NotFound();
                }
                return Ok(genre);//возвращаем автора
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                return BadRequest();
            }
        }

    }
}
