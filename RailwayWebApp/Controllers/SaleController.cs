using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using Microsoft.EntityFrameworkCore;
using RailwayWebApp.Data;
using RailwayWebApp.Models;

namespace RailwayWebApp.Controllers {
    [Authorize(Roles = "admin")]
    public class SaleController : Controller {
        public static string FONT = Directory.GetCurrentDirectory() + "/wwwroot/arial.ttf";
        private readonly RailwaysDBContext dbContext;
        public SaleController(RailwaysDBContext dbContext) {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> Index() {
            var dateFrom = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd");
            var dateTo = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");
            ViewBag.DateFrom = dateFrom;
            ViewBag.DateTo = dateTo;
            Response.Cookies.Append("dateFrom", dateFrom.ToString(CultureInfo.InvariantCulture));
            Response.Cookies.Append("dateTo", dateTo.ToString(CultureInfo.InvariantCulture));
            return View(await dbContext.Sale
                .Include(pass => pass.PassengerNavigation)
                .Include(user => user.PassengerNavigation.UserNavigation)
                .Include(ticket => ticket.TicketNavigation)
                .Include(departure => departure.TicketNavigation.TrainDepartureTownNavigation)
                .Include(arrival => arrival.TicketNavigation.TrainArrivalTownNavigation)
                .Where(x => x.SaleDate >= DateTime.Parse(dateFrom) && x.SaleDate <= DateTime.Parse(dateTo))
                .ToListAsync());
        }

        public async Task<IActionResult> ShowSales(DateTime dateFrom, DateTime dateTo) {
            try {
                var sales = new List<Sale>(await dbContext.Sale
                    .Include(pass => pass.PassengerNavigation)
                    .Include(user => user.PassengerNavigation.UserNavigation)
                    .Include(ticket => ticket.TicketNavigation)
                    .Include(departure => departure.TicketNavigation.TrainDepartureTownNavigation)
                    .Include(arrival => arrival.TicketNavigation.TrainArrivalTownNavigation)
                    .Where(x => x.SaleDate >= dateFrom && x.SaleDate <= dateTo)
                    .OrderBy(x => x.SaleDate)
                    .ToListAsync());
                if (sales.Count > 0) {
                    Response.Cookies.Append("dateFrom", dateFrom.ToString(CultureInfo.InvariantCulture));
                    Response.Cookies.Append("dateTo", dateTo.ToString(CultureInfo.InvariantCulture));
                }
                ViewBag.DateFrom = dateFrom.ToString("yyyy-MM-dd");
                ViewBag.DateTo = dateTo.ToString("yyyy-MM-dd");
                return View("Index", sales);
            } catch {
                return NotFound();
            }
        }
        public JsonResult CreateSaleReport() {
            try {
                var dateFrom = DateTime.Parse(Request.Cookies["dateFrom"]);
                var dateTo = DateTime.Parse(Request.Cookies["dateTo"]);
                var sales = new List<Sale>(dbContext.Sale
                        .Include(pass => pass.PassengerNavigation)
                        .Include(user => user.PassengerNavigation.UserNavigation)
                        .Include(ticket => ticket.TicketNavigation)
                        .Include(departure => departure.TicketNavigation.TrainDepartureTownNavigation)
                        .Include(arrival => arrival.TicketNavigation.TrainArrivalTownNavigation)
                        .Where(x => x.SaleDate >= dateFrom && x.SaleDate <= dateTo))
                    .ToList();
                var dest = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + $"/Отчёт по продажам от {dateFrom:yyyy-MM-dd}.pdf";
                var file = new FileInfo(dest);
                file.Directory.Create();
                var pdf = new PdfDocument(new PdfWriter(dest));
                var document = new Document(pdf, PageSize.A4.Rotate());
                var font = PdfFontFactory.CreateFont(FONT, PdfEncodings.IDENTITY_H);

                float[] columnWidths = { 1, 5, 5, 5 };
                var table = new Table(UnitValue.CreatePercentArray(columnWidths));

                var cell = new Cell(1, 4)
                    .Add(new Paragraph($"Отчёт по продажам {dateFrom:yyyy-MM-dd} - {dateTo:yyyy-MM-dd}"))
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
                var process = new Process {
                    StartInfo = new ProcessStartInfo(dest) {
                        UseShellExecute = true
                    }
                };
                process.Start();
                document.Close();
            } catch {
                return new JsonResult("Не удалось сохранить отчёт");
            }
            return new JsonResult("Отчёт сохранен");
        }
    }
}