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
    public class PublisherController : ControllerBase
    {
        private readonly IDBCrud _context;


        public PublisherController(IDBCrud context)
        {
            _context = context; // получаем контекст базы данных
        }

        [HttpGet]
        public IEnumerable<PublisherModel> GetAll() //получить все заказы
        {
            try
            {//возвращаем список всех заказов для текущего пользователя
                Log.WriteSuccess("PublisherController.GetAll", "возвращаем список всех издателей.");
                return _context.GetAllPublishers().OrderBy(p=>p.Name);
            }
            catch (Exception ex)
            {//если что-то пошло не так, выводим исключение в консоль
                Console.WriteLine("Возникла ошибка при получении списка всех издателей.");
                Log.Write(ex);
                return null;
            }
        }



        [HttpGet("{id}")]
        //получить автора по его id
        public IActionResult GetPublisher([FromRoute] int id)
        {
            try
            {
                //получить заказ по id заказа
                if (!ModelState.IsValid)
                {
                    Log.WriteSuccess(" PublisherController.GetPublisher", "Валидация внутри контроллера неудачна.");
                    return BadRequest(ModelState);
                }
                var publisher =  _context.GetPublisher(id);
                if (publisher == null)//если ничего не получили -- не найдено
                {
                    Log.WriteSuccess(" PublisherController.GetAuthorPublisher ", "ничего не получили.");
                    return NotFound();
                }
                return Ok(publisher);//возвращаем издательство
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                return BadRequest();
            }
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public IActionResult Create([FromBody] PublisherModel publisher)
        {//добавить нового автора
         //получаем данные о заказе во входных параметрах
            try
            {
                if (!ModelState.IsValid)
                {
                    Log.WriteSuccess(" PublisherController.Create", "Валидация внутри контроллера неудачна.");
                    return BadRequest(ModelState);
                }
                Log.WriteSuccess(" PublisherController.Create", "Данные валидны.");
                IEnumerable<PublisherModel> publishers = _context.GetAllPublishers().Where(d => d.Name == publisher.Name);
                if (!publishers.Any()) { _context.CreatePublisher(publisher); }//добавление автора в БД
                else {
                    Log.WriteSuccess(" PublisherController.Create", "Попытка добавить существующее издательство!");
                    return Conflict();
                }
                publisher = _context.GetAllPublishers().Where(d => d.Name == publisher.Name).FirstOrDefault();
                Log.WriteSuccess(" PublisherController.Create", "добавление издательства " + publisher.Id + " в БД");
                return CreatedAtAction("GetPublisher", new { id = publisher.Id }, publisher);
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                return BadRequest();
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public IActionResult Update([FromRoute] int id, [FromBody] PublisherModel publisher)
        {//обновить существующий заказ
            try
            {
                if (!ModelState.IsValid)
                {
                    Log.WriteSuccess(" PublisherController.Update", "Валидация внутри контроллера неудачна.");
                    return BadRequest(ModelState);
                }
                _context.UpdatePublisher(publisher);
                Log.WriteSuccess("PublisherController.Update", "обновление издательства " + publisher.Id + " в БД.");
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
                    Log.WriteSuccess(" PublisherController.Delete", "Валидация внутри контроллера неудачна.");
                    return BadRequest(ModelState);
                }
                _context.DeletePublisher(id);
                Log.WriteSuccess(" PublisherController.Delete", "удаление издательства " + id + " в БД.");
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
