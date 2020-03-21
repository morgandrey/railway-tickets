using System;
using System.Collections.Generic;

namespace RailwayDesktopApp.Models.Data
{
    public partial class Train
    {
        public Train()
        {
            TrainWagon = new HashSet<TrainWagon>();
        }

        public int IdTrain { get; set; }
        public string TrainName { get; set; }

        public virtual ICollection<TrainWagon> TrainWagon { get; set; }
    }
}
