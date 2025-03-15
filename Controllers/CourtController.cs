using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PadelApp.Data;
using PadelserviceApp.Models;

namespace PadelApp.Controllers
{
    public class CourtController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CourtController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Court
        public async Task<IActionResult> Index()
        {
            return View(await _context.PadelCourts.ToListAsync());
        }

        // GET: Court/Details/5
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

        // GET: Court/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Court/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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

        // GET: Court/Edit/5
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

        // POST: Court/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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

        // GET: Court/Delete/5
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

        // POST: Court/Delete/5
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
