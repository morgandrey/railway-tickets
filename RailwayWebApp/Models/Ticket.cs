using System;
using System.Collections.Generic;

namespace RailwayWebApp.Models {
    public partial class Ticket
    {
        public int IdTicket { get; set; }
        public int IdSeat { get; set; }
        public int IdTrainDepartureTown { get; set; }
        public int IdTrainArrivalTown { get; set; }
        public DateTime TicketDate { get; set; }
        public TimeSpan TicketTravelTime { get; set; }

        public Seat SeatNavigation { get; set; }
        public TrainArrivalTown TrainArrivalTownNavigation { get; set; }
        public TrainDepartureTown TrainDepartureTownNavigation { get; set; }
        public ICollection<Sale> Sales { get; set; }
    }
}
