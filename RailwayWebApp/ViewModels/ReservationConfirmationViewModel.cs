using System;

namespace RailwayWebApp.ViewModels {
    public class ReservationConfirmationViewModel {
        public int IdDiscount { get; set; }
        public string PassengerFullName { get; set; }
        public string PassportType { get; set; }
        public string PassportData { get; set; }
        public string DepartureTown { get; set; }
        public string ArrivalTown { get; set; }
        public DateTime? DepartureTime { get; set; }
        public string WagonType { get; set; }
        public int WagonNumber { get; set; }
        public int  SeatNumber { get; set; }
        public double Price { get; set; }
        public int  IdPassenger { get; set; }
        public int IdTicket { get; set; }
    }
}