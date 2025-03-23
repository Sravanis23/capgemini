using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using RailwayReservationMVC.Models; // ✅ Ensure proper namespace for PassengerViewModel

namespace RailwayReservationMVC.Models.ViewModels
{
    public class ReservationViewModel
    {
        public int TrainID { get; set; }

        [Required]
        public string TrainName { get; set; } = string.Empty; // ✅ Prevent null errors

        [Required]
        public DateTime JourneyDate { get; set; }

        public string? ClassType { get; set; }

        [Required]
        public int QuotaID { get; set; }

        public List<Quota> Quotas { get; set; } = new List<Quota>(); // ✅ Used in View

        public List<string> ClassTypes { get; set; } =  new List<string> // ✅ Used in View
         {
            "Sleeper", "3rd AC", "2nd AC", "1st AC", "Economy", "Business"
        };
        
        public List<Passenger> Passengers { get; set; } = new List<Passenger>(); // ✅ FIXED: Added Property

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}
