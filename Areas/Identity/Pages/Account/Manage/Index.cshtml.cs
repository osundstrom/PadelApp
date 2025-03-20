

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PadelApp.Data; 
using PadelserviceApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace PadelApp.Areas.Identity.Pages.Account.Manage
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        private readonly ApplicationDbContext _context;
        public List<PadelBooking> Bookings { get; set; } = new List<PadelBooking>();

        //private readonly ILogger<IndexModel> _logger;
         public PadelBooking? PadelBooking { get; set; }
        public IndexModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ApplicationDbContext context)
            
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
             PadelBooking = new PadelBooking();
             
        }

   
  //-------------------------Get(båda)--------------------------------------------//

        public bool AdminCheck { get; set; }

         DateTime sortDate = DateTime.Today;
     
        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null){
                return NotFound();
            }

        
        AdminCheck = await _userManager.IsInRoleAsync(user, "Admin");
        if (AdminCheck){
        Bookings = await _context.PadelBookings //bara tider från idag och framåt
            .Where(b => b.BookingTime >= sortDate) 
            .Include(b => b.Court)  
            .Include(b => b.User)   
            .ToListAsync();
            return Page();
            }
        if(!AdminCheck) {
            Bookings = await _context.PadelBookings
                .Where(b => b.UserId == user.Id  && b.BookingTime >= sortDate) //bara tider från idag och framåt och samma användare
                .Include(b => b.Court)  
                .Include(b => b.User)   
                .ToListAsync();
                return Page();
            }
            return Page();
        }

  //-------------------------delete(båda)--------------------------------------------//
        public async Task<IActionResult> OnPostDeleteAsync(int? id){

        var user = await _userManager.GetUserAsync(User);
           
        if (user == null){
            return NotFound();
        }

        AdminCheck = await _userManager.IsInRoleAsync(user, "Admin");
        if(AdminCheck) {
            var Booking = await _context.PadelBookings
            .Include(p => p.User)
            .FirstOrDefaultAsync(m => m.BookingId == id);

            if(Booking == null) {
                return NotFound();
            }
            _context.PadelBookings.Remove(Booking);
            await _context.SaveChangesAsync();
            return RedirectToPage();
        }


        if(!AdminCheck){
            var Booking = await _context.PadelBookings
            .Where(b => b.UserId == user.Id)
            .Include(p => p.User)
            .FirstOrDefaultAsync(m => m.BookingId == id);

            if(Booking == null) {
                return NotFound();
            }
            _context.PadelBookings.Remove(Booking);
            await _context.SaveChangesAsync();
        }
         return RedirectToPage();

            
        }

    }
}
