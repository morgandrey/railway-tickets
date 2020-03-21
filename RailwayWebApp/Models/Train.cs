using System.Collections.Generic;

namespace RailwayWebApp.Models {
    public class Train
    {
        public int IdTrain { get; set; }
        public string TrainName { get; set; }

        public ICollection<TrainWagon> TrainWagon { get; set; }
    }
}
