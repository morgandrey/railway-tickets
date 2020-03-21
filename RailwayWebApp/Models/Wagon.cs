using System.Collections.Generic;

namespace RailwayWebApp.Models
{
    public class Wagon
    {
        public int IdWagon { get; set; }
        public int IdTrainWagon { get; set; }
        public int WagonNumber { get; set; }
        public int IdWagonType { get; set; }

        public TrainWagon TrainWagonNavigation { get; set; }
        public WagonType WagonTypeNavigation { get; set; }
        public ICollection<Seat> Seats { get; set; }
    }
}
