using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RailwayWebApp.Data;
using RailwayWebApp.Models;

namespace RailwayWebApp.Controllers {
    [Authorize(Roles = "admin")]
    public class WagonController : Controller {
        private readonly RailwaysDBContext dbContext;
        public WagonController(RailwaysDBContext dbContext) {
            this.dbContext = dbContext;
        }
        [HttpGet]
        public async Task<IActionResult> Index() {
            return View(await dbContext.WagonType.ToListAsync());
        }

        public ActionResult Create() {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(WagonType wagonType) {
            try {
                if (ModelState.IsValid) {
                    await dbContext.AddAsync(wagonType);
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
                var wagonTypeItem = await dbContext.WagonType.FindAsync(id);
                return View(wagonTypeItem);
            } catch {
                return NotFound();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(WagonType wagonType) {
            try {
                if (ModelState.IsValid) {
                    dbContext.Entry(wagonType).State = EntityState.Modified;
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