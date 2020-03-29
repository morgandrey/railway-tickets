using System.Collections.Generic;

namespace RailwayDesktopApp.Models
{
    public class Seat
    {
        public int IdSeat { get; set; }
        public int IdWagon { get; set; }
        public int Seat1 { get; set; }
        public bool SeatAvailability { get; set; }

        public Wagon IdWagonNavigation { get; set; }
        public ICollection<Ticket> Ticket { get; set; }
    }
}
