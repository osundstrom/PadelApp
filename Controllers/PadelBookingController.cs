using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PadelApp.Data;
using PadelserviceApp.Models;

namespace Padelapp.Controllers
{
     public class PadelBookingController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public PadelBookingController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: PadelBooking
      public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User); 
        if (user == null)
        {
            return NotFound();
        }

        
        bool isAdmin = await _userManager.IsInRoleAsync(user, "Admin");

        ViewBag.IsAdmin = isAdmin;

        if (isAdmin)
        {
           
            var allBookings = await _context.PadelBookings
                .Include(p => p.Court)
                .Include(p => p.User)
                .ToListAsync();
            return View(allBookings);
        }
        else
        {
            
            var userBookings = await _context.PadelBookings
                .Where(b => b.UserId == user.Id)
                .Include(p => p.Court)
                .Include(p => p.User)
                .ToListAsync();
            return View(userBookings);
        }
    }

        // GET: PadelBooking/Details/5
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
        
       [Authorize(Roles = "Admin")]
        // GET: PadelBooking/Create
        public IActionResult Create()
        {
            ViewData["CourtId"] = new SelectList(_context.Set<PadelCourt>(), "CourtId", "CourtName");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: PadelBooking/Create
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
            ViewData["CourtId"] = new SelectList(_context.Set<PadelCourt>(), "CourtId", "CourtName", padelBooking.CourtId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", padelBooking.UserId);
            return View(padelBooking);
        }

//-----------------------------------------------------------------------------------------------------------//

        // GET: PadelBooking/Edit/5
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


            var courts = _context.PadelCourts
        .Select(c => new
        {
            CourtId = c.CourtId,
            CourtDisplay = $"{c.CourtName} ({c.CourtType})" 
        })
        .ToList();

            ViewData["CourtId"] = new SelectList(courts, "CourtId", "CourtDisplay", padelBooking.CourtId);
            var users = _context.Users
                .Select(u => new
                {
                    UserId = u.Id,
                    UserName = u.UserName
                })
                .ToList();

            ViewData["UserId"] = new SelectList(users, "UserId", "UserName", padelBooking.UserId);  
            return View(padelBooking);
        }

        // POST: PadelBooking/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")] 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BookingId,BookingTime,CourtId,UserId")] PadelBooking padelBooking)
        {
            if (id != padelBooking.BookingId)
            {
                return NotFound();
            }

             var existingBooking = await _context.PadelBookings
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.BookingId == id);

            if (existingBooking == null){
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

        return RedirectToAction(nameof(Index)); // Redirect back to the BookingPage (or wherever you want to)
    }
            ViewData["CourtId"] = new SelectList(_context.PadelCourts, "CourtId", "CourtName", padelBooking.CourtId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "UserName", padelBooking.UserId);
            return View(padelBooking);
        }

        // GET: PadelBooking/Delete/5
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

        // POST: PadelBooking/Delete/5
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
