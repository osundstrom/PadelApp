using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PadelApp.Data;
using PadelserviceApp.Models;

namespace PadelApp.Controllers
{   
    [Authorize]
    public class BookingController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookingController(ApplicationDbContext context)
        {
            _context = context;
        }

//---------------------------------------------------------------------------------------------//


        // GET: Booking
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(){

       


            var applicationDbContext = _context.PadelBookings
            .Include(p => p.Court)
            .Include(p => p.User);

            var bookings = await applicationDbContext.ToListAsync();

            return View(bookings);
        }
        
//-----------------------------------Edit använder-------------------------------------------//
        // GET: Booking/Edit/5
        [Authorize(Roles = "Admin")] //måset admin
        public async Task<IActionResult> Edit(int? id){
        
        //bokning med id
        var padelBooking = await _context.PadelBookings.FindAsync(id);

        if(padelBooking == null) {
            return NotFound();
        }
   
        //Alla banor hämtas
        var courts = _context.PadelCourts
        .Select(c => new {
            CourtId = c.CourtId, //banans id
            CourtDisplay = $"{c.CourtName} ({c.CourtType})" //banans namn och typ
        })
        .ToList();
            
            //sätter  viewData
            ViewData["CourtId"] = new SelectList(courts, "CourtId", "CourtDisplay", padelBooking.CourtId);
            ViewData["UserId"] = padelBooking.UserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            return View(padelBooking);
        }

        // POST: Booking/Edit/5
        [Authorize(Roles = "Admin")] //måste admin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BookingId,BookingTime,CourtId,UserId")] PadelBooking padelBooking)
        {
            if (id != padelBooking.BookingId)
            {
                return NotFound();
            }

            var existingBooking = await _context.PadelBookings.AsNoTracking().FirstOrDefaultAsync(b => b.BookingId == id);
        if (existingBooking == null)
        {
        return NotFound();
     }

    
    padelBooking.UserId = existingBooking.UserId;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(padelBooking);
                     _context.Entry(padelBooking).Property(b => b.BookingTime).IsModified = true; 
                    _context.Entry(padelBooking).Property(b => b.CourtId).IsModified = true;  
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PadelBookingExists(padelBooking.BookingId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return Redirect("/Identity/Account/Manage");  
            }
            ViewData["CourtId"] = new SelectList(_context.PadelCourts, "CourtId", "CourtName", padelBooking.CourtId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", padelBooking.UserId);
            return View(padelBooking);
        }

        //----------------------------------------------------------------------------------//
        private bool PadelBookingExists(int id)
        {
            return _context.PadelBookings.Any(e => e.BookingId == id);
        }
    }
}
