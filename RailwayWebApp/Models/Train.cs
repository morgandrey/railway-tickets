using System.Collections.Generic;
using System.ComponentModel;

namespace RailwayWebApp.Models {
    public class Train
    {
        public int IdTrain { get; set; }
        [DisplayName("Название поезда")]
        public string TrainName { get; set; }

        public ICollection<TrainWagon> TrainWagon { get; set; }
    }
}
