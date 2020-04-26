﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using diploma.Models;
using System.IO;
using Microsoft.AspNetCore.Http;
using BookShop.Models;
using Microsoft.EntityFrameworkCore;
using BookShop;
using Microsoft.AspNetCore.Hosting;

namespace diploma.Controllers
{
    public class HomeController : Controller
    {

        private readonly BookingContext _context;
        IHostingEnvironment _appEnvironment;

        public HomeController(BookingContext context, IHostingEnvironment appEnvironment)
        {
            _context = context;
            _appEnvironment = appEnvironment;
        }

        public IActionResult Index()
        {
                IEnumerable<Book> books = _context.Book.Include(p => p.BookOrders).Where(d => d.isDeleted == false);
                BookView[] bookViews = new BookView[books.Count()];
                int i = 0;
                foreach (Book item in books)
                {
                    bookViews[i] = new BookView()
                    {
                        Id = item.Id,
                        image = item.image,
                        Stored = item.Stored,
                        Title = item.Title,
                        Year = item.Year,
                        Cost = item.Cost,
                        Content = item.Content
                    };
                    Publisher publisher = _context.Publisher.Find(item.IdPublisher);
                    List<string> au = new List<string>();
                    List<string> ge = new List<string>();
                    bookViews[i].Publisher = publisher.Name;
                    IEnumerable<BookAuthor> bookauthors = _context.BookAuthor.Where(b => b.IdBook == item.Id);
                    IEnumerable<BookGenre> bookgenres = _context.BookGenre.Where(b => b.IdBook == item.Id);
                    foreach (BookAuthor line in bookauthors)
                    {
                        Author author = _context.Author.Find(line.IdAuthor);
                        au.Add(author.Name);
                    }
                    bookViews[i].Authors = au.ToArray();
                    foreach (BookGenre line in bookgenres)
                    {
                        Genre genre = _context.Genre.Find(line.IdGenre);
                        ge.Add(genre.Name);
                    }
                    bookViews[i].Genres = ge.ToArray();
                    i++;
                }
                IEnumerable<BookView> views = bookViews;
                ViewBag.Books = bookViews;
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> UploadFile(IFormFile file)
        {
            //Task<ActionResult>
            if (file != null )
                try
                {
                    string path = Path.Combine(_appEnvironment.WebRootPath+"\\img\\", Path.GetFileName(file.FileName));
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                    ViewBag.Message = "File uploaded successfully";
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "ERROR:" + ex.Message.ToString();
                }
            else
            {
                ViewBag.Message = "You have not specified a file.";
            }
            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
