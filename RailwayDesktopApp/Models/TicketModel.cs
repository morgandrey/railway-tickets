using System;

namespace RailwayDesktopApp.Models {
    public class TicketModel {
        public string DepartureTown { get; set; }
        public string ArrivalTown { get; set; }
        public DateTime? DepartureTime { get; set; }
        public string TrainName { get; set; }
        public TimeSpan TravelDuration { get; set; }
    }
}