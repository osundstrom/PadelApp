
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PadelApp.Data; 
using PadelserviceApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace PadelApp.Areas.Identity.Pages.Account.Manage
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        private readonly ApplicationDbContext _context;

        public IndexModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

   
  //-------------------------Get--------------------------------------------//

        public List<PadelBooking> Bookings { get; set; }
        public bool AdminCheck { get; set; }

     
        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null){
                return NotFound();
            }

        
        AdminCheck = await _userManager.IsInRoleAsync(user, "Admin");
        if (AdminCheck){
        Bookings = await _context.PadelBookings
            .Include(b => b.Court)  
            .Include(b => b.User)   
            .ToListAsync();
            return Page();
            }
        if(!AdminCheck) {
            Bookings = await _context.PadelBookings
                .Where(b => b.UserId == user.Id)
                .Include(b => b.Court)  
                .Include(b => b.User)   
                .ToListAsync();
                return Page();
            }
            return Page();
        }

  //-------------------------delete--------------------------------------------//
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
            _context.PadelBookings.Remove(Booking);
            await _context.SaveChangesAsync();
            return RedirectToPage();
        }
        if(!AdminCheck){
            var Booking = await _context.PadelBookings
            .Where(b => b.UserId == user.Id)
            .Include(p => p.User)
            .FirstOrDefaultAsync(m => m.BookingId == id);
            _context.PadelBookings.Remove(Booking);
            await _context.SaveChangesAsync();
        }
         return RedirectToPage();

            
        }

    }
}
