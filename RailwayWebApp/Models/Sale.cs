using System;
using System.ComponentModel;

namespace RailwayWebApp.Models {
    public class Sale
    {
        public int IdSale { get; set; }

        [DisplayName("Пассажир")]
        public int IdPassenger { get; set; }
        public int IdTicket { get; set; }
        public DateTime SaleDate { get; set; }

        [DisplayName("Скидка")]
        public int IdDiscount { get; set; }
        public double TotalPrice { get; set; }

        public Discount DiscountNavigation { get; set; }
        public Passenger PassengerNavigation { get; set; }
        public Ticket TicketNavigation { get; set; }
    }
}
