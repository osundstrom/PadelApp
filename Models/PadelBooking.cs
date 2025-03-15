
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;


namespace PadelserviceApp.Models
{
  public class PadelBooking
  {
   [Key]
    public int BookingId { get; set; } //unik bokning

    [Required]
    public DateTime BookingTime { get; set; }

    [Required]
     public int CourtId { get; set; }

     [ForeignKey("CourtId")]
     public PadelCourt? Court { get; set; }


     [Required]
      public string? UserId { get; set; }
    
     [ForeignKey("UserId")]
      public IdentityUser? User { get; set; }
  }
}