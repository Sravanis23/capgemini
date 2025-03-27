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

/*************  ✨ Codeium Command ⭐  *************/
        /// <summary>
        /// Constructor for ReservationController. 
        /// </summary>
/******  7a50db61-1e22-4359-835e-b232dbbaa9ff  *******/
        public ReservationController(ApplicationDbContext context, QuotaService quotaService)
        {
            _context = context;
            _quotaService = quotaService;
        }

        [HttpGet]
        public IActionResult Book(int trainId)
        {
            var train = _context.Trains.Find(trainId);
            if (train == null)
            {
                return NotFound("Train not found.");
            }

            var quotas = _context.Quota
                .Where(q => q.TrainID == trainId && q.SeatsAvailable > 0)
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

            _quotaService.AllocateSeats(model.TrainID, model.QuotaID, totalSeatsRequested);

            int pnrNo = GeneratePNR();


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
 
                    _context.Reservations.Add(reservation);
                    _context.SaveChanges();

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

                  quota.SeatsAvailable -= totalSeatsRequested;
            _context.Quota.Update(quota);
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
        .Include(r => r.Passengers)
        .FirstOrDefault(r => r.PNRNo == pnrNo);

    if (reservation == null)
    {
        ViewBag.ErrorMessage = "Reservation not found!";
        return View();
    }
    if (reservation.JourneyDate == null)
    {
        reservation.JourneyDate = DateTime.MinValue; 
    }

    return View("ReservationStatus", reservation);
}
[HttpPost]
public IActionResult MakePayment(int pnrNo)
{
    var reservation = _context.Reservations.FirstOrDefault(r => r.PNRNo == pnrNo);
    
    if (reservation == null)
    {
        return NotFound("Reservation not found.");
    }

    reservation.PaymentStatus = "Completed";

    _context.SaveChanges();

    TempData["Message"] = "Payment successful!";

    return RedirectToAction("CheckStatus", new { pnrNo = pnrNo });
}


public IActionResult UpdateTotalFare(int reservationId)
{
    var reservation = _context.Reservations.FirstOrDefault(r => r.PNRNo == reservationId);
    if (reservation == null)
    {
        return NotFound("Reservation not found.");
    }

    var train = _context.Trains.FirstOrDefault(t => t.TrainID == reservation.TrainID);
    if (train == null)
    {
        return NotFound("Train not found.");
    }

    reservation.TotalFare = train.Fare * reservation.SeatsBooked;
 
    _context.Reservations.Update(reservation);
    _context.SaveChanges();

    return RedirectToAction("CheckStatus", new { pnrNo = reservationId });
}


[HttpPost]
public IActionResult Create(Reservation reservation)
{
    var train = _context.Trains.FirstOrDefault(t => t.TrainID == reservation.TrainID);
    if (train == null)
    {
        return NotFound("Train not found.");
    }

    reservation.TotalFare = train.Fare * reservation.SeatsBooked;

    _context.Reservations.Add(reservation);
    _context.SaveChanges();

    return RedirectToAction("CheckStatus", new { pnrNo = reservation.PNRNo });
}


    }
}
