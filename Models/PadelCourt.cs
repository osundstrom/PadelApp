using System.ComponentModel.DataAnnotations;


namespace PadelserviceApp.Models
{
  public class PadelCourt
  {
    [Key]
    public int CourtId { get; set; }

    [Required]
    public string? CourtName { get; set; } 

    [Required]
    public string? CourtType { get; set; }

    [Required]
    public ICollection<PadelBooking> Booking { get; set; } = new List<PadelBooking>();

    
  }
}