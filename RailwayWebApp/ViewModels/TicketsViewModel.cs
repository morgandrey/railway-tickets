using System;
using System.ComponentModel.DataAnnotations;

namespace RailwayWebApp.ViewModels {
    public class TicketsViewModel {
        [Display(Name = "Город отбытия")]
        public string DepartureTown { get; set; }
        [Display(Name = "Город прибытия")]
        public string ArrivalTown { get; set; }
        [Display(Name = "Дата отправления")]
        public DateTime? DepartureTime { get; set; }
        [Display(Name = "Номер поезда")]
        public string TrainName { get; set; }
    }
}