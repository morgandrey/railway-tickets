using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RailwayWebApp.Models
{
    public class WagonType
    {
        public int IdWagonType { get; set; }
        [DisplayName("Тип вагона")]
        public string WagonType1 { get; set; }
        [Range(1.0,50000.0, ErrorMessage = "Неправильное значение цены")]
        [DisplayName("Цена")]
        public double WagonPrice { get; set; }

        public ICollection<Wagon> Wagons { get; set; }
    }
}
