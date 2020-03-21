using System.Collections.Generic;

namespace RailwayWebApp.Models
{
    public class TrainArrivalTown
    {
        public int IdTrainArrivalTown { get; set; }
        public string TownName { get; set; }

        public ICollection<Ticket> Tickets { get; set; }
    }
}
