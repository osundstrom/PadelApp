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
    [Authorize(Roles = "Admin")]
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

        // GET: PadelCourt/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var padelCourt = await _context.PadelCourts
                .FirstOrDefaultAsync(m => m.CourtId == id);
            if (padelCourt == null)
            {
                return NotFound();
            }

            return View(padelCourt);
        }

        // GET: PadelCourt/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: PadelCourt/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CourtId,CourtName,CourtType")] PadelCourt padelCourt)
        {
            if (ModelState.IsValid)
            {
                _context.Add(padelCourt);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(padelCourt);
        }

        // GET: PadelCourt/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var padelCourt = await _context.PadelCourts.FindAsync(id);
            if (padelCourt == null)
            {
                return NotFound();
            }
            return View(padelCourt);
        }

        // POST: PadelCourt/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CourtId,CourtName,CourtType")] PadelCourt padelCourt)
        {
            if (id != padelCourt.CourtId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(padelCourt);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PadelCourtExists(padelCourt.CourtId))
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
            return View(padelCourt);
        }

        // GET: PadelCourt/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var padelCourt = await _context.PadelCourts
                .FirstOrDefaultAsync(m => m.CourtId == id);
            if (padelCourt == null)
            {
                return NotFound();
            }

            return View(padelCourt);
        }

        // POST: PadelCourt/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var padelCourt = await _context.PadelCourts.FindAsync(id);
            if (padelCourt != null)
            {
                _context.PadelCourts.Remove(padelCourt);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PadelCourtExists(int id)
        {
            return _context.PadelCourts.Any(e => e.CourtId == id);
        }
    }
}
