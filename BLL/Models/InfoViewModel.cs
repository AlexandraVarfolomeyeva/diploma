﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.Models
{
    public class InfoViewModel
    {
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Не указано ФИО")]
        [Display(Name = "ФИО")]
        [RegularExpression("[A-Za-zА-Яа-я- ]{6,}", ErrorMessage = "Пожалуйста, введите полное ФИО, избегая цифр и знаков.")]
        public string Fio { get; set; }

        [Required(ErrorMessage = "Не указан Email")]
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Пожалуйста, введите ваш настоящий Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Не указано имя пользователя")]
        [Display(Name = "Имя пользователя")]
        [RegularExpression("[0-9a-z_-]+", ErrorMessage = "Только латинские буквы, цифры, знаки дефиса и нижнего подчеркивания")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Не указан номер телефона")]
        [Display(Name = "Номер телефона")]
        [Phone(ErrorMessage = "Поле номер телефона заполнено неверно")]
        public string PhoneNumber { get; set; }
    }
}
