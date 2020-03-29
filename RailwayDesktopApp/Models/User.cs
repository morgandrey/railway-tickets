using System.Collections.Generic;

namespace RailwayDesktopApp.Models
{
    public partial class User
    {
        public int IdUser { get; set; }
        public string UserType { get; set; }
        public string UserLogin { get; set; }
        public string UserHash { get; set; }
        public string UserSalt { get; set; }

        public virtual ICollection<Passenger> Passenger { get; set; }
    }
}
