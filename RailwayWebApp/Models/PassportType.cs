using System;
using System.Collections.Generic;

namespace RailwayWebApp.Models
{
    public class PassportType
    {
        public int IdPassportType { get; set; }
        public string Passport { get; set; }

        public ICollection<Passenger> Passengers { get; set; }
    }
}
