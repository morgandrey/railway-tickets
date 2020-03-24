using System.Collections.Generic;
using System.ComponentModel;

namespace RailwayWebApp.Models
{
    public class TrainDepartureTown
    {
        public int IdTrainDepartureTown { get; set; }

        [DisplayName("Город отбытия")]
        public string TownName { get; set; }

        public ICollection<Ticket> Tickets { get; set; }
    }
}
