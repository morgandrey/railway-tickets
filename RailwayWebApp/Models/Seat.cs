using System.Collections.Generic;

namespace RailwayWebApp.Models {
    public class Seat
    {
        public int IdSeat { get; set; }
        public int IdWagon { get; set; }
        public int Seat1 { get; set; }
        public bool SeatAvailability { get; set; }

        public Wagon WagonNavigation { get; set; }
        public ICollection<Ticket> Tickets { get; set; }
    }
}
