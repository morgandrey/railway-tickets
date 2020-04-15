using System.Linq;
using RailwayWebApp.Data;

namespace RailwayWebApp.Models {
    public static class SampleData {
        public static void Initialize(RailwaysDBContext context) {
            if (context.Database.EnsureCreated()) {
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
                if (context.User.FirstOrDefault(x => x.UserType == "admin") == null)
                {
                    context.User.Add(new User
                    {
                        UserType = "admin",
                        UserLogin = "admin",
                        UserSalt = "OpBHsm/+DtkSaYzfWJbq9A==",
                        UserHash = "mSqUFm6YahFaXF2pos7RFXpkS8kySmGumv6Rru9QHhs="
                    });
                }
                context.SaveChanges();
            }
        }
    }
}