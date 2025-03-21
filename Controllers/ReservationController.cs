using Microsoft.AspNetCore.Mvc;
using RailwayReservationMVC.Data;
using RailwayReservationMVC.Models;
using RailwayReservationMVC.Services;
using System;
using System.Collections.Generic;
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

        [HttpGet]
        public IActionResult Book(int trainId)
        {
            var train = _context.Trains.Find(trainId);
            if (train == null)
            {
                return NotFound("Train not found.");
            }

            // ✅ Fetch available quotas for the selected train
            var quotas = _context.Quota.Where(q => q.TrainID == trainId).ToList();

            // ✅ Define available class types dynamically
            var classTypes = new List<string> { "Sleeper", "3rd AC", "2nd AC", "1st AC", "Economy", "Business" };

            var bookingModel = new Reservation
            {
                TrainID = trainId,
                TrainName = train.TrainName
            };

            ViewBag.Quotas = quotas;       // ✅ Send quota data to the view
            ViewBag.ClassTypes = classTypes; // ✅ Send class type options to the view
            return View(bookingModel);
        }

        [HttpPost]
        public IActionResult Book(Reservation reservation, List<Passenger> passengers)
        {
            var train = _context.Trains.Find(reservation.TrainID);
            if (train == null)
            {
                return NotFound("Train not found.");
            }

            var quota = _context.Quota.FirstOrDefault(q => q.QuotaID == reservation.QuotaID);
            if (quota == null)
            {
                return BadRequest("Invalid quota selected.");
            }

            if (passengers == null || passengers.Count == 0)
            {
                return BadRequest("At least one passenger must be added.");
            }

            int totalSeatsRequested = passengers.Count;
            if (!_quotaService.CheckSeatAvailability(reservation.TrainID, reservation.QuotaID, totalSeatsRequested))
            {
                return BadRequest("Not enough seats available in the selected quota.");
            }

            _quotaService.AllocateSeats(reservation.TrainID, reservation.QuotaID, totalSeatsRequested);

            // ✅ Assign TrainName before saving
            reservation.TrainName = train.TrainName;
            reservation.SeatsBooked = totalSeatsRequested;

            // ✅ Save reservation first
            _context.Reservations.Add(reservation);
            _context.SaveChanges();

            // ✅ Link passengers to reservation
            foreach (var passenger in passengers)
            {
                passenger.ReservationID = reservation.PNRNo;
                _context.Passengers.Add(passenger);
            }

            try
            {
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                return BadRequest($"Error saving reservation: {ex.Message}");
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
                .Where(p => p.ReservationID == reservation.PNRNo)
                .Select(p => new { p.Name, p.Age, p.Gender })
                .ToList();

            // ✅ Create ViewModel for booking success
            var viewModel = new
            {
                reservation.PNRNo,
                reservation.TrainName,
                reservation.JourneyDate,
                reservation.ClassType,
                reservation.SeatsBooked,
                reservation.Email,
                Passengers = passengers
            };

            return View(viewModel);
        }
    }
}
