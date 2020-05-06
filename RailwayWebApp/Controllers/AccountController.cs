using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RailwayWebApp.Models;
using RailwayWebApp.Data;
using RailwayWebApp.ViewModels;

namespace RailwayWebApp.Controllers {
    public class AccountController : Controller {

        private readonly RailwaysDBContext dbContext;

        public AccountController(RailwaysDBContext dbContext) {
            this.dbContext = dbContext;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login() {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
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
                return item.UserType == "passenger" ? RedirectToAction("Index", "Home")
                    : RedirectToAction("Index", "Passenger");
            }
            ViewBag.CheckUserLogin = false;
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register() {
            ViewBag.PassportType = new SelectList(dbContext.PassportType.ToList(), "IdPassportType", "Passport");
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel) {
            if (ModelState.IsValid) {
                try {
                    await using var transaction = dbContext.Database.BeginTransaction();
                    var salt = HashPassword.CreateSalt();
                    var hash = HashPassword.GenerateSaltedHash(Encoding.UTF8.GetBytes(registerViewModel.Password), salt);
                    var user = new User {
                        UserLogin = registerViewModel.Login,
                        UserSalt = Convert.ToBase64String(salt),
                        UserHash = Convert.ToBase64String(hash),
                        UserType = "passenger"
                    };
                    await dbContext.User.AddAsync(user);
                    await dbContext.SaveChangesAsync();

                    var passenger = new Passenger {
                        IdUser = user.IdUser,
                        PassengerFullName = registerViewModel.FullName,
                        PassengerBirthday = registerViewModel.Birthday,
                        IdPassengerPassportType = registerViewModel.IdPassportType,
                        PassengerPassport = registerViewModel.PassportData
                    };
                    await dbContext.Passenger.AddAsync(passenger);
                    await dbContext.SaveChangesAsync();
                    transaction.Commit();
                    await Authenticate(user);
                    return RedirectToAction("Index", "Home");
                } catch {
                    return NotFound();
                }
            }
            ViewBag.PassportType = new SelectList(dbContext.PassportType.ToList(), "IdPassportType", "Passport");
            return View(registerViewModel);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Profile() {
            return View(await dbContext.Passenger
                .Include(type => type.PassportTypeNavigation)
                .FirstAsync(x => x.UserNavigation.UserLogin == User.Identity.Name));
        }

        [Authorize]
        public async Task<IActionResult> Edit(int? id) {
            if (id == null) {
                return NotFound();
            }
            var passenger = await dbContext.Passenger.FindAsync(id);
            if (passenger == null) {
                return NotFound();
            }
            ViewData["PassportTypes"] = new SelectList(dbContext.PassportType.ToList(), "IdPassportType", "Passport");
            return View(passenger);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, Passenger passenger) {
            if (id != passenger.IdPassenger) {
                return NotFound();
            }

            if (ModelState.IsValid) {
                try {
                    dbContext.Entry(passenger).State = EntityState.Modified;
                    await dbContext.SaveChangesAsync();
                } catch {
                    return NotFound();
                }
                return RedirectToAction("Profile");
            }
            ViewData["PassportTypes"] = new SelectList(dbContext.PassportType.ToList(), "IdPassportType", "Passport");
            return View(passenger);
        }

        private async Task Authenticate(User user) {
            var claims = new List<Claim> {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.UserLogin),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.UserType)
            };
            var id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        public async Task<IActionResult> Logout() {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}
