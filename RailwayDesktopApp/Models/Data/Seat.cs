using System;
using System.Collections.Generic;

namespace RailwayDesktopApp.Models.Data
{
    public partial class Seat
    {
        public Seat()
        {
            Ticket = new HashSet<Ticket>();
        }

        public int IdSeat { get; set; }
        public int IdWagon { get; set; }
        public int Seat1 { get; set; }
        public bool SeatAvailability { get; set; }

        public virtual Wagon IdWagonNavigation { get; set; }
        public virtual ICollection<Ticket> Ticket { get; set; }
    }
}
