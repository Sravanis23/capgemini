using Microsoft.AspNetCore.Mvc;
using RailwayReservationMVC.Models;
using Microsoft.AspNetCore.Http;

namespace RailwayReservationMVC.Controllers
{
    public class AccountController : Controller
    {

        private const string AdminEmail = "admin@railway.com";
        private const string AdminPassword = "admin123";


        private static readonly List<User> Users = new List<User>
        {
            new User { Email = "user1@email.com", Username = "user1", Password = "user123" },
            new User { Email = "user2@email.com", Username = "user2", Password = "user456" }
        };

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginModel model)
        {
            if (model.LoginType == "Admin")
            {
                if (model.Email == AdminEmail && model.Password == AdminPassword)
                {
                    HttpContext.Session.SetString("UserRole", "Admin");
                    return RedirectToAction("Dashboard", "Admin");
                }
            }
            else if (model.LoginType == "User")
            {
                var user = Users.FirstOrDefault(u => u.Email == model.Email && u.Username == model.Username && u.Password == model.Password);
                if (user != null)
                {
                    HttpContext.Session.SetString("UserRole", "User");
                    HttpContext.Session.SetString("Username", user.Username);
                    return RedirectToAction("SearchTrain", "Train");
                }
            }

            ViewBag.Error = "Invalid credentials!";
            return View();
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
