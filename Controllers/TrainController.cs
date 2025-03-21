using Microsoft.AspNetCore.Mvc;
using RailwayReservationMVC.Data;
using RailwayReservationMVC.Models;
using System.Linq;

namespace RailwayReservationMVC.Controllers
{
    public class TrainController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TrainController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult SearchTrain()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SearchTrain(string source, string destination)
        {
            var filteredTrains = _context.Trains
                .Where(t => t.Source.ToLower() == source.ToLower() && t.Destination.ToLower() == destination.ToLower())
                .ToList();

            if (!filteredTrains.Any())
            {
                ViewBag.Message = "No trains found for the given route.";
            }

            return View("TrainResults", filteredTrains);
        }
    }
}
