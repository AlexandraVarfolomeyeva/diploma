using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace BookShop.Models
{
    public class LoginViewModel
    {

        [Required]
        [Display(Name = "User")]
        public string User { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }
        [Display(Name = "Запомнить?")]
        public bool RememberMe { get; set; }
        [Display(Name = "Email")]
        public string Email { get; set; }
        public string ReturnUrl { get; set; }
    }
}
