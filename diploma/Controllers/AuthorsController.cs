using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using BLL.Interfaces;
using BLL.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace diploma.Controllers
{ 
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorsController : ControllerBase
    {
        private readonly IDBCrud _context;


        public AuthorsController(IDBCrud context)
        {
            _context = context; // получаем контекст базы данных
        }
   
        [HttpGet]
        public IEnumerable<AuthorModel> GetAll() //получить все заказы
        {
          try
            {//возвращаем список всех заказов для текущего пользователя
             Log.WriteSuccess("AuthorsController.GetAll", "возвращаем список всех авторов.");
             return _context.GetAllAuthors().OrderBy(p=>p.Name);
            }
            catch (Exception ex)
            {//если что-то пошло не так, выводим исключение в консоль
                Log.Write(ex);
                return null;
            }
        }



        [HttpGet("{id}")]
        //получить автора по его id
        public IActionResult GetAuthor([FromRoute] int id)
        {
            try
            {
                //получить заказ по id заказа
                if (!ModelState.IsValid)
                {
                    Log.WriteSuccess(" AuthorsController.GetAuthor", "Валидация внутри контроллера неудачна.");
                    return BadRequest(ModelState);
                }
                var author = _context.GetAuthor(id);
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
        public IActionResult Create([FromBody] AuthorModel author)
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
                IEnumerable<AuthorModel> authors = _context.GetAllAuthors().Where(d => d.Name == author.Name);
                if (!authors.Any())
                {
                    _context.CreateAuthor(author);
                }//добавление автора в БД
                else
                {
                    Log.WriteSuccess(" AuthorsController.Create", "Попытка добавить существующего автора!");
                    return Conflict();
                }
                author = _context.GetAllAuthors().Where(d => d.Name == author.Name).FirstOrDefault();
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
        public IActionResult Update([FromRoute] int id, [FromBody] AuthorModel author)
        {//обновить существующий заказ
            try
            {
                if (!ModelState.IsValid)
                {
                    Log.WriteSuccess(" AuthorsController.Update", "Валидация внутри контроллера неудачна.");
                    return BadRequest(ModelState);
                }
                _context.UpdateAuthor(author);
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
        public IActionResult Delete([FromRoute] int id)
        {//удаление заказа
            try
            {
                if (!ModelState.IsValid)
                {
                    Log.WriteSuccess(" AuthorsController.Delete", "Валидация внутри контроллера неудачна.");
                    return BadRequest(ModelState);
                }
                _context.DeleteAuthor(id);
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
