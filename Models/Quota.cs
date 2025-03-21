using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RailwayReservationMVC.Models
{
    public class Quota
    {
        [Key]
        public int QuotaID { get; set; }

        [ForeignKey("Train")]
        public int TrainID { get; set; }

        public string QuotaType { get; set; } = string.Empty;
        public int SeatsAvailable { get; set; }

        // ✅ Navigation property to link Quota with Train
        public virtual Train? Train { get; set; }

        // ✅ Add this line: Navigation property for Reservations
        public virtual List<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}
