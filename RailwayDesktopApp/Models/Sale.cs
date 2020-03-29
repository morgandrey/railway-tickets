using System;

namespace RailwayDesktopApp.Models
{
    public class Sale
    {
        public int IdSale { get; set; }
        public int IdPassenger { get; set; }
        public int IdTicket { get; set; }
        public DateTime SaleDate { get; set; }
        public int IdDiscount { get; set; }
        public double TotalPrice { get; set; }

        public Discount IdDiscountNavigation { get; set; }
        public Passenger IdPassengerNavigation { get; set; }
        public Ticket IdTicketNavigation { get; set; }
    }
}
