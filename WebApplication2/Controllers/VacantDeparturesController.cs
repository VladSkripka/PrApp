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
using WebApplication2.Infrastructure.Services;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    public class VacantDeparturesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IVacantDepartureService _vds;
        private readonly ITrainService _trainService;

        public VacantDeparturesController(
            ApplicationDbContext context, 
            UserManager<IdentityUser> userManager,
            IVacantDepartureService vds,
            ITrainService trainService)
        {
            _context = context;
            _userManager = userManager;
            _vds = vds;
            _trainService = trainService;
        }

        // GET: VacantDepartures
        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("Admin"))
            {
                var vacantDepatures = await _vds.GetVacantDepartures();
                return View(vacantDepatures);
            }
            else
            {
                var vacantDepatures = await _vds.GetVacantDepartures(_userManager.GetUserId(User));
                return View(vacantDepatures);
            }
        }

        // GET: VacantDepartures/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.VacantDepartures == null)
            {
                return NotFound();
            }

            var vacantDeparture = _vds.GetVacantDepartureDetails(id);

            if (vacantDeparture == null)
            {
                return NotFound();
            }

            vacantDeparture.Train = _trainService.GetTrainById(vacantDeparture.Train.trainTypeID);
            
            return View(vacantDeparture);
        }

        // GET: VacantDepartures/Create
        public IActionResult Create()
        {
            List<Train> trainTypes = _trainService.GetTrains();
            ViewBag.TrainTypes = new SelectList(trainTypes, "trainTypeID", "typeName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VacantDeparture vacantDeparture)
        {
            vacantDeparture.Train = _trainService.GetTrainById(vacantDeparture.Train.trainTypeID);
            vacantDeparture.seatsCount = _vds.CalculateTotalSeatsInDeparture(vacantDeparture.carriageCount, vacantDeparture.Train.trainSeats);
            var userId = _userManager.GetUserId(User);
            await _vds.CreateAsync(vacantDeparture, userId);
            return RedirectToAction(nameof(Index));
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
        public async Task<IActionResult> Edit(int id, VacantDeparture vacantDeparture)
        {
            if (id != vacantDeparture.vdID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _vds.EditAsync(vacantDeparture);
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
            await _vds.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private bool VacantDepartureExists(int id)
        {
          return (_context.VacantDepartures?.Any(e => e.vdID == id)).GetValueOrDefault();
        }
    }
}
