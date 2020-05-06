using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RailwayWebApp.Data;
using RailwayWebApp.Models;

namespace RailwayWebApp.Controllers {
    [Authorize(Roles = "admin")]
    public class TownController : Controller {
        private readonly RailwaysDBContext dbContext;
        public TownController(RailwaysDBContext dbContext) {
            this.dbContext = dbContext;
        }

        public async Task<IActionResult> Index() {
            return View(await dbContext.TrainArrivalTown.ToListAsync());
        }

        public ActionResult Create() {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(TrainArrivalTown town) {
            try {
                if (ModelState.IsValid) {
                    var transaction = dbContext.Database.BeginTransaction();
                    await dbContext.TrainArrivalTown.AddAsync(town);
                    var departureTown = new TrainDepartureTown {
                        TownName = town.TownName
                    };
                    await dbContext.TrainDepartureTown.AddAsync(departureTown);
                    await dbContext.SaveChangesAsync();
                    transaction.Commit();
                    return RedirectToAction("Index");
                }
                return View();
            } catch {
                return NotFound();
            }
        }

        public async Task<ActionResult> Edit(int id) {
            try {
                var arrivalTownItem = await dbContext.TrainArrivalTown.FindAsync(id);
                return View(arrivalTownItem);
            } catch {
                return NotFound();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(TrainArrivalTown arrivalTown) {
            try {
                if (ModelState.IsValid) {
                    var transaction = dbContext.Database.BeginTransaction();
                    var arrivalTownNameBefore =
                        await dbContext.TrainArrivalTown.FindAsync(arrivalTown.IdTrainArrivalTown);
                    var departureTown = await dbContext.TrainDepartureTown
                        .FirstOrDefaultAsync(x => x.TownName == arrivalTownNameBefore.TownName);
                    arrivalTownNameBefore.TownName = arrivalTown.TownName;
                    departureTown.TownName = arrivalTown.TownName;
                    dbContext.Entry(arrivalTownNameBefore).State = EntityState.Modified;
                    dbContext.Entry(departureTown).State = EntityState.Modified;
                    await dbContext.SaveChangesAsync();
                    transaction.Commit();
                    return RedirectToAction("Index");
                }
                return View();
            } catch {
                return NotFound();
            }
        }
    }
}