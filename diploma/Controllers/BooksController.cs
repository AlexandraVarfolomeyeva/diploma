using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BLL.Interfaces;
using BLL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace diploma.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IDBCrud _context;

        public BooksController(IDBCrud context)
        {
            _context = context;
        }

        [HttpGet]
        public IEnumerable<BookModel> GetAll() //получение списка всех книг
        {
            try
            {
                return _context.GetAllBooks().Where(book => book.isDeleted == false);
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                return null;
            }

        }
   

        [HttpGet("{id}")]
        public IActionResult GetBook([FromRoute] int id)
        {//получение книги по id
            try
            {
                if (!ModelState.IsValid)
                {
                    Log.WriteSuccess(" BooksController.GetBook", "Валидация внутри контроллера неудачна.");
                    return BadRequest(ModelState);
                }

                BookModel item = _context.GetBookModel(id);

                if (item == null)
                {
                    Log.WriteSuccess(" BooksController.GetBook", "Книга не найдена.");
                    return NotFound();
                }
                return Ok(item);
            } catch(Exception ex)
            {
                Log.Write(ex);
                return BadRequest(ModelState);
            }
        }

        [HttpPost]
        [Authorize(Roles = "seller")]
        public IActionResult Create([FromBody] BookAdd item)
        {//создание новой книги возможно только администратором
            try
            {
                if (!ModelState.IsValid)
                {
                    Log.WriteSuccess(" BooksController.Create", "Валидация внутри контроллера неудачна.");
                    return BadRequest(ModelState);
                }
                _context.CreateBook(item);
              
                return CreatedAtAction("GetBook", new { id = item.Id }, item);
            } catch (Exception ex)
            {
                Log.Write(ex);
                return BadRequest(ModelState);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "seller")]
        public IActionResult Update([FromRoute] int id, [FromBody] BookModel book)
        {//обновление информации о существующей книге возможно только администратором
          try{  if (!ModelState.IsValid)
            {
                Log.WriteSuccess(" BooksController.Update", "Валидация внутри контроллера неудачна.");
                return BadRequest(ModelState);
            }

            _context.UpdateBook(book);
            return NoContent();
        } catch (Exception ex)
            {
                Log.Write(ex);
                return BadRequest(ModelState);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "seller")]
        public IActionResult Delete([FromRoute] int id)
        {//удаление книги из БД возможно только администратором
            try
            {
                if (!ModelState.IsValid)
                {
                    Log.WriteSuccess(" BooksController.Delete", "Валидация внутри контроллера неудачна.");
                    return BadRequest(ModelState);
                }
                _context.DeleteBook(id);
                return NoContent();
            } catch (Exception ex)
            {
                Log.Write(ex);
                return BadRequest(ModelState);
            }
        }
    }
}
