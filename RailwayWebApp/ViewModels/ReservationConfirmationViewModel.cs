using System;
using System.ComponentModel.DataAnnotations;

namespace RailwayWebApp.ViewModels {
    public class ReservationConfirmationViewModel {
        public int IdDiscount { get; set; }
        [Display(Name = "ФИО пассажира")]
        public string PassengerFullName { get; set; }
        [Display(Name = "Тип паспорта")]
        public string PassportType { get; set; }
        [Display(Name = "Паспортные данные")]
        public string PassportData { get; set; }
        [Display(Name = "Город отбытия")]
        public string DepartureTown { get; set; }
        [Display(Name = "Город прибытия")]
        public string ArrivalTown { get; set; }
        [Display(Name = "Время отправления")]
        public DateTime? DepartureTime { get; set; }
        [Display(Name = "Тип вагона")]
        public string WagonType { get; set; }
        [Display(Name = "Номер вагона")]
        public int WagonNumber { get; set; }
        [Display(Name = "Место")]
        public int  SeatNumber { get; set; }
        [Display(Name = "Цена")]
        public double Price { get; set; }
        public int  IdPassenger { get; set; }
        public int IdTicket { get; set; }
    }
}