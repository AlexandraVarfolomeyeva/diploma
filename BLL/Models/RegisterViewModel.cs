using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.Models
{
    public class RegisterViewModel
    {
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Не указано ФИО")]
        [Display(Name = "ФИО")]
        [RegularExpression("[A-Za-zА-Яа-я- ]{3,}", ErrorMessage = "Пожалуйста, введите полное ФИО, избегая цифр и знаков.")]
        public string Fio { get; set; }

        [Required(ErrorMessage = "Не указан Email")]
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Пожалуйста, введите ваш настоящий Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Не указано имя пользователя")]
        [Display(Name = "Имя пользователя")]
        [RegularExpression("[0-9a-z_-]+", ErrorMessage = "Только латинские буквы нижнего регистра, цифры, знаки дефиса и нижнего подчеркивания.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Не указан номер телефона")]
        [Display(Name = "Номер телефона")]
        [RegularExpression("[0-9]{10}", ErrorMessage = "Пожалуйста, введите свой настоящий номер мобильного телефона.")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Не указан физический адрес")]
        [Display(Name = "Адрес")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Не указан город")]
        [Display(Name = "Город")]
        public int IdCity { get; set; }

        [Required(ErrorMessage = "Не указан пароль")]
        [DataType(DataType.Password, ErrorMessage = "Пароль должен содержать не менее шести символов и состоять из латинских букв, цифр, как минимум одной заглавной и одной строчной буквы и одного служебного символа.")]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Пароль не был подтвержден.")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают.")]
        [DataType(DataType.Password, ErrorMessage = "Пароль должен содержать не менее шести символов и состоять из латинских букв, цифр, как минимум одной заглавной и одной строчной буквы и одного служебного символа.")]
        [Display(Name = "Подтвердить пароль")]
        public string PasswordConfirm { get; set; }
    }
}
