using System.ComponentModel.DataAnnotations;

namespace RailwayReservationMVC.Models
{
    public class User
    {
        [Key]
        public int UserID { get; set; } 
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;

          public string PasswordHash { get; set; } = string.Empty;
        public string UserType { get; set; } = "User";
    
    }
}
