using System.Collections.Generic;

namespace RailwayWebApp.Models
{
    public class Discount
    {
        public int IdDiscount { get; set; }
        public string DiscountName { get; set; }
        public double DiscountMultiply { get; set; }

        public ICollection<Sale> Sales { get; set; }
    }
}
