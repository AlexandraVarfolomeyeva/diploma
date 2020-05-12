using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BLL.Models
{
    public class ChangePasswordViewModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }

        [Required(ErrorMessage = "Не указан старый пароль")]
        [DataType(DataType.Password, ErrorMessage = "Пароль должен состоять из латинских букв, цифр, как минимум одной заглавной и одной строчной буквы и одного служебного символа.")]
        [Display(Name = "Старый пароль")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "Не указан новый пароль")]
        [DataType(DataType.Password, ErrorMessage = "Пароль должен состоять из латинских букв, цифр, как минимум одной заглавной и одной строчной буквы и одного служебного символа.")]
        [Display(Name = "Новый пароль")]
        public string NewPassword { get; set; }


        [Required(ErrorMessage = "Не подтвержден новый пароль")]
        [DataType(DataType.Password, ErrorMessage = "Пароль должен состоять из латинских букв, цифр, как минимум одной заглавной и одной строчной буквы и одного служебного символа.")]
        [Display(Name = "Подтвердить пароль")]
        [Compare("NewPassword", ErrorMessage = "Пароли не совпадают")]
        public string NewPasswordConfirm { get; set; }

    }
}
