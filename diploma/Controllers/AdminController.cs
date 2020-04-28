using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace diploma.Controllers
{
    public class AdminController : Controller
    {
        [HttpGet]
        public IActionResult AddBook()
        {
            return View();
        }

      
    }
}