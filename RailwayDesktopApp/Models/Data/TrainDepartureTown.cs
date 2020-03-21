using System;
using System.Collections.Generic;

namespace RailwayDesktopApp.Models.Data
{
    public partial class TrainDepartureTown
    {
        public TrainDepartureTown()
        {
            Ticket = new HashSet<Ticket>();
        }

        public int IdTrainDepartureTown { get; set; }
        public string TownName { get; set; }

        public virtual ICollection<Ticket> Ticket { get; set; }
    }
}
