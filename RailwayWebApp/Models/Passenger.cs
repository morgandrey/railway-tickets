using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RailwayWebApp.Models
{
    public class Passenger 
    {
        public int IdPassenger { get; set; }

        [Required (ErrorMessage = "Не указано ФИО")]
        public string PassengerFullName { get; set; }

        [Required (ErrorMessage = "Не указан день рождения")]
        public DateTime PassengerBirthday { get; set; }
        public int IdPassengerPassportType { get; set; }
        [StringLength(12, MinimumLength = 8, ErrorMessage = "Не правильные данные паспорта")]
        public string PassengerPassport { get; set; }
        public int IdUser { get; set; }

        public PassportType PassportTypeNavigation { get; set; }
        public User UserNavigation { get; set; }
        public ICollection<Sale> Sales { get; set; }
    }
}