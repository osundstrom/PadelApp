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
        //Hämtar alla banor samt deras tider
        var courts = _context.PadelCourts.Include(c => c.Booking)
            .Where(c => date.HasValue && !c.Booking.Any(b => b.BookingTime.Date == date.Value.Date && b.BookingTime.Hour == date.Value.Hour ))
            .ToList();

        //viewData med valt datum.
        ViewData["oneDate"] = date;
        return View(courts);
    }

    [AllowAnonymous] //ej inloggad
    public IActionResult Index()
    {
        
        return View();
    }

    [HttpGet]
    public IActionResult AvailableTimes(int courtId, DateTime? date)
    {   
        //Visar banan tider.
        var court = _context.PadelCourts
            .Include(c => c.Booking)
            .FirstOrDefault(c => c.CourtId == courtId);

        //om bana är null
        if (court == null)
        {
            return NotFound();
        }

        //om datum är null
        if (date == null)
        {
            return NotFound();
        }

        //skickas till vyn
        ViewData["Court"] = court;
        ViewData["oneDate"] = date;

        var availableTimes = new List<DateTime>();
        var alreadyBooked =  new List<DateTime>();
        var allTimes =  new List<DateTime>();


        //for loop för att displaya all tider. tider gåt att boka 9-22
        for (int i = 9; i < 23; i++) 
        {   
            //skapat objekt med valt timme och dag.
            var bookingDate = date.Value.Date.AddHours(i);
            //lägger in allt i allTimes
            allTimes.Add(bookingDate);
            
            
        }
        

        return View(allTimes);
    }

   
    [Authorize]//måste vara inloggad
    public async Task<IActionResult> BookCourt(int courtId, DateTime date)
    {
        //hämtar banor och tider. 
        var court = await _context.PadelCourts
            .Include(c => c.Booking)
            .FirstOrDefaultAsync(c => c.CourtId == courtId);

        //om banan är null
        if (court == null)
        {
             TempData["ErrorMedd"] = "Banan hittas inte";
             return RedirectToAction(nameof(Index));
        }

        

        //Kollar om tiden är bokad
        if (court.Booking.Any(b => 
        b.BookingTime.Date == date.Date && 
        b.BookingTime.Hour == date.Hour))
        {   
            return RedirectToAction(nameof(Index));
        }

        //hämta userId
        var userID = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        var booking = new PadelBooking{
            CourtId = courtId,
            BookingTime = date,
            UserId = userID
        };

        _context.PadelBookings.Add(booking);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}