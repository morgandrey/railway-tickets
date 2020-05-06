using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RailwayWebApp.Data;
using RailwayWebApp.Models;
using RailwayWebApp.ViewModels;

namespace RailwayWebApp.Controllers {
    public class HomeController : Controller {
        private readonly RailwaysDBContext dbContext;

        public HomeController(RailwaysDBContext dbContext) {
            this.dbContext = dbContext;
        }
        public IActionResult Index() {
            if (User.Identity != null && User.IsInRole("admin")) {
                return RedirectToAction("Index", "Passenger");
            }
            ViewBag.ArrivalTowns = new SelectList(dbContext.TrainArrivalTown.ToList(), "IdTrainArrivalTown", "TownName");
            ViewBag.DepartureTowns = new SelectList(dbContext.TrainDepartureTown.ToList(), "IdTrainDepartureTown", "TownName");
            return View();
        }

        public async Task<IActionResult> SearchTickets(int idDepartureTown, int idArrivalTown, DateTime dateFrom) {
            try {
                var tickets = await dbContext.Ticket
                    .Include(arrival => arrival.TrainArrivalTownNavigation)
                    .Include(departure => departure.TrainDepartureTownNavigation)
                    .Include(type => type.SeatNavigation.WagonNavigation.WagonTypeNavigation)
                    .Include(train => train.SeatNavigation.WagonNavigation.TrainWagonNavigation.TrainNavigation)
                    .Where(x => x.TicketDate >= dateFrom && x.IdTrainDepartureTown == idDepartureTown && x.IdTrainArrivalTown == idArrivalTown)
                    .ToListAsync();

                var result = tickets
                    .GroupBy(x => new {
                        ArrivalTown = x.TrainArrivalTownNavigation.TownName,
                        DepartureTown = x.TrainDepartureTownNavigation.TownName,
                        x.TicketDate,
                        x.SeatNavigation.WagonNavigation.TrainWagonNavigation.TrainNavigation.TrainName
                    })
                    .Select(x => new {
                        x.Key.DepartureTown,
                        x.Key.ArrivalTown,
                        x.Key.TicketDate,
                        x.Key.TrainName
                    }).ToList();

                var findTicketViewModel = new List<TicketsViewModel>();
                for (var i = 0; i < result.Count; i++) {
                    var item = new TicketsViewModel {
                        ArrivalTown = result[i].ArrivalTown,
                        DepartureTown = result[i].DepartureTown,
                        DepartureTime = result[i].TicketDate,
                        TrainName = result[i].TrainName
                    };
                    findTicketViewModel.Add(item);
                }
                ViewBag.ArrivalTowns = new SelectList(dbContext.TrainArrivalTown.ToList(), "IdTrainArrivalTown", "TownName");
                ViewBag.DepartureTowns = new SelectList(dbContext.TrainDepartureTown.ToList(), "IdTrainDepartureTown", "TownName");
                return View("Index", findTicketViewModel);
            } catch {
                return NotFound();
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Reservation(string departureTown, string arrivalTown, string date, string trainName) {
            var tickets = await dbContext.Ticket
                .Include(type => type.SeatNavigation.WagonNavigation.WagonTypeNavigation)
                .Include(train => train.SeatNavigation.WagonNavigation.TrainWagonNavigation.TrainNavigation)
                .Include(departure => departure.TrainDepartureTownNavigation)
                .Include(arrival => arrival.TrainArrivalTownNavigation)
                .Where(x => x.TrainDepartureTownNavigation.TownName == departureTown
                            && x.TrainArrivalTownNavigation.TownName == arrivalTown
                            && x.SeatNavigation.WagonNavigation.TrainWagonNavigation.TrainNavigation.TrainName == trainName
                            && x.TicketDate == DateTime.Parse(date)
                            && x.SeatNavigation.SeatAvailability)
                .OrderBy(x => x.SeatNavigation.WagonNavigation.WagonNumber).ThenBy(x => x.SeatNavigation.Seat1)
                .ToListAsync();
            return View(tickets);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ReservationConfirmation(int id) {
            try {
                var ticket = await dbContext.Ticket
                    .Include(type => type.SeatNavigation.WagonNavigation.WagonTypeNavigation)
                    .Include(train => train.SeatNavigation.WagonNavigation.TrainWagonNavigation.TrainNavigation)
                    .Include(departure => departure.TrainDepartureTownNavigation)
                    .Include(arrival => arrival.TrainArrivalTownNavigation)
                    .FirstOrDefaultAsync(x => x.IdTicket == id);
                var passenger = await dbContext.Passenger
                    .Include(user => user.UserNavigation)
                    .Include(type => type.PassportTypeNavigation)
                    .FirstOrDefaultAsync(x => x.UserNavigation.UserLogin == User.Identity.Name);
                var reservationViewModel = new ReservationConfirmationViewModel {
                    PassengerFullName = passenger.PassengerFullName,
                    PassportType = passenger.PassportTypeNavigation.Passport,
                    PassportData = passenger.PassengerPassport,
                    DepartureTown = ticket.TrainDepartureTownNavigation.TownName,
                    ArrivalTown = ticket.TrainArrivalTownNavigation.TownName,
                    DepartureTime = ticket.TicketDate,
                    WagonType = ticket.SeatNavigation.WagonNavigation.WagonTypeNavigation.WagonType1,
                    WagonNumber = ticket.SeatNavigation.WagonNavigation.WagonNumber,
                    SeatNumber = ticket.SeatNavigation.Seat1,
                    Price = ticket.SeatNavigation.WagonNavigation.WagonTypeNavigation.WagonPrice,
                    IdTicket = ticket.IdTicket,
                    IdPassenger = passenger.IdPassenger
                };
                ViewBag.Discounts = new SelectList(dbContext.Discount.ToList(), "IdDiscount", "DiscountName");
                return View(reservationViewModel);
            } catch {
                return NotFound();
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ReservationConfirmation(ReservationConfirmationViewModel reservationConfirmationViewModel) {
            try {
                var discount = await dbContext.Discount.FirstOrDefaultAsync(x =>
                    x.IdDiscount == reservationConfirmationViewModel.IdDiscount);
                var ticket = await dbContext.Ticket
                    .Include(seat => seat.SeatNavigation)
                    .SingleOrDefaultAsync(x => x.IdTicket == reservationConfirmationViewModel.IdTicket);
                ticket.SeatNavigation.SeatAvailability = false;
                dbContext.Entry(ticket.SeatNavigation).State = EntityState.Modified;
                var sale = new Sale {
                    IdPassenger = reservationConfirmationViewModel.IdPassenger,
                    IdTicket = reservationConfirmationViewModel.IdTicket,
                    IdDiscount = reservationConfirmationViewModel.IdDiscount,
                    SaleDate = DateTime.Now,
                    TotalPrice = reservationConfirmationViewModel.Price * discount.DiscountMultiply
                };
                await dbContext.AddAsync(sale);
                await dbContext.SaveChangesAsync();
                return RedirectToAction("History");
            } catch {
                return NotFound();
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> History() {
            var sales = await dbContext.Sale
                .Include(passenger => passenger.PassengerNavigation.UserNavigation)
                .Include(ticket => ticket.TicketNavigation.SeatNavigation.WagonNavigation.WagonTypeNavigation)
                .Include(departure => departure.TicketNavigation.TrainDepartureTownNavigation)
                .Include(arrival => arrival.TicketNavigation.TrainArrivalTownNavigation)
                .Where(x => x.PassengerNavigation.UserNavigation.UserLogin == User.Identity.Name)
                .OrderBy(x => x.TicketNavigation.TicketDate)
                .ToListAsync();
            return View(sales);
        }
    }
}