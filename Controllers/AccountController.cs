using Microsoft.AspNetCore.Mvc;
using RailwayReservationMVC.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;

namespace RailwayReservationMVC.Controllers
{
    public class AccountController : Controller
    {
        private static readonly List<User> Users = new List<User>
        {
            new User { Id = 1, Email = "user1@email.com", Username = "user1", Password = "user123", UserType = "User" },
            new User { Id = 2, Email = "user2@email.com", Username = "user2", Password = "user456", UserType = "User" }
        };

        // GET: Register Page
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: Register User
        [HttpPost]
        public IActionResult Register(User model)
        {
            if (Users.Any(u => u.Email == model.Email))
            {
                ViewBag.Error = "Email already registered!";
                return View();
            }

            // Assign new Id (incremental)
            int newId = Users.Count > 0 ? Users.Max(u => u.Id) + 1 : 1;

            Users.Add(new User
            {
                Id = newId,
                Email = model.Email,
                Username = model.Username,
                Password = model.Password,
                UserType = "User"
            });

            Console.WriteLine($"✅ New User Registered: {model.Email}");

            return RedirectToAction("Login");
        }

        // GET: Login Page
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: Login User
        [HttpPost]
        public IActionResult Login(LoginModel model)
        {
            Console.WriteLine($"🔍 Attempting Login: {model.Email}, Type: {model.LoginType}");

            if (model.LoginType == "Admin")
            {
                if (model.Email == "admin@railway.com" && model.Password == "admin123")
                {
                    HttpContext.Session.SetString("UserRole", "Admin");
                    Console.WriteLine("✅ Admin Login Successful!");
                    return RedirectToAction("Dashboard", "Admin");
                }
            }
            else if (model.LoginType == "User")
            {
                var user = Users.FirstOrDefault(u => u.Email == model.Email && u.Password == model.Password);
                if (user != null)
                {
                    HttpContext.Session.SetString("UserRole", "User");
                    HttpContext.Session.SetString("Username", user.Username);
                    Console.WriteLine($"✅ User Login Successful: {user.Username}");
                    return RedirectToAction("SearchTrain", "Train");
                }
            }

            ViewBag.Error = "Invalid credentials!";
            Console.WriteLine("❌ Login Failed: Invalid credentials");
            return View();
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            Console.WriteLine("🔴 User Logged Out");
            return RedirectToAction("Login");
        }
    }
}
