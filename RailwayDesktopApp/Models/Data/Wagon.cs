using System;
using System.Collections.Generic;

namespace RailwayDesktopApp.Models.Data
{
    public partial class Wagon
    {
        public Wagon()
        {
            Seat = new HashSet<Seat>();
        }

        public int IdWagon { get; set; }
        public int IdTrainWagon { get; set; }
        public int WagonNumber { get; set; }
        public int IdWagonType { get; set; }

        public virtual TrainWagon IdTrainWagonNavigation { get; set; }
        public virtual WagonType IdWagonTypeNavigation { get; set; }
        public virtual ICollection<Seat> Seat { get; set; }
    }
}
