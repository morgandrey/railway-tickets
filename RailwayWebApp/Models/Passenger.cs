using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RailwayWebApp.Models
{
    public class Passenger 
    {
        public int IdPassenger { get; set; }

        [Required (ErrorMessage = "Не указано ФИО")]
        [DisplayName("Полное имя")]
        public string PassengerFullName { get; set; }

        [Required (ErrorMessage = "Не указан день рождения")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        [DisplayName("Дата рождения")]
        public DateTime? PassengerBirthday { get; set; }
        public int IdPassengerPassportType { get; set; }

        [Required(ErrorMessage = "Не указаны паспортные данные")]
        [StringLength(12, MinimumLength = 8, ErrorMessage = "Не правильные данные паспорта")]
        [DisplayName("Паспортные данные")]
        public string PassengerPassport { get; set; }
        public int IdUser { get; set; }

        public PassportType PassportTypeNavigation { get; set; }
        public User UserNavigation { get; set; }
        public ICollection<Sale> Sales { get; set; }
    }
}