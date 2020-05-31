using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace RailwayWebApp.ViewModels {
    public class RegisterViewModel {
        [Required(ErrorMessage = "Обязательное поле")]
        [Display(Name = "Логин")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Логин от 1 до 100 символов")]
        [Remote("CheckLogin", "Account", ErrorMessage = "Логин уже используется")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        [Display(Name = "Пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Подтверждение пароля")]
        [Compare("Password",
            ErrorMessage = "Пароли не совпадают")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        [Display(Name = "Ф.И.О.")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        [Display(Name = "Дата рождения")]
        public DateTime? Birthday { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        [Display(Name = "Вид паспорта")]
        public int IdPassportType { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        [Display(Name = "Паспортные данные")]
        [StringLength(12, MinimumLength = 8, ErrorMessage = "Длина от 8 до 12")]
        public string PassportData { get; set; }
    }
}