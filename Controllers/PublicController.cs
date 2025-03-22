using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PadelApp.Data;
using PadelserviceApp.Models;


public class PublicController : Controller
{
    private readonly ApplicationDbContext _context;

    public PublicController(ApplicationDbContext context)
    {
        _context = context;
    }

    [AllowAnonymous] //tillgänglig ej inloggad
    public IActionResult Booking(DateTime? date)
    {   
        //Hämtar alla banor 
        var courts = _context.PadelCourts.ToList();

        //viewData med valt datum.
        ViewData["oneDate"] = date;
        return View(courts);
    }

    [AllowAnonymous] //ej inloggade
    public IActionResult Index(){
        
        return View();
    }


    [HttpGet]
    public IActionResult AvailableTimes(int courtId, DateTime? date)
    {   
        //Hämta banan och dess bokade tider, courtid
        var court = _context.PadelCourts
            .Include(c => c.Booking)
            .FirstOrDefault(c => c.CourtId == courtId);

        //om bana är null
        if (court == null){
            return NotFound();
        }

        //om datum är null
        if (date == null){
            return NotFound();
        }

        //skickas till vyn
        ViewData["Court"] = court;
        ViewData["oneDate"] = date;
       
        var allTimes =  new List<DateTime>(); 

        //for loop för att displaya all tider, går att boka 9-22
        for (int i = 9; i < 23; i++) {   
            //skapat objekt med valt timme och dag.
            var bookingDate = date.Value.Date.AddHours(i);
            //lägger in allt i allTimes
            allTimes.Add(bookingDate);
            }
        

        return View(allTimes);
    }

   
    [Authorize]//måste vara inloggad
    public async Task<IActionResult> BookCourt(int courtId, DateTime date){
        //hämtar banan och dens aktiva bokningar
        var court = await _context.PadelCourts
            .Include(c => c.Booking)
            .FirstOrDefaultAsync(c => c.CourtId == courtId);

        //om banan ej finns
        if (court == null){
             TempData["ErrorMedd"] = "Banan hittas inte";
             return RedirectToAction(nameof(Index));
        }

        //Kollar om tiden är bokad
        if (court.Booking.Any(b => b.BookingTime.Date == date.Date && b.BookingTime.Hour == date.Hour)) {   
            return RedirectToAction(nameof(Index));
        }

        //hämta userId
        var userID = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        var booking = new PadelBooking{ //en bokning
            CourtId = courtId, //bana
            BookingTime = date, //datum
            UserId = userID //användare
        };
        _context.PadelBookings.Add(booking); //lägg till bokning
        await _context.SaveChangesAsync(); //spara 

        return RedirectToAction(nameof(Index));
    }
}