using System.Collections.Generic;

namespace RailwayDesktopApp.Models
{
    public class TrainWagon
    {
        public int IdTrainWagon { get; set; }
        public int IdTrain { get; set; }
        public int TrainTravelCount { get; set; }

        public Train IdTrainNavigation { get; set; }
        public ICollection<Wagon> Wagon { get; set; }
    }
}
