using System.Collections.Generic;

namespace RailwayDesktopApp.Models
{
    public class Wagon
    {
        public int IdWagon { get; set; }
        public int IdTrainWagon { get; set; }
        public int WagonNumber { get; set; }
        public int IdWagonType { get; set; }

        public TrainWagon IdTrainWagonNavigation { get; set; }
        public WagonType IdWagonTypeNavigation { get; set; }
        public ICollection<Seat> Seat { get; set; }
    }
}
