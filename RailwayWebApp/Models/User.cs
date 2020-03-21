using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RailwayWebApp.Models
{
    public class User
    {
        public int IdUser { get; set; }
        public string UserType { get; set; }

        [Required (ErrorMessage = "Не указан логин")]
        public string UserLogin { get; set; }
        public string UserHash { get; set; }
        public string UserSalt { get; set; }

        public ICollection<Passenger> Passengers { get; set; }
    }
}
