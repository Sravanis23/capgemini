using Microsoft.AspNetCore.Mvc;
using RailwayReservationMVC.Data;
using RailwayReservationMVC.Models;
using System.Linq;

namespace RailwayReservationMVC.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Dashboard()
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            var trains = _context.Trains.ToList(); 
            return View(trains);
        }

        [HttpGet]
        public IActionResult AddTrain()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddTrain(Train train)
        {
            _context.Trains.Add(train);
            _context.SaveChanges();
            return RedirectToAction("Dashboard");
        }

        [HttpGet]
        public IActionResult EditTrain(int id)
        {
            var train = _context.Trains.Find(id);
            if (train == null) return NotFound();
            return View(train);
        }

        [HttpPost]
        public IActionResult EditTrain(Train updatedTrain)
        {
            var train = _context.Trains.Find(updatedTrain.TrainID);
            if (train != null)
            {
                train.TrainName = updatedTrain.TrainName;
                train.Source = updatedTrain.Source;
                train.Destination = updatedTrain.Destination;
                train.DepartureTime = updatedTrain.DepartureTime;
                train.ArrivalTime = updatedTrain.ArrivalTime;
                train.Fare = updatedTrain.Fare;
                train.SeatsAvailable = updatedTrain.SeatsAvailable;
                _context.SaveChanges();
            }
            return RedirectToAction("Dashboard");
        }

        public IActionResult DeleteTrain(int id)
        {
            var train = _context.Trains.Find(id);
            if (train != null)
            {
                _context.Trains.Remove(train);
                _context.SaveChanges();
            }
            return RedirectToAction("Dashboard");
        }
    }
}
