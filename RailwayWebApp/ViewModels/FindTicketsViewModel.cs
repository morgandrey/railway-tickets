using System;

namespace RailwayWebApp.ViewModels {
    public class FindTicketsViewModel {

        public string DepartureTown { get; set; }
        public string ArrivalTown { get; set; }
        public DateTime? DepartureTime { get; set; }
        public string TrainName { get; set; }
    }
}