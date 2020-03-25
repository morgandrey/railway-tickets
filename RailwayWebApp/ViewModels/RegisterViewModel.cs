using System;
using System.ComponentModel.DataAnnotations;

namespace RailwayWebApp.ViewModels {
    public class RegisterViewModel {
        [Required(ErrorMessage = "Обязательное поле")]
        [Display(Name = "Логин")]
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
        public string PassportData { get; set; }
    }
}