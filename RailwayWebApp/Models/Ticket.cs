using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RailwayWebApp.Models {
    public partial class Ticket
    {
        public int IdTicket { get; set; }
        public int IdSeat { get; set; }
        public int IdTrainDepartureTown { get; set; }
        public int IdTrainArrivalTown { get; set; }

        [DisplayName("Дата поездки")]
        [Required(ErrorMessage = "Не указана дата отправления")]
        public DateTime? TicketDate { get; set; }

        [DisplayName("Время в пути")]
        [Required(ErrorMessage = "Не указано время в пути")]
        public TimeSpan? TicketTravelTime { get; set; }

        public Seat SeatNavigation { get; set; }
        public TrainArrivalTown TrainArrivalTownNavigation { get; set; }
        public TrainDepartureTown TrainDepartureTownNavigation { get; set; }
        public ICollection<Sale> Sales { get; set; }
    }
}
