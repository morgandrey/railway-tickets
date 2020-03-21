using System;
using System.Collections.Generic;

namespace RailwayDesktopApp.Models.Data
{
    public partial class PassportType
    {
        public PassportType()
        {
            Passenger = new HashSet<Passenger>();
        }

        public int IdPassportType { get; set; }
        public string PassportType1 { get; set; }

        public virtual ICollection<Passenger> Passenger { get; set; }
    }
}
