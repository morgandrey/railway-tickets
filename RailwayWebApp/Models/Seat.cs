using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RailwayWebApp.Models {
    public class Seat
    {
        public int IdSeat { get; set; }
        public int IdWagon { get; set; }

        [DisplayName("Номер места")]
        [Range(1, 50, ErrorMessage = "Введите число от 1 до 50")]
        public int Seat1 { get; set; }
        public bool SeatAvailability { get; set; }

        public Wagon WagonNavigation { get; set; }
        public ICollection<Ticket> Tickets { get; set; }
    }
}
