using System.ComponentModel.DataAnnotations;


namespace PadelserviceApp.Models
{
  public class PadelCourt
  {
    [Key]
    public int CourtId { get; set; } //banans id(unik)

    [Required]
    public string? CourtName { get; set; }  //namn bana

    [Required]
    public string? CourtType { get; set; } //tpy bana

    [Required]
    public ICollection<PadelBooking> Booking { get; set; } = new List<PadelBooking>(); //lista över bokningar, 1 bana knan ha flea bokningar

    
  }
}