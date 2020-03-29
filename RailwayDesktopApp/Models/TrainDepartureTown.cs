using System.Collections.Generic;

namespace RailwayDesktopApp.Models
{
    public class TrainDepartureTown
    {
        public int IdTrainDepartureTown { get; set; }
        public string TownName { get; set; }

        public ICollection<Ticket> Ticket { get; set; }
    }
}
