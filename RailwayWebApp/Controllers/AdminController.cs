using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using iText.IO.Font;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RailwayWebApp.Data;
using RailwayWebApp.Models;

namespace RailwayWebApp.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        public static string FONT = Directory.GetCurrentDirectory() + "/Fonts/arial.ttf";
        private readonly RailwaysDBContext dbContext;

        public AdminController(RailwaysDBContext dbContext) {
            this.dbContext = dbContext;
        }

        public async Task<ActionResult> Passengers() {
            return View(await dbContext.Passenger
                .Include(usr => usr.UserNavigation)
                .Include(type => type.PassportTypeNavigation)
                .ToListAsync());
        }

        public IActionResult Sales() {
            List<Sale> sales = null;
            if (TempData["entity"] != null) {
                sales = JsonConvert.DeserializeObject<List<Sale>>(TempData["entity"].ToString());
            }
            return View(sales);
        }

        [HttpPost]
        public async Task<IActionResult> ShowSales(DateTime dateFrom, DateTime dateTo) {
            try {
                var sales = new List<Sale>(await dbContext.Sale
                    .Include(pass => pass.PassengerNavigation)
                    .Include(user => user.PassengerNavigation.UserNavigation)
                    .Include(ticket => ticket.TicketNavigation)
                    .Include(departure => departure.TicketNavigation.TrainDepartureTownNavigation)
                    .Include(arrival => arrival.TicketNavigation.TrainArrivalTownNavigation)
                    .Where(x => x.SaleDate >= dateFrom && x.SaleDate <= dateTo)
                    .ToListAsync());
                if (sales.Count > 0) {
                    TempData["entity"] = JsonConvert.SerializeObject(sales, Formatting.Indented,
                        new JsonSerializerSettings {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        });
                }
                Response.Cookies.Append("dateFrom", dateFrom.ToString("d"));
                Response.Cookies.Append("dateTo", dateTo.ToString("d"));
            } catch {
                return NotFound();
            }
            return RedirectToAction("Sales");
        }

        public IActionResult CreateSaleReport() {
            try {
                var dateFrom = Request.Cookies["dateFrom"];
                var dateTo = Request.Cookies["dateTo"];
                var sales = new List<Sale>(dbContext.Sale
                        .Include(pass => pass.PassengerNavigation)
                        .Include(user => user.PassengerNavigation.UserNavigation)
                        .Include(ticket => ticket.TicketNavigation)
                        .Include(departure => departure.TicketNavigation.TrainDepartureTownNavigation)
                        .Include(arrival => arrival.TicketNavigation.TrainArrivalTownNavigation)
                        .Where(x => x.SaleDate >= DateTime.Parse(dateFrom) && x.SaleDate <= DateTime.Parse(dateTo)))
                    .ToList();
                var dest = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/Отчёт по продажам.pdf";
                var file = new FileInfo(dest);
                file.Directory.Create();
                var pdf = new PdfDocument(new PdfWriter(dest));
                var document = new Document(pdf, PageSize.A4.Rotate());
                var font = PdfFontFactory.CreateFont(FONT, PdfEncodings.IDENTITY_H);

                float[] columnWidths = { 1, 5, 5, 5 };
                var table = new Table(UnitValue.CreatePercentArray(columnWidths));

                var cell = new Cell(1, 4)
                    .Add(new Paragraph($"Отчёт по продажам {dateFrom} - {dateTo}"))
                    .SetFont(font)
                    .SetFontSize(13)
                    .SetFontColor(DeviceGray.WHITE)
                    .SetBackgroundColor(DeviceGray.BLACK)
                    .SetTextAlignment(TextAlignment.CENTER);
                table.AddHeaderCell(cell);

                Cell[] headerFooter =
                {
                    new Cell().SetFont(font).SetBackgroundColor(new DeviceGray(0.75f)).Add(new Paragraph("#")),
                    new Cell().SetFont(font).SetBackgroundColor(new DeviceGray(0.75f)).Add(new Paragraph("Номер билета")),
                    new Cell().SetFont(font).SetBackgroundColor(new DeviceGray(0.75f)).Add(new Paragraph("Дата продажи")),
                    new Cell().SetFont(font).SetBackgroundColor(new DeviceGray(0.75f)).Add(new Paragraph("Цена"))
                };

                foreach (var hfCell in headerFooter) {
                    table.AddHeaderCell(hfCell);
                }

                for (int i = 0; i < sales.Count; i++) {
                    table.AddCell(new Cell().SetTextAlignment(TextAlignment.CENTER).SetFont(font).Add(new Paragraph((i + 1).ToString())));
                    table.AddCell(new Cell().SetTextAlignment(TextAlignment.CENTER).SetFont(font).Add(new Paragraph(sales[i].IdTicket.ToString())));
                    table.AddCell(new Cell().SetTextAlignment(TextAlignment.CENTER).SetFont(font).Add(new Paragraph(sales[i].SaleDate.ToString("g"))));
                    table.AddCell(new Cell().SetTextAlignment(TextAlignment.CENTER).SetFont(font).Add(new Paragraph(sales[i].TotalPrice.ToString(CultureInfo.InvariantCulture))));
                    if (i == sales.Count - 1) {
                        table.AddCell(new Cell());
                        table.AddCell(new Cell());
                        table.AddCell(new Cell().SetBold().SetTextAlignment(TextAlignment.CENTER).SetFont(font).Add(new Paragraph("Итог:")));
                        table.AddCell(new Cell().SetTextAlignment(TextAlignment.CENTER).SetFont(font).Add(new Paragraph($"{sales.Sum(x => x.TotalPrice)}")));
                    }
                }
                document.Add(table);

                document.Close();
            } catch {
                return NotFound();
            }
            return RedirectToAction("Sales");
        }

        public async Task<IActionResult> Tickets() {
            return View(await dbContext.Ticket
                .Include(departure => departure.TrainDepartureTownNavigation)
                .Include(arrival => arrival.TrainArrivalTownNavigation)
                .Include(seat => seat.SeatNavigation)
                .Include(wagon => wagon.SeatNavigation.WagonNavigation)
                .Include(trainWagon => trainWagon.SeatNavigation.WagonNavigation.TrainWagonNavigation)
                .Include(train => train.SeatNavigation.WagonNavigation.TrainWagonNavigation.TrainNavigation)
                .Include(type => type.SeatNavigation.WagonNavigation.WagonTypeNavigation)
                .OrderBy(x => x.TicketDate)
                .ToListAsync());
        }

        public IActionResult CreateNewTickets() {
            ViewBag.Trains = new SelectList(dbContext.Train.ToList(), "IdTrain", "TrainName");
            ViewBag.DepartureTowns = new SelectList(dbContext.TrainDepartureTown.ToList(), "IdTrainDepartureTown", "TownName");
            ViewBag.ArrivalTowns = new SelectList(dbContext.TrainArrivalTown.ToList(), "IdTrainArrivalTown", "TownName");
            return View();
        }

        [HttpPost]
        public IActionResult CreateNewTickets(Ticket ticket, int numberOfWagons) {
            try {
                Response.Cookies.Append("idTrain",
                    ticket.SeatNavigation.WagonNavigation.TrainWagonNavigation.TrainNavigation.IdTrain.ToString());
                Response.Cookies.Append("numberOfWagons", numberOfWagons.ToString());
                Response.Cookies.Append("idDepartureTown",
                    ticket.TrainDepartureTownNavigation.IdTrainDepartureTown.ToString());
                Response.Cookies.Append("idArrivalTown",
                    ticket.TrainArrivalTownNavigation.IdTrainArrivalTown.ToString());
                Response.Cookies.Append("ticketDate", ticket.TicketDate.ToString("g"));
                Response.Cookies.Append("ticketTravelDuration", ticket.TicketTravelTime.ToString("g"));
            } catch {
                return NotFound();
            }

            return RedirectToAction("TicketWagonInformation");
        }

        public IActionResult TicketWagonInformation() {
            ViewBag.NumberOfWagons = Request.Cookies["numberOfWagons"];
            ViewBag.WagonTypes = new SelectList(dbContext.WagonType.ToList(), "IdWagonType", "WagonType1");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> TicketWagonInformation(List<Wagon> wagons, List<int> numberOfSeats) {
            try {
                await using var transaction = dbContext.Database.BeginTransaction();
                var trainWagons = dbContext.TrainWagon.Where(x => x.IdTrain == int.Parse(Request.Cookies["idTrain"])).ToList();
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
            } catch {
                return NotFound();
            }

            return RedirectToAction("Tickets");
        }

        [HttpGet]
        public async Task<IActionResult> DeleteTicket(int id) {
            var ticket = await dbContext.Ticket
                .FirstOrDefaultAsync(m => m.IdTicket == id);
            if (ticket == null) {
                return NotFound();
            }
            return View(ticket);
        }

        [HttpPost, ActionName("DeleteTicket")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id) {
            var ticket = await dbContext.Ticket.FindAsync(id);
            dbContext.Ticket.Remove(ticket);
            await dbContext.SaveChangesAsync();
            return RedirectToAction("Tickets");
        }

    }
}