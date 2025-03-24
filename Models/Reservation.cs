using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RailwayReservationMVC.Models
{
    public class Reservation
    {
        [Key]
        public int PNRNo { get; set; } 
        
        [ForeignKey("Train")]
        public int TrainID { get; set; }

        [ForeignKey("Quota")]
        public int QuotaID { get; set; } 

        public DateTime JourneyDate { get; set; }
        public string ClassType { get; set; } = string.Empty;
        public decimal TotalFare { get; set; }
        public string PaymentStatus { get; set; } = string.Empty;
        public string CancellationStatus { get; set; } = string.Empty;
        public string TrainName { get; set; } = string.Empty;
        public int SeatsBooked { get; set; }
        public string Email { get; set; } = string.Empty;

        public virtual List<Passenger> Passengers { get; set; } = new List<Passenger>();
        public virtual Train? Train { get; set; }
        public virtual Quota? Quota { get; set; }
    }
}
