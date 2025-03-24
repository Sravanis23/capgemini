using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RailwayReservationMVC.Models
{
    public class Passenger
    {
        [Key]
        public int PassengerID { get; set; }  

        public string Name { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public int Age { get; set; }
        public string PassengerType { get; set; } = string.Empty;

        
        [ForeignKey("Reservation")]
        public int PNRNo { get; set; }

        public virtual Reservation? Reservation { get; set; }

        public virtual User? User { get; set; }
    }
}
