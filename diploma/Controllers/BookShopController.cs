using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BookShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookShopController : ControllerBase
    {
        private readonly BookingContext _context;
        public BookShopController(BookingContext context)
        {
            _context = context;
            if (_context.User.Count() == 0)
            {
                _context.User.Add(new User
                { 
                Address=" ",
                //Login="login",
                //Name ="name",
                //Password="password",
                //Phone=" ",
                //Url = "http:\\BookShop.net"
                });
                _context.SaveChanges();
            }
        }

        [HttpGet]
        public IEnumerable<User> GetAll()
        {
            return _context.User.Include(p => p.Orders); //.Include(p => p.);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _context.User.SingleOrDefaultAsync(m => m.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.User.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.Id }, user);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var item = _context.User.Find(id);
            if (item == null)
            {
                return NotFound();
            }
            // item.Url = user.Url;
            //item.Address = user.Address;
            //item.Login = user.Login;
            //item.Name = user.Name;
            //item.Password = user.Password;
            //item.Phone = user.Phone;
            _context.User.Update(item);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var item = _context.User.Find(id);
            if (item == null)
            {
                return NotFound();
            }
            _context.User.Remove(item);
            await _context.SaveChangesAsync();
            return NoContent();
        }



    }
}
