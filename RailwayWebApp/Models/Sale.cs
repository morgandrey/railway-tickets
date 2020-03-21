using System;

namespace RailwayWebApp.Models {
    public class Sale
    {
        public int IdSale { get; set; }
        public int IdPassenger { get; set; }
        public int IdTicket { get; set; }
        public DateTime SaleDate { get; set; }
        public int IdDiscount { get; set; }
        public double TotalPrice { get; set; }

        public Discount DiscountNavigation { get; set; }
        public Passenger PassengerNavigation { get; set; }
        public Ticket TicketNavigation { get; set; }
    }
}
