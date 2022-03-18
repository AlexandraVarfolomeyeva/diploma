using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace BLL.Models
{
    public class LoginViewModel
    {

        [Required(ErrorMessage = "Не указано имя пользователя")]
        [Display(Name = "User")]
        public string User { get; set; }
        [Required(ErrorMessage = "Не указан пароль")]
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
