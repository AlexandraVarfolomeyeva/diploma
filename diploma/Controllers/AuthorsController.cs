using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BookShop.Controllers
{ 
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorsController : ControllerBase
    {
        // GET: /<controller>/
        private readonly BookingContext _context;

        public AuthorsController(BookingContext context)
        {
            _context = context; // получаем контекст базы данных
        }
   
        [HttpGet]
        public IEnumerable<Author> GetAll() //получить все заказы
        {
          try
            {//возвращаем список всех заказов для текущего пользователя
             Log.WriteSuccess("AuthorsController.GetAll", "возвращаем список всех авторов.");
             return _context.Author.Include(p => p.BookAuthors);
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
        public async Task<IActionResult> GetAuthor([FromRoute] int id)
        {
            try
            {
                //получить заказ по id заказа
                if (!ModelState.IsValid)
                {
                    Log.WriteSuccess(" AuthorsController.GetAuthor", "Валидация внутри контроллера неудачна.");
                    return BadRequest(ModelState);
                }
                var author = await _context.Author.SingleOrDefaultAsync(m => m.Id == id);
                if (author == null)//если ничего не получили -- не найдено
                {
                    Log.WriteSuccess(" AuthorsController.GetAuthor ", "ничего не получили.");
                    return NotFound();
                }
                return Ok(author);//возвращаем автора
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                return BadRequest();
            }
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create([FromBody] Author author)
        {//добавить нового автора
         //получаем данные о заказе во входных параметрах
            try
            {
                if (!ModelState.IsValid)
                {
                    Log.WriteSuccess(" AuthorsController.Create", "Валидация внутри контроллера неудачна.");
                    return BadRequest(ModelState);
                }
                Log.WriteSuccess(" AuthorsController.Create", "Данные валидны.");
                IEnumerable<Author> authors = _context.Author.Include(p => p.BookAuthors).Where(d => d.Name == author.Name);
                if (!authors.Any()) { _context.Author.Add(author); } //добавление автора в БД
                else {
                    Log.WriteSuccess(" AuthorsController.Create", "Попытка добавить существующего автора!");
                    return Conflict();
                }
                await _context.SaveChangesAsync();//асинхронное сохранение изменений
                Log.WriteSuccess(" AuthorsController.Create", "добавление автора " + author.Id + " в БД");
                return CreatedAtAction("GetAuthor", new { id = author.Id }, author);
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                return BadRequest();
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] Author author)
        {//обновить существующий заказ
            try
            {
                if (!ModelState.IsValid)
                {
                    Log.WriteSuccess(" AuthorsController.Update", "Валидация внутри контроллера неудачна.");
                    return BadRequest(ModelState);
                }
                var item = _context.Author.Find(id);
                if (item == null)
                {
                    Log.WriteSuccess(" AuthorsController.Update", "Элемент для обновления не найден в БД.");
                    return NotFound();
                }
                item.Name = author.Name;
                _context.Author.Update(item);
                await _context.SaveChangesAsync();
                Log.WriteSuccess(" AuthorsController.Update", "обновление автора " + author.Id + " в БД.");
                return NoContent();
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                return BadRequest();
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {//удаление заказа
            try
            {
                if (!ModelState.IsValid)
                {
                    Log.WriteSuccess(" AuthorsController.Delete", "Валидация внутри контроллера неудачна.");
                    return BadRequest(ModelState);
                }
                var item = _context.Author.Find(id);
                if (item == null)
                {
                    Log.WriteSuccess(" AuthorsController.Delete", "Элемент для удаления не найден в БД.");
                    return NotFound();
                }
                _context.Author.Remove(item);
                await _context.SaveChangesAsync();
                Log.WriteSuccess(" AuthorsController.Delete", "удаление автора " + id + " в БД.");
                return NoContent();
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                return BadRequest();
            }
        }
}
}
