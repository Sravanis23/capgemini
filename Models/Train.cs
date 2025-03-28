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
        public DateTime DepartureTime { get; set; } = DateTime.Now;
        public DateTime ArrivalTime { get; set; }
        public decimal Fare { get; set; }
        public int SeatsAvailable { get; set; }

        public virtual List<Quota> Quotas { get; set; } = new List<Quota>();

        public virtual List<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}
