using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Data;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    public class VacantDeparturesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHttpClientFactory _clientFactory;

        public VacantDeparturesController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IHttpClientFactory clientFactory)
        {
            _context = context;
            _userManager = userManager;
            _clientFactory = clientFactory;
        }

        // GET: VacantDepartures
        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("Admin"))
            {
                return _context.VacantDepartures != null ?
                          View(await _context.VacantDepartures.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.TicketModels'  is null.");
            }
            else
            {
                var id = _userManager.GetUserId(User);
                return _context.VacantDepartures != null ?
                          View(await _context.VacantDepartures.Where(dep => dep.provider.Id == id).ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.TicketModels'  is null.");
            }
        }

        // GET: VacantDepartures/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.VacantDepartures == null)
            {
                return NotFound();
            }

            var vacantDeparture = await _context.VacantDepartures
                .Include(vd => vd.Train) // Include the Train navigation property
                .Include(vd => vd.provider) // Include the provider navigation property
                .FirstOrDefaultAsync(m => m.vdID == id);
           
            if (vacantDeparture == null)
            {
                return NotFound();
            }
            vacantDeparture.Train = _context.Trains.FirstOrDefault(train => train.trainTypeID == vacantDeparture.Train.trainTypeID);
            return View(vacantDeparture);
        }

        // GET: VacantDepartures/Create
        public IActionResult Create()
        {
            List<Train> trainTypes = _context.Trains.ToList();
            // Pass the list to the view
            ViewBag.TrainTypes = new SelectList(trainTypes, "trainTypeID", "typeName");
            return View();
        }

        // POST: VacantDepartures/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VacantDeparture vacantDeparture)
        {
            vacantDeparture.Train = _context.Trains.FirstOrDefault(train => train.trainTypeID == vacantDeparture.Train.trainTypeID);
            vacantDeparture.seatsCount = vacantDeparture.carriageCount * vacantDeparture.Train.trainSeats;

            var userId = _userManager.GetUserId(User);
            vacantDeparture.provider = _context.Users.FirstOrDefault(user => user.Id == userId);

            if (ModelState.IsValid)
            { 
                _context.Add(vacantDeparture);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                // Log validation errors
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine(error.ErrorMessage);
                }
            }

            return View(vacantDeparture);
        }

        // GET: VacantDepartures/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.VacantDepartures == null)
            {
                return NotFound();
            }
            var vacantDeparture = await _context.VacantDepartures.FindAsync(id);
            if (vacantDeparture == null)
            {
                return NotFound();
            }
            return View(vacantDeparture);
        }

        // POST: VacantDepartures/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("vdID,PID,vacanDepartureName,departureP,departureCoord,arrivalP,departureD,arrivalD,price,seatsCount, carriageCount")] VacantDeparture vacantDeparture)
        {
            if (id != vacantDeparture.vdID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(vacantDeparture);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VacantDepartureExists(vacantDeparture.vdID))
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
            return View(vacantDeparture);
        }

        // GET: VacantDepartures/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.VacantDepartures == null)
            {
                return NotFound();
            }

            var vacantDeparture = await _context.VacantDepartures
                .FirstOrDefaultAsync(m => m.vdID == id);
            if (vacantDeparture == null)
            {
                return NotFound();
            }

            return View(vacantDeparture);
        }

        // POST: VacantDepartures/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.VacantDepartures == null)
            {
                return Problem("Entity set 'ApplicationDbContext.VacantDepartures'  is null.");
            }
            var vacantDeparture = await _context.VacantDepartures.FindAsync(id);
            if (vacantDeparture != null)
            {
                _context.VacantDepartures.Remove(vacantDeparture);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VacantDepartureExists(int id)
        {
          return (_context.VacantDepartures?.Any(e => e.vdID == id)).GetValueOrDefault();
        }
    }
}
