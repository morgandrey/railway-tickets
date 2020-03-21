using System.Collections.Generic;

namespace RailwayWebApp.Models
{
    public partial class TrainWagon
    {
        public int IdTrainWagon { get; set; }
        public int IdTrain { get; set; }
        public int TrainTravelCount { get; set; }

        public Train TrainNavigation { get; set; }
        public ICollection<Wagon> Wagon { get; set; }
    }
}
