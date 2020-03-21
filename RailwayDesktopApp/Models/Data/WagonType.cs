using System;
using System.Collections.Generic;

namespace RailwayDesktopApp.Models.Data
{
    public partial class WagonType
    {
        public WagonType()
        {
            Wagon = new HashSet<Wagon>();
        }

        public int IdWagonType { get; set; }
        public string WagonType1 { get; set; }
        public double WagonPrice { get; set; }

        public virtual ICollection<Wagon> Wagon { get; set; }
    }
}
