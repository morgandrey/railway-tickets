using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RailwayWebApp.Models;
using RailwayWebApp.Data;

namespace RailwayWebApp.Controllers {
    public class AuthorizationController : Controller {

        private readonly RailwaysDBContext dbContext;

        public AuthorizationController(RailwaysDBContext dbContext) {
            this.dbContext = dbContext;
        }

        public IActionResult Login() {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(User user, string password) {
            ViewBag.CheckUserPassword = true;
            ViewBag.CheckUserLogin = true;
            var item = dbContext.User.FirstOrDefault(x => x.UserLogin == user.UserLogin);
            if (item != null && !string.IsNullOrEmpty(password)) {
                var salt = Convert.FromBase64String(item.UserSalt);
                var saltedHash = HashPassword.GenerateSaltedHash(Encoding.UTF8.GetBytes(password), salt);
                var hash = Convert.FromBase64String(item.UserHash);
                if (!HashPassword.CompareByteArrays(saltedHash, hash)) {
                    ViewBag.CheckUserPassword = false;
                    return View();
                }
                await Authenticate(item);
                if (item.UserType == "admin") {
                    return RedirectToAction("Passengers", "Admin");
                }
            } else {
                ViewBag.CheckUserLogin = false;
            }
            return View();
        }

        public IActionResult CreateUser() {
            return View();
        }

        [HttpPost]
        public IActionResult CreateUser(User user, string password) {
            if (ModelState.IsValid) {
                var salt = HashPassword.CreateSalt();
                var hash = HashPassword.GenerateSaltedHash(Encoding.UTF8.GetBytes(password), salt);
                try {
                    user.UserSalt = Convert.ToBase64String(salt);
                    user.UserHash = Convert.ToBase64String(hash);
                    Response.Cookies.Append("idSaltCookie", user.UserSalt);
                    Response.Cookies.Append("idHashCookie", user.UserHash);
                    Response.Cookies.Append("LoginCookie", user.UserLogin);
                    return RedirectToAction("CreatePassenger");
                } catch {
                    return NotFound();
                }
            }
            return View();
        }
        public IActionResult CreatePassenger() {
            ViewBag.PassportType = new SelectList(dbContext.PassportType.ToList(), "IdPassportType", "Passport");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreatePassenger(Passenger passenger) {
            if (ModelState.IsValid) {
                try {
                    await using var transaction = dbContext.Database.BeginTransaction();
                    var user = new User {
                        UserLogin = Request.Cookies["LoginCookie"],
                        UserHash = Request.Cookies["idHashCookie"],
                        UserSalt = Request.Cookies["idSaltCookie"],
                        UserType = "passenger"
                    };
                    await dbContext.User.AddAsync(user);
                    await dbContext.SaveChangesAsync();
                    passenger.IdUser = user.IdUser;
                    await dbContext.Passenger.AddAsync(passenger);
                    passenger.UserNavigation = null;
                    await dbContext.SaveChangesAsync();
                    transaction.Commit();
                    return RedirectToAction("Login");
                } catch (Exception ex) {
                    Console.WriteLine(ex.Message);
                    return NotFound();
                }
            }
            ViewBag.PassportType = new SelectList(dbContext.PassportType.ToList(), "IdPassportType", "Passport");
            return View();
        }
        private async Task Authenticate(User user) {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.UserLogin),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.UserType)
            };
            var id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }
    }
}
