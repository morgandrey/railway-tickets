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
    [Authorize(Roles = "admin")]
    public class TicketController : Controller {
        private readonly RailwaysDBContext dbContext;
        public TicketController(RailwaysDBContext dbContext) {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> Index() {
            var tickets = await dbContext.Ticket
                .Include(departure => departure.TrainDepartureTownNavigation)
                .Include(arrival => arrival.TrainArrivalTownNavigation)
                .Include(train => train.SeatNavigation.WagonNavigation.TrainWagonNavigation.TrainNavigation)
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
                })
                .OrderBy(x => x.TicketDate)
                .ToListAsync();
            var ticketsViewModel = new List<TicketsViewModel>();
            foreach (var ticket in tickets) {
                ticketsViewModel.Add(
                    new TicketsViewModel {
                        TrainName = ticket.TrainName,
                        DepartureTime = ticket.TicketDate,
                        DepartureTown = ticket.DepartureTown,
                        ArrivalTown = ticket.ArrivalTown
                    });
            }
            return View(ticketsViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Details(string departureTown, string arrivalTown, string date, string trainName) {
            var tickets = await dbContext.Ticket
                .Include(train => train.SeatNavigation.WagonNavigation.TrainWagonNavigation.TrainNavigation)
                .Include(type => type.SeatNavigation.WagonNavigation.WagonTypeNavigation)
                .Include(departure => departure.TrainDepartureTownNavigation)
                .Include(arrival => arrival.TrainArrivalTownNavigation)
                .Where(x => x.TrainDepartureTownNavigation.TownName == departureTown
                            && x.TrainArrivalTownNavigation.TownName == arrivalTown
                            && x.SeatNavigation.WagonNavigation.TrainWagonNavigation.TrainNavigation.TrainName == trainName
                            && x.TicketDate == DateTime.Parse(date))
                .OrderBy(x => x.SeatNavigation.WagonNavigation.WagonNumber).ThenBy(x => x.SeatNavigation.Seat1)
                .ToListAsync();
            return View(tickets);
        }

        public IActionResult Create() {
            ViewBag.Trains = new SelectList(dbContext.Train.ToList(), "IdTrain", "TrainName");
            ViewBag.DepartureTowns = new SelectList(dbContext.TrainDepartureTown.ToList(), "IdTrainDepartureTown", "TownName");
            ViewBag.ArrivalTowns = new SelectList(dbContext.TrainArrivalTown.ToList(), "IdTrainArrivalTown", "TownName");
            return View();
        }

        [HttpPost]
        public IActionResult Create(Ticket ticket, int numberOfWagons) {
            if (!string.IsNullOrEmpty(ticket.TicketDate.ToString()) && !string.IsNullOrEmpty(ticket.TicketTravelTime.ToString())) {
                try {
                    Response.Cookies.Append("idTrain",
                        ticket.SeatNavigation.WagonNavigation.TrainWagonNavigation.TrainNavigation.IdTrain.ToString());
                    Response.Cookies.Append("numberOfWagons", numberOfWagons.ToString());
                    Response.Cookies.Append("idDepartureTown",
                        ticket.TrainDepartureTownNavigation.IdTrainDepartureTown.ToString());
                    Response.Cookies.Append("idArrivalTown",
                        ticket.TrainArrivalTownNavigation.IdTrainArrivalTown.ToString());
                    Response.Cookies.Append("ticketDate", ticket.TicketDate.ToString());
                    Response.Cookies.Append("ticketTravelDuration", ticket.TicketTravelTime.ToString());
                } catch {
                    return NotFound();
                }
                return RedirectToAction("CreateWagonInformation");
            }
            ViewBag.Trains = new SelectList(dbContext.Train.ToList(), "IdTrain", "TrainName");
            ViewBag.DepartureTowns = new SelectList(dbContext.TrainDepartureTown.ToList(), "IdTrainDepartureTown", "TownName");
            ViewBag.ArrivalTowns = new SelectList(dbContext.TrainArrivalTown.ToList(), "IdTrainArrivalTown", "TownName");
            return View();
        }

        public IActionResult CreateWagonInformation() {
            ViewBag.NumberOfWagons = Request.Cookies["numberOfWagons"];
            ViewBag.WagonTypes = new SelectList(dbContext.WagonType.ToList(), "IdWagonType", "WagonType1");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateWagonInformation(List<Wagon> wagons, List<int?> numberOfSeats) {
            try {
                await using var transaction = dbContext.Database.BeginTransaction();
                var trainWagons = dbContext.TrainWagon
                    .Where(x => x.IdTrain == int.Parse(Request.Cookies["idTrain"])).ToList();
                var nextTravelCount = 1;

                if (trainWagons.Count > 0) {
                    for (int i = 0; i < trainWagons.Count; i++) {
                        if (nextTravelCount == trainWagons[i].TrainTravelCount) {
                            nextTravelCount++;
                        }
                    }
                }

                var trainWagon = new TrainWagon {
                    IdTrain = int.Parse(Request.Cookies["idTrain"]),
                    TrainTravelCount = nextTravelCount
                };
                await dbContext.TrainWagon.AddAsync(trainWagon);
                await dbContext.SaveChangesAsync();

                var wagonEntities = new List<Wagon>();
                for (int i = 0; i < wagons.Count; i++) {
                    wagonEntities.Add(new Wagon {
                        IdTrainWagon = trainWagon.IdTrainWagon,
                        WagonNumber = int.Parse($"{i + 1}"),
                        IdWagonType = wagons[i].IdWagonType
                    });
                }

                await dbContext.Wagon.AddRangeAsync(wagonEntities);
                await dbContext.SaveChangesAsync();

                var seats = new List<Seat>();
                for (int i = 0; i < wagonEntities.Count; i++) {
                    for (int j = 1; j <= numberOfSeats[i]; j++) {
                        seats.Add(new Seat {
                            IdWagon = wagonEntities[i].IdWagon,
                            Seat1 = j,
                            SeatAvailability = true
                        });
                    }
                }

                await dbContext.Seat.AddRangeAsync(seats);
                await dbContext.SaveChangesAsync();

                var tickets = new List<Ticket>();
                for (int i = 0; i < seats.Count; i++) {
                    tickets.Add(new Ticket {
                        IdSeat = seats[i].IdSeat,
                        IdTrainDepartureTown = int.Parse(Request.Cookies["idDepartureTown"]),
                        IdTrainArrivalTown = int.Parse(Request.Cookies["idArrivalTown"]),
                        TicketDate = DateTime.Parse(Request.Cookies["ticketDate"]),
                        TicketTravelTime = TimeSpan.Parse(Request.Cookies["ticketTravelDuration"])
                    });
                }

                await dbContext.Ticket.AddRangeAsync(tickets);
                await dbContext.SaveChangesAsync();

                await transaction.CommitAsync();
                return RedirectToAction("Index");
            } catch {
                return NotFound();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id) {
            var ticket = await dbContext.Ticket
                .Include(departure => departure.TrainDepartureTownNavigation)
                .Include(arrival => arrival.TrainArrivalTownNavigation)
                .Include(wagon => wagon.SeatNavigation.WagonNavigation)
                .Include(seat => seat.SeatNavigation)
                .FirstOrDefaultAsync(m => m.IdTicket == id);
            if (ticket == null) {
                return NotFound();
            }
            return View(ticket);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id) {
            try {
                await using var transaction1 = dbContext.Database.BeginTransaction();
                var ticket = await dbContext.Ticket
                    .Include(seat => seat.SeatNavigation)
                    .Include(wagon => wagon.SeatNavigation.WagonNavigation)
                    .Include(traionwagon => traionwagon.SeatNavigation.WagonNavigation.TrainWagonNavigation)
                    .FirstOrDefaultAsync(x => x.IdTicket == id);

                dbContext.Entry(ticket.SeatNavigation).State = EntityState.Deleted;
                dbContext.Entry(ticket).State = EntityState.Deleted;
                var seats = dbContext.Seat
                    .Where(x => x.IdWagon == ticket.SeatNavigation.WagonNavigation.IdWagon)
                    .ToList();
                if (seats.Count == 1) {
                    dbContext.Entry(ticket.SeatNavigation.WagonNavigation).State = EntityState.Deleted;
                }
                var wagons = dbContext.Wagon
                    .Where(x => x.TrainWagonNavigation.IdTrainWagon ==
                                ticket.SeatNavigation.WagonNavigation.TrainWagonNavigation.IdTrainWagon)
                    .ToList();
                if (wagons.Count == 1 && seats.Count == 1) {
                    dbContext.Entry(ticket.SeatNavigation.WagonNavigation.TrainWagonNavigation).State =
                        EntityState.Deleted;
                }
                await dbContext.SaveChangesAsync();
                transaction1.Commit();
            } catch {
                return NotFound();
            }
            return RedirectToAction("Index");
        }
    }
}