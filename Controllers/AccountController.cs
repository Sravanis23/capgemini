using Microsoft.AspNetCore.Mvc;
using RailwayReservationMVC.Models;
using RailwayReservationMVC.Data;
using Microsoft.AspNetCore.Http;
using System.Linq;
using BCrypt.Net;
using Microsoft.Extensions.Logging;

namespace RailwayReservationMVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AccountController> _logger;

        public AccountController(ApplicationDbContext context, ILogger<AccountController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Register Page
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: Register User
        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (_context.Users.Any(u => u.Email == model.Email))
            {
                ViewBag.Error = "Email already registered!";
                return View();
            }

            // ✅ Hash the password before saving
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password ?? "");

            var newUser = new User
            {
                Email = model.Email!,
                Username = model.Username!,
                PasswordHash = hashedPassword,
                UserType = "User"
            };

            _context.Users.Add(newUser);
            _context.SaveChanges();  // ✅ Save user to the database

            _logger.LogInformation($"✅ New User Registered: {model.Email}");
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
            _logger.LogInformation($"🔍 Attempting Login: {model.Email}, Type: {model.LoginType}");

            if (model.LoginType == "Admin")
            {
                if (model.Email == "admin@railway.com" && model.Password == "admin123")
                {
                    HttpContext.Session.SetString("UserRole", "Admin");
                    _logger.LogInformation("✅ Admin Login Successful!");
                    return RedirectToAction("Dashboard", "Admin");
                }
            }
            else if (model.LoginType == "User")
            {
                var user = _context.Users.FirstOrDefault(u => u.Email == model.Email);

                // 🔴 Check if user exists
                if (user == null)
                {
                    ViewBag.Error = "Invalid email or password!";
                    _logger.LogWarning("❌ Login Failed: User not found.");
                    return View();
                }

                // 🔴 Check if PasswordHash is empty
                if (string.IsNullOrEmpty(user.PasswordHash))
                {
                    ViewBag.Error = "Password not set. Please reset your password.";
                    _logger.LogWarning("❌ Login Failed: PasswordHash is missing.");
                    return View();
                }

                // 🔴 Check if stored password is a valid BCrypt hash
                if (!user.PasswordHash.StartsWith("$2a$"))
                {
                    ViewBag.Error = "Invalid stored password format.";
                    _logger.LogWarning("❌ Login Failed: PasswordHash format incorrect.");
                    return View();
                }

                // ✅ Verify the password using BCrypt
                if (!BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
                {
                    ViewBag.Error = "Invalid email or password!";
                    _logger.LogWarning("❌ Login Failed: Incorrect password.");
                    return View();
                }

                // ✅ Login successful
                HttpContext.Session.SetString("UserRole", "User");
                HttpContext.Session.SetString("Username", user.Username);
                _logger.LogInformation($"✅ User Login Successful: {user.Username}");
                return RedirectToAction("SearchTrain", "Train");
            }

            ViewBag.Error = "Invalid credentials!";
            _logger.LogWarning("❌ Login Failed: Invalid credentials");
            return View();
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            _logger.LogInformation("🔴 User Logged Out");
            return RedirectToAction("Login");
        }
    }
}
