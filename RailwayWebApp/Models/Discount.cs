using System.Collections.Generic;
using RailwayWebApp.Data;

namespace RailwayWebApp.Models
{
    public class Discount
    {
        public int IdDiscount { get; set; }
        public string DiscountName { get; set; }
        public double DiscountMultiply { get; set; }

        public virtual ICollection<Sale> Sales { get; set; }
    }
}
