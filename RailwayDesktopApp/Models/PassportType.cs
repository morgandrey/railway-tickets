using System.Collections.Generic;

namespace RailwayDesktopApp.Models
{
    public class PassportType
    {
        public int IdPassportType { get; set; }
        public string PassportType1 { get; set; }

        public ICollection<Passenger> Passenger { get; set; }
    }
}
