using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RailwayReservationMVC.Models.ViewModels
{
    public class BookingCancelledViewModel
    {
        public int PNRNo { get; set; }

        [Required]
        public string TrainName { get; set; } = string.Empty; // ✅ Prevent null errors

        [Required]
        public DateTime JourneyDate { get; set; }

        public string? ClassType { get; set; }

        public int SeatsBooked { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string CancellationStatus { get; set; } = string.Empty; 
        public List<Passenger> Passengers { get; set; } = new List<Passenger>(); // ✅ FIXED: Added Property
    }
}
