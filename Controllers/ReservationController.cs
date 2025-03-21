using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RailwayReservationMVC.Data;
using RailwayReservationMVC.Models;
using RailwayReservationMVC.Services;
using RailwayReservationMVC.Models.ViewModels;
using System;
using System.Linq;

namespace RailwayReservationMVC.Controllers
{
    public class ReservationController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly QuotaService _quotaService;

        public ReservationController(ApplicationDbContext context, QuotaService quotaService)
        {
            _context = context;
            _quotaService = quotaService;
        }

        // ✅ Step 1: Booking Form
        [HttpGet]
        public IActionResult Book(int trainId)
        {
            var train = _context.Trains.Find(trainId);
            if (train == null)
            {
                return NotFound("Train not found.");
            }

            var quotas = _context.Quota
                .Where(q => q.TrainID == trainId)
                .ToList();

            var classTypes = new List<string> { "Sleeper", "3rd AC", "2nd AC", "1st AC", "Economy", "Business" };

            var viewModel = new ReservationViewModel
            {
                TrainID = train.TrainID,
                TrainName = train.TrainName ?? "Unknown Train", // Prevents null error
                Quotas = quotas,
                ClassTypes = classTypes,
                Passengers = new List<Passenger>() // Ensure it's initialized
            };

            return View(viewModel);
        }

        // ✅ Step 2: Process Booking
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Book(ReservationViewModel model)
        {
            if (model.Passengers == null || model.Passengers.Count == 0)
            {
                ModelState.AddModelError("", "At least one passenger must be added.");
                return View(model);
            }

            var train = _context.Trains.Find(model.TrainID);
            if (train == null)
            {
                return NotFound("Train not found.");
            }

            var quota = _context.Quota.FirstOrDefault(q => q.QuotaID == model.QuotaID);
            if (quota == null)
            {
                ModelState.AddModelError("", "Invalid quota selected.");
                return View(model);
            }

            int totalSeatsRequested = model.Passengers.Count;

            // ✅ Check seat availability
            if (!_quotaService.CheckSeatAvailability(model.TrainID, model.QuotaID, totalSeatsRequested))
            {
                ModelState.AddModelError("", "Not enough seats available in the selected quota.");
                return View(model);
            }

            // ✅ Allocate seats
            _quotaService.AllocateSeats(model.TrainID, model.QuotaID, totalSeatsRequested);

            // ✅ Generate Unique PNR
            var reservation = new Reservation
            {
                PNRNo = new Random().Next(100000, 999999), // Use GeneratePNR method to generate a unique PNR
                TrainID = model.TrainID,
                TrainName = train.TrainName ?? "Unknown Train",
                JourneyDate = model.JourneyDate,
                ClassType = model.ClassType ?? "Sleeper",
                QuotaID = model.QuotaID,
                SeatsBooked = totalSeatsRequested,
                Email = model.Email,
                PaymentStatus = "Pending",
                CancellationStatus = "Active"
            };

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    _context.Reservations.Add(reservation);
                    _context.SaveChanges();

                    // Add passengers to the reservation
                    foreach (var passengerVM in model.Passengers)
                    {
                        var passenger = new Passenger
                        {
                            Name = passengerVM.Name,
                            Age = passengerVM.Age,
                            Gender = passengerVM.Gender,
                            PassengerType = passengerVM.PassengerType,
                            ReservationID = reservation.PNRNo // Associate passenger with PNR
                        };

                        _context.Passengers.Add(passenger);
                    }

                    _context.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ModelState.AddModelError("", $"Error saving reservation: {ex.Message}");
                    return View(model);
                }
            }

            return RedirectToAction("BookingSuccess", new { pnrNo = reservation.PNRNo });
        }

        // ✅ Step 3: Booking Success Page
        [HttpGet]
        public IActionResult BookingSuccess(int pnrNo)
        {
            var reservation = _context.Reservations
                .Where(r => r.PNRNo == pnrNo)
                .FirstOrDefault();

            if (reservation == null)
            {
                return NotFound("Reservation not found.");
            }

            var passengers = _context.Passengers
                .Where(p => p.ReservationID == reservation.PNRNo)
                .Select(p => new Passenger
                {
                    Name = p.Name,
                    Age = p.Age,
                    Gender = p.Gender,
                    PassengerType = p.PassengerType
                })
                .ToList();

            var viewModel = new BookingSuccessViewModel
            {
                PNRNo = reservation.PNRNo,
                TrainName = reservation.TrainName ?? "Unknown Train",
                JourneyDate = reservation.JourneyDate,
                ClassType = reservation.ClassType ?? "Sleeper",
                SeatsBooked = reservation.SeatsBooked,
                Email = reservation.Email ?? "Not Provided",
                Passengers = passengers
            };

            return View(viewModel);
        }

        // ✅ Method to generate unique PNR number
        private string GeneratePNR(ReservationViewModel model)
        {
            var random = new Random();
            // PNR format: "TrainID+UserID+Date+RandomNumber"
            string pnr = $"{model.TrainID}{model.Email.GetHashCode()}{model.JourneyDate:yyyyMMdd}{random.Next(1000, 9999)}";
            return pnr;
        }
    }
}
