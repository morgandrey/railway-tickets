using System;
using System.Collections.Generic;

namespace RailwayDesktopApp.Models.Data
{
    public partial class Sale
    {
        public int IdSale { get; set; }
        public int IdPassenger { get; set; }
        public int IdTicket { get; set; }
        public DateTime SaleDate { get; set; }
        public int IdDiscount { get; set; }
        public double TotalPrice { get; set; }

        public virtual Discount IdDiscountNavigation { get; set; }
        public virtual Passenger IdPassengerNavigation { get; set; }
        public virtual Ticket IdTicketNavigation { get; set; }
    }
}
