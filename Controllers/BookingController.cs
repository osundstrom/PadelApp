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



        

        // GET: Booking/Details/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var padelBooking = await _context.PadelBookings
                .Include(p => p.Court)
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.BookingId == id);
            if (padelBooking == null)
            {
                return NotFound();
            }

            return View(padelBooking);
        }

        // GET: Booking/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["CourtId"] = new SelectList(_context.PadelCourts, "CourtId", "CourtName");
            //ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Booking/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BookingId,BookingTime,CourtId,UserId")] PadelBooking padelBooking)
        {
            if (ModelState.IsValid)
            {
                
                _context.Add(padelBooking);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CourtId"] = new SelectList(_context.PadelCourts, "CourtId", "CourtName", padelBooking.CourtId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", padelBooking.UserId);
            return View(padelBooking);
        }

        // GET: Booking/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var padelBooking = await _context.PadelBookings.FindAsync(id);
            if (padelBooking == null)
            {
                return NotFound();
            }
            ViewData["CourtId"] = new SelectList(_context.PadelCourts, "CourtId", "CourtName", padelBooking.CourtId);
            //ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", padelBooking.UserId);
            ViewData["UserId"] = padelBooking.UserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            return View(padelBooking);
        }

        // POST: Booking/Edit/5
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BookingId,BookingTime,CourtId,UserId")] PadelBooking padelBooking)
        {
            if (id != padelBooking.BookingId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(padelBooking);
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
                return RedirectToAction(nameof(Index));
            }
            ViewData["CourtId"] = new SelectList(_context.PadelCourts, "CourtId", "CourtName", padelBooking.CourtId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", padelBooking.UserId);
            return View(padelBooking);
        }

        // GET: Booking/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var padelBooking = await _context.PadelBookings
                .Include(p => p.Court)
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.BookingId == id);
            if (padelBooking == null)
            {
                return NotFound();
            }

            return View(padelBooking);
        }

        // POST: Booking/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var padelBooking = await _context.PadelBookings.FindAsync(id);
            if (padelBooking != null)
            {
                _context.PadelBookings.Remove(padelBooking);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PadelBookingExists(int id)
        {
            return _context.PadelBookings.Any(e => e.BookingId == id);
        }
    }
}
