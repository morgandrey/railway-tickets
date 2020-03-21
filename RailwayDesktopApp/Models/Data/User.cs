using System;
using System.Collections.Generic;

namespace RailwayDesktopApp.Models.Data
{
    public partial class User
    {
        public User()
        {
            Passenger = new HashSet<Passenger>();
        }

        public int IdUser { get; set; }
        public bool UserType { get; set; }
        public string UserLogin { get; set; }
        public string UserHash { get; set; }
        public string UserSalt { get; set; }

        public virtual ICollection<Passenger> Passenger { get; set; }
    }
}
