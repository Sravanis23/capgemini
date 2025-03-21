using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RailwayReservationMVC.Models
{
    public class Train
    {
        [Key]
        public int TrainID { get; set; }

        public string TrainName { get; set; } = string.Empty;
        public string Source { get; set; } = string.Empty;
        public string Destination { get; set; } = string.Empty;
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public decimal Fare { get; set; }
        public int SeatsAvailable { get; set; }

        // ✅ Navigation property for Quotas (One Train → Many Quotas)
        public virtual List<Quota> Quotas { get; set; } = new List<Quota>();

        // ✅ Navigation property for Reservations (One Train → Many Reservations)
        public virtual List<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}
