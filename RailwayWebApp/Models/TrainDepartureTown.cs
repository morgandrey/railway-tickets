using System.Collections.Generic;

namespace RailwayWebApp.Models
{
    public class TrainDepartureTown
    {
        public int IdTrainDepartureTown { get; set; }
        public string TownName { get; set; }

        public ICollection<Ticket> Tickets { get; set; }
    }
}
