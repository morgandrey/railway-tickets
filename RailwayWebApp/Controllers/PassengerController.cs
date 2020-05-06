using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RailwayWebApp.Data;

namespace RailwayWebApp.Controllers {
    [Authorize(Roles = "admin")]
    public class PassengerController : Controller {
        private readonly RailwaysDBContext dbContext;
        public PassengerController(RailwaysDBContext dbContext) {
            this.dbContext = dbContext;
        }
        [HttpGet]
        public async Task<ActionResult> Index() {
            return View(await dbContext.Passenger
                .Include(usr => usr.UserNavigation)
                .Include(type => type.PassportTypeNavigation)
                .ToListAsync());
        }
    }
}