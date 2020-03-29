using System.Collections.Generic;

namespace RailwayDesktopApp.Models
{
    public  class TrainArrivalTown
    {
        public int IdTrainArrivalTown { get; set; }
        public string TownName { get; set; }

        public ICollection<Ticket> Ticket { get; set; }
    }
}
