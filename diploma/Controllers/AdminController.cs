﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using diploma.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace diploma.Controllers
{
    public class AdminController : Controller
    {


        private readonly BookingContext _context;
        private readonly UserManager<User> _userManager;
        IHostingEnvironment _appEnvironment;

        public AdminController(BookingContext context,
     UserManager<User> userManager, IHostingEnvironment appEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _appEnvironment = appEnvironment;
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


        private async Task<Order> GetCurrentOrder()
        {
            try
            {
                User usr = await _userManager.GetUserAsync(HttpContext.User);
                if (usr != null)
                {
                    string id = usr.Id;
                    IEnumerable<Order> orders = _context.Order.Where(p => p.UserId == id && p.Active == 1).Include(p => p.BookOrders);
                    return orders.FirstOrDefault();
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {

                Log.Write(ex);
                return null;
            }
        }


        [HttpGet]
        //[Route("/Admin/Book/{id}")][FromRoute]
        public async Task<IActionResult> Book(int id)
        {
            ViewBag.Username = GetUserName().Result;

            try
            {
                if (!ModelState.IsValid)
                {
                    Log.WriteSuccess(" BookViewsController.GetBook", "Валидация внутри контроллера неудачна.");
                    return BadRequest(ModelState);
                }

                var item = await _context.Book.SingleOrDefaultAsync(m => m.Id == id);

                if (item == null)
                {
                    Log.WriteSuccess(" BookViewsController.GetBook", "Книга не найдена.");
                    return NotFound();
                }
                BookAdd b = new BookAdd()
                {
                    Id = item.Id,
                    Content = item.Content,
                    isDeleted = item.isDeleted,
                    Cost = item.Cost,
                    image = item.image,
                    Publisher = item.IdPublisher,
                    Stored = item.Stored,
                    Title = item.Title,
                    Year = item.Year
                };
                ViewBag.image = item.image;
                List<int> au = new List<int>();
                List<string> aus = new List<string>();
                List<int> ge = new List<int>();
                List<string> ges = new List<string>();
                IEnumerable<BookAuthor> bookauthors = _context.BookAuthor.Where(f => f.IdBook == item.Id);
                IEnumerable<BookGenre> bookgenres = _context.BookGenre.Where(f => f.IdBook == item.Id);
                foreach (BookAuthor line in bookauthors)
                {
                    Author author = _context.Author.Find(line.IdAuthor);
                    au.Add(author.Id);
                    aus.Add(author.Name);
                }
                b.idAuthors = au.ToArray();
                b.Authors = aus.ToArray();
                foreach (BookGenre line in bookgenres)
                {
                    Genre genre = _context.Genre.Find(line.IdGenre);
                    ge.Add(genre.Id);
                    ges.Add(genre.Name);
                }
                b.idGenres = ge.ToArray();
                b.Genres = ges.ToArray();
                BookDetails bd = new BookDetails()
                {
                    Book = b,
                    CurrentOrder = GetCurrentOrder().Result
                };
                return View(bd);
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
            return View();
        }


        [HttpGet]
        public IActionResult AddBook(string filename)
        {
            ViewBag.Username = GetUserName().Result;
            ViewBag.FileName = filename;
            return View();
        }

        [HttpGet]
        public IActionResult UploadPicture()
        {
            ViewBag.Username = GetUserName().Result;
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> UploadPicture(IFormFile file)
        {
            //Task<ActionResult>
            if (file != null)
                try
                {
                    string format;
                    String[] words = file.FileName.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                    format = words[words.Length - 1];
                    string b = DateTime.Now.ToFileTime() + "." + format;
                    //Path.GetFileName(file.FileName)
                    string path = Path.Combine(_appEnvironment.WebRootPath + "\\img\\", b);
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                    //RedirectToAction("AddBook","Admin", new { filename = b });
                    return RedirectToAction("AddBook", new { filename = b });
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "ERROR:" + ex.Message.ToString();
                }
            else
            {
                ViewBag.Message = "You have not specified a file.";
            }
            //return BadRequest();
            return View();
            //return RedirectToAction("Index");
        }

    }
}