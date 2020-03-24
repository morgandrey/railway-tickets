using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RailwayWebApp.Models
{
    public class Wagon
    {
        public int IdWagon { get; set; }
        public int IdTrainWagon { get; set; }

        [DisplayName("Номер вагона")]
        [Range(1, 30, ErrorMessage = "Введите число от 1 до 30")]
        public int WagonNumber { get; set; }
        public int IdWagonType { get; set; }

        public TrainWagon TrainWagonNavigation { get; set; }
        public WagonType WagonTypeNavigation { get; set; }
        public ICollection<Seat> Seats { get; set; }
    }
}
