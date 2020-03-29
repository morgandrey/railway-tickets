using System;
using System.Collections.Generic;

namespace RailwayDesktopApp.Models
{
    public class Passenger
    {
        public int IdPassenger { get; set; }
        public string PassengerFullName { get; set; }
        public DateTime PassengerBirthday { get; set; }
        public int IdPassengerPassportType { get; set; }
        public string PassengerPassport { get; set; }
        public int IdUser { get; set; }

        public PassportType IdPassengerPassportTypeNavigation { get; set; }
        public User IdUserNavigation { get; set; }
        public ICollection<Sale> Sale { get; set; }
    }
}
