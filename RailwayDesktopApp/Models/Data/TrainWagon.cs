using System;
using System.Collections.Generic;

namespace RailwayDesktopApp.Models.Data
{
    public partial class TrainWagon
    {
        public TrainWagon()
        {
            Wagon = new HashSet<Wagon>();
        }

        public int IdTrainWagon { get; set; }
        public int IdTrain { get; set; }
        public int TrainTravelCount { get; set; }

        public virtual Train IdTrainNavigation { get; set; }
        public virtual ICollection<Wagon> Wagon { get; set; }
    }
}
