using System.Collections.Generic;
using System.ComponentModel;

namespace RailwayWebApp.Models
{
    public class TrainArrivalTown
    {
        public int IdTrainArrivalTown { get; set; }

        [DisplayName("Город прибытия")]
        public string TownName { get; set; }

        public ICollection<Ticket> Tickets { get; set; }
    }
}
