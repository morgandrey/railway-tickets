using System;
using System.Collections.Generic;

namespace RailwayDesktopApp.Models.Data
{
    public partial class Discount
    {
        public Discount()
        {
            Sale = new HashSet<Sale>();
        }

        public int IdDiscount { get; set; }
        public string DiscountName { get; set; }
        public double DiscountMultiply { get; set; }

        public virtual ICollection<Sale> Sale { get; set; }
    }
}
