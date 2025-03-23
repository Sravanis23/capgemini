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
                TrainName = train.TrainName ?? "Unknown Train",
                Quotas = quotas,
                ClassTypes = classTypes,
                Passengers = new List<Passenger>()
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

            // ✅ Generate Unique PNR Automatically
            int pnrNo = GeneratePNR();

            // ✅ Create and save reservation
            var reservation = new Reservation
            {
                PNRNo = pnrNo,
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
                    // ✅ Add reservation first
                    
                    _context.Reservations.Add(reservation);
                    _context.SaveChanges();

                    // ✅ Add passengers after saving reservation to get valid PNRNo
                    foreach (var passengerVM in model.Passengers)
                    {
                        var passenger = new Passenger
                        {
                            Name = passengerVM.Name,
                            Age = passengerVM.Age,
                            Gender = passengerVM.Gender,
                            PassengerType = passengerVM.PassengerType,
                            PNRNo = reservation.PNRNo
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
                .Where(p => p.PNRNo == reservation.PNRNo)
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

        // ✅ Generate unique PNR number automatically (using random 6 digits)
        private int GeneratePNR()
        {
            var random = new Random();

            // ✅ Generate 6-digit random number for PNR
            int pnr = random.Next(100000, 999999);

            // ✅ Ensure PNR is unique (regenerate if necessary)
            while (_context.Reservations.Any(r => r.PNRNo == pnr))
            {
                pnr = random.Next(100000, 999999);
            }

            return pnr;
        }

        [HttpPost]
[ValidateAntiForgeryToken]
public IActionResult Cancel(int pnrNo)
{
    var reservation = _context.Reservations.FirstOrDefault(r => r.PNRNo == pnrNo);
    
    if (reservation == null)
    {
        return NotFound("Reservation not found.");
    }

    if (reservation.CancellationStatus.ToLower() == "cancelled")
    {
        return BadRequest("Reservation is already cancelled.");
    }

    using (var transaction = _context.Database.BeginTransaction())
    {
        try
        {
            // ✅ Mark the reservation as cancelled
            reservation.CancellationStatus = "Cancelled";
            _context.Reservations.Update(reservation);

            // ✅ Free up the allocated seats
            var quota = _context.Quota.FirstOrDefault(q => q.QuotaID == reservation.QuotaID);
            if (quota != null)
            {
                quota.SeatsAvailable += reservation.SeatsBooked;
                _context.Quota.Update(quota);
            }

            _context.SaveChanges();
            transaction.Commit();
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            ModelState.AddModelError("", $"Error cancelling reservation: {ex.Message}");
            return RedirectToAction("BookingDetails", new { pnrNo });
        }
    }

    return RedirectToAction("BookingCancelled", new { pnrNo });
}

[HttpGet]
public IActionResult BookingCancelled(int pnrNo)
{
    return View(pnrNo);
}


[HttpGet]
public IActionResult CheckStatus()
{
    return View();
}

[HttpPost]
public IActionResult CheckStatus(int pnrNo)
{
    var reservation = _context.Reservations
        .Include(r => r.Passengers) // Include related passengers
        .FirstOrDefault(r => r.PNRNo == pnrNo);

    if (reservation == null)
    {
        ViewBag.ErrorMessage = "Reservation not found!";
        return View();
    }

    return View("ReservationStatus", reservation);
}


    }
}
