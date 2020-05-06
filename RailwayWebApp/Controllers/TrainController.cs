using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RailwayWebApp.Data;
using RailwayWebApp.Models;

namespace RailwayWebApp.Controllers {
    [Authorize(Roles = "admin")]
    public class TrainController : Controller {
        private readonly RailwaysDBContext dbContext;
        public TrainController(RailwaysDBContext dbContext) {
            this.dbContext = dbContext;
        }

        public async Task<IActionResult> Index() {
            return View(await dbContext.Train.ToListAsync());
        }

        public ActionResult Create() {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Train train) {
            try {
                if (ModelState.IsValid) {
                    await dbContext.AddAsync(train);
                    await dbContext.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                return View();
            } catch {
                return NotFound();
            }
        }

        public async Task<ActionResult> Edit(int id) {
            try {
                var trainItem = await dbContext.Train.FindAsync(id);
                return View(trainItem);
            } catch {
                return NotFound();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Train train) {
            try {
                if (ModelState.IsValid) {
                    dbContext.Entry(train).State = EntityState.Modified;
                    await dbContext.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                return View();
            } catch {
                return NotFound();
            }
        }
    }
}