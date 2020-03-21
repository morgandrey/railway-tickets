using System;
using System.Collections.Generic;

namespace RailwayDesktopApp.Models.Data
{
    public partial class TrainArrivalTown
    {
        public TrainArrivalTown()
        {
            Ticket = new HashSet<Ticket>();
        }

        public int IdTrainArrivalTown { get; set; }
        public string TownName { get; set; }

        public virtual ICollection<Ticket> Ticket { get; set; }
    }
}
