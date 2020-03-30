using System.Collections.Generic;
using System.ComponentModel;

namespace RailwayWebApp.Models
{
    public class PassportType
    {
        public int IdPassportType { get; set; }

        [DisplayName("Вид паспорта")]
        public string Passport { get; set; }

        public ICollection<Passenger> Passengers { get; set; }
    }
}
