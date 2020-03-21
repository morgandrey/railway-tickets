using System;
using System.Collections.Generic;

namespace RailwayDesktopApp.Models.Data
{
    public partial class Ticket
    {
        public Ticket()
        {
            Sale = new HashSet<Sale>();
        }

        public int IdTicket { get; set; }
        public int IdSeat { get; set; }
        public int IdTrainDepartureTown { get; set; }
        public int IdTrainArrivalTown { get; set; }
        public DateTime TicketDate { get; set; }
        public TimeSpan TicketTravelTime { get; set; }

        public virtual Seat IdSeatNavigation { get; set; }
        public virtual TrainArrivalTown IdTrainArrivalTownNavigation { get; set; }
        public virtual TrainDepartureTown IdTrainDepartureTownNavigation { get; set; }
        public virtual ICollection<Sale> Sale { get; set; }
    }
}
