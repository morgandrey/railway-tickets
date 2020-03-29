using System.Collections.Generic;

namespace RailwayDesktopApp.Models
{
    public class Discount
    {
        public int IdDiscount { get; set; }
        public string DiscountName { get; set; }
        public double DiscountMultiply { get; set; }

        public ICollection<Sale> Sale { get; set; }
    }
}
