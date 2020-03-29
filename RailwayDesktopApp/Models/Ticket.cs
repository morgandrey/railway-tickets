using System;
using System.Collections.Generic;

namespace RailwayDesktopApp.Models
{
    public class Ticket
    {
        public int IdTicket { get; set; }
        public int IdSeat { get; set; }
        public int IdTrainDepartureTown { get; set; }
        public int IdTrainArrivalTown { get; set; }
        public DateTime TicketDate { get; set; }
        public TimeSpan TicketTravelTime { get; set; }

        public Seat IdSeatNavigation { get; set; }
        public TrainArrivalTown IdTrainArrivalTownNavigation { get; set; }
        public TrainDepartureTown IdTrainDepartureTownNavigation { get; set; }
        public ICollection<Sale> Sale { get; set; }
    }
}
