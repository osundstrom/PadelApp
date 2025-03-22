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
     public class PadelBookingController : Controller{
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public PadelBookingController(ApplicationDbContext context, UserManager<IdentityUser> userManager){
            _context = context;
            _userManager = userManager; //hantera användare
        }

        // GET: PadelBooking
      public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User); //användare

        if (user == null) //om ej finns
        {
            return NotFound();
        }

        //bool för kolla om admin
        bool isAdmin = await _userManager.IsInRoleAsync(user, "Admin");

        ViewBag.IsAdmin = isAdmin;//skickas till vy

        if (isAdmin) { //om är admin
           
            var allBookings = await _context.PadelBookings
                .Include(p => p.Court) //banor
                .Include(p => p.User) //användare
                .ToListAsync();
            return View(allBookings);

        }else{ //om ej admin
            
            var userBookings = await _context.PadelBookings
                .Where(b => b.UserId == user.Id) //endast den användarens
                .Include(p => p.Court) //banor
                .Include(p => p.User) //användare
                .ToListAsync();
            return View(userBookings);
        }
    }

    //-------------------------------Create------------------------------------------------------//    
       [Authorize(Roles = "Admin")]
        // GET: PadelBooking/Create
        public IActionResult Create()
        {
            ViewData["CourtId"] = new SelectList(_context.Set<PadelCourt>(), "CourtId", "CourtName");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BookingId,BookingTime,CourtId,UserId")] PadelBooking padelBooking)
        {
            if (ModelState.IsValid)
            {
                _context.Add(padelBooking); //lägger till
                await _context.SaveChangesAsync(); //sparar
                return RedirectToAction(nameof(Index)); //tillbaka
            }

            ViewData["CourtId"] = new SelectList(_context.Set<PadelCourt>(), "CourtId", "CourtName", padelBooking.CourtId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", padelBooking.UserId);
            return View(padelBooking);
        }

//--------------------------------------Edit---------------------------------------------------------------------//

        // GET: PadelBooking/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id){

            if (id == null){ //om inge id
                return NotFound();
            }

            //hitta baserat id
            var padelBooking = await _context.PadelBookings.FindAsync(id);

            if (padelBooking == null){ //om ej finns
                return NotFound();
            }

            //alla banor och användare
            var courts = _context.PadelCourts
            .Select(c => new
            {
            CourtId = c.CourtId,
            CourtDisplay = $"{c.CourtName} ({c.CourtType})" // namn och typ
            })
            .ToList(); //lista för dropdown

            //vewdata med courts
            ViewData["CourtId"] = new SelectList(courts, "CourtId", "CourtDisplay", padelBooking.CourtId);
            
            return View(padelBooking);
        }

        
        [Authorize(Roles = "Admin")] //måste admin 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BookingId,BookingTime,CourtId,UserId")] PadelBooking padelBooking)
        {
            if (id != padelBooking.BookingId) //om id inte är samma som bokning
            {
                return NotFound();
            }
            
            //Hämta bokning
            var existingBooking = await _context.PadelBookings
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.BookingId == id);

            if (existingBooking == null){ //om bokning ej hittas
                return NotFound();
                }

            //samma user ska behållas(ej ändras)
            padelBooking.UserId = existingBooking.UserId; 

            if (ModelState.IsValid){

            try{
                _context.Update(padelBooking);//uppdatera
                //ändrade fält
                _context.Entry(padelBooking).Property(b => b.BookingTime).IsModified = true;
                _context.Entry(padelBooking).Property(b => b.CourtId).IsModified = true;
                await _context.SaveChangesAsync(); //spara

                }catch(DbUpdateConcurrencyException){

                if (!PadelBookingExists(padelBooking.BookingId)){ //om bokningen ej finns
                    return NotFound();
                }}

        return RedirectToAction(nameof(Index)); //tillbaka
        }

            //återställ listan(ej lyckad)
            ViewData["CourtId"] = new SelectList(_context.PadelCourts, "CourtId", "CourtName", padelBooking.CourtId);
            return View(padelBooking);
        }

//-------------------------------DELETE------------------------------------------------------//    

        // GET: PadelBooking/Delete/5
        public async Task<IActionResult> Delete(int? id){

            if (id == null){//om inget id
                return NotFound();
            }
            
            //hämta bokning baserat på id
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
        public async Task<IActionResult> DeleteConfirmed(int id){

            //hämta
            var padelBooking = await _context.PadelBookings.FindAsync(id);

            if (padelBooking != null) //om bokning finns
            {
                _context.PadelBookings.Remove(padelBooking);//radera
            }

            await _context.SaveChangesAsync(); //spara
            return RedirectToAction(nameof(Index));
        }

        //Bool om bokning existerar eller ej
        public bool PadelBookingExists(int id){
            return _context.PadelBookings.Any(e => e.BookingId == id);
        }
    }

    
}
