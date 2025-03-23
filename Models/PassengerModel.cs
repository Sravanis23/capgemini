using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RailwayReservationMVC.Models
{
    public class Passenger
    {
        [Key]
        public int PassengerID { get; set; }  // Primary Key

        public string Name { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public int Age { get; set; }
        public string PassengerType { get; set; } = string.Empty;

        // ✅ Add ReservationID as a Foreign Key
        [ForeignKey("Reservation")]
        public int PNRNo { get; set; }

        // ✅ Navigation property for Reservation
        public virtual Reservation? Reservation { get; set; }

        // ✅ Navigation property for User (for email retrieval)
        public virtual User? User { get; set; }
    }
}
