using System.Linq;
using RailwayWebApp.Data;

namespace RailwayWebApp.Models {
    public static class SampleData {
        public static void Initialize(RailwaysDBContext context) {
            if (!context.PassportType.Any()) {
                context.PassportType.AddRange(
                    new PassportType {
                        Passport = "Паспорт РФ"
                    },
                    new PassportType {
                        Passport = "Загран паспорт"
                    });
            }

            if (!context.WagonType.Any()) {
                context.WagonType.AddRange(
                    new WagonType {
                        WagonType1 = "Плацкарт",
                        WagonPrice = 2000
                    },
                    new WagonType {
                        WagonType1 = "Купе",
                        WagonPrice = 3500
                    });
            }

            if (!context.Discount.Any()) {
                context.Discount.AddRange(
                    new Discount {
                        DiscountName = "Нет",
                        DiscountMultiply = 1
                    },
                    new Discount {
                        DiscountName = "Студент",
                        DiscountMultiply = 0.9
                    });
            }

            if (!context.TrainDepartureTown.Any()) {
                context.TrainDepartureTown.AddRange(
                    new TrainDepartureTown {
                        TownName = "Москва"
                    },
                    new TrainDepartureTown {
                        TownName = "Йошкар-Ола"
                    },
                    new TrainDepartureTown {
                        TownName = "Санкт-Петербург"
                    });
            }

            if (!context.TrainArrivalTown.Any()) {
                context.TrainArrivalTown.AddRange(
                    new TrainArrivalTown {
                        TownName = "Москва"
                    },
                    new TrainArrivalTown {
                        TownName = "Йошкар-Ола"
                    },
                    new TrainArrivalTown {
                        TownName = "Санкт-Петербург"
                    });
            }

            if (!context.Train.Any()) {
                context.Train.AddRange(
                    new Train {
                        TrainName = "Б-08"
                    },
                    new Train {
                        TrainName = "А-02"
                    }
                    );
            }
            context.SaveChanges();
        }
    }
}