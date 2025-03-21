using System.ComponentModel.DataAnnotations;

namespace RailwayReservationMVC.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; } // Primary Key
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
