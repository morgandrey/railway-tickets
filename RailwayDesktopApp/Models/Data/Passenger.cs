using System;
using System.Collections.Generic;

namespace RailwayDesktopApp.Models.Data
{
    public partial class Passenger
    {
        public Passenger()
        {
            Sale = new HashSet<Sale>();
        }

        public int IdPassenger { get; set; }
        public string PassengerFullName { get; set; }
        public DateTime PassengerBirthday { get; set; }
        public int IdPassengerPassportType { get; set; }
        public string PassengerPassport { get; set; }
        public int IdUser { get; set; }

        public virtual PassportType IdPassengerPassportTypeNavigation { get; set; }
        public virtual User IdUserNavigation { get; set; }
        public virtual ICollection<Sale> Sale { get; set; }
    }
}
