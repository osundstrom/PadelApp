using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PadelApp.Data;
using PadelserviceApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Padelapp.Controllers
{
    [Authorize(Roles = "Admin")] //admin
    public class PadelCourtController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PadelCourtController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: PadelCourt
        public async Task<IActionResult> Index()
        {
            return View(await _context.PadelCourts.ToListAsync());
        }
   
//--------------------------Create-----------------------------------------------------//       

        [Authorize(Roles = "Admin")] //admin
        public IActionResult Create()
        {
            return View();
        }

        
        [Authorize(Roles = "Admin")] //admin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CourtId,CourtName,CourtType")] PadelCourt padelCourt)
        {
            if (ModelState.IsValid)
            {
                _context.Add(padelCourt);//lägg till
                await _context.SaveChangesAsync(); //spara
                return RedirectToAction(nameof(Index)); //tillbaka till vy
            }
            return View(padelCourt);
        }

//--------------------------Delete-----------------------------------------------------//       

        // GET: PadelCourt/Delete/5
        [Authorize(Roles = "Admin")] //admin
        public async Task<IActionResult> Delete(int? id){

            if (id == null){//om id ej finns
                return NotFound();
            }

            //hämta padelbanan baserat id
            var padelCourt = await _context.PadelCourts
                .FirstOrDefaultAsync(m => m.CourtId == id); 

            if (padelCourt == null){//om ej finns
                return NotFound();
            }

            return View(padelCourt);
        }

        // POST: PadelCourt/Delete/5
        [Authorize(Roles = "Admin")] //admin
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id){

            //hitta baserat på id
            var padelCourt = await _context.PadelCourts.FindAsync(id);
            if (padelCourt != null)
            {
                _context.PadelCourts.Remove(padelCourt);//ta bort
            }

            await _context.SaveChangesAsync(); //spara
            return RedirectToAction(nameof(Index));
        }

//--------------------------Bool-----------------------------------------------------//       

        private bool PadelCourtExists(int id)
        {
            return _context.PadelCourts.Any(e => e.CourtId == id);
        }
    }
}
