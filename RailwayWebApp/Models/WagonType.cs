using System.Collections.Generic;

namespace RailwayWebApp.Models
{
    public class WagonType
    {
        public int IdWagonType { get; set; }
        public string WagonType1 { get; set; }
        public double WagonPrice { get; set; }

        public ICollection<Wagon> Wagons { get; set; }
    }
}
