using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Data;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    public class TicketController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public TicketController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Ticket
        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("Admin"))
            {
                return _context.TicketModels != null ?
                          View(
                              await _context.TicketModels
                                  .OrderByDescending(t => t.PurchaseDate)
                                  .ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.TicketModels'  is null.");
            }
            else
            {
                var id = _userManager.GetUserId(User);
                return _context.TicketModels != null ?
                          View(
                              await _context.TicketModels
                                .Include( t => t.user)
                                .Where(t => t.user.Id == id)
                                .OrderByDescending(t => t.PurchaseDate)
                                .ToListAsync()
                          ) :
                          Problem("Entity set 'ApplicationDbContext.TicketModels'  is null.");
            }

        }

        // GET: Ticket/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.TicketModels == null)
            {
                return NotFound();
            }

            var ticketModel = await _context.TicketModels
                .FirstOrDefaultAsync(m => m.ticketID == id);
            if (ticketModel == null)
            {
                return NotFound();
            }

            return View(ticketModel);
        }

        // GET: Ticket/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Ticket/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ticketID,ticketPID,departureP,arrivalP,departureD,arrivalD,passengerFullName,carriageNumber,seat,status,vacantDepatureID")] TicketModel ticketModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(ticketModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(ticketModel);
        }

        // GET: Ticket/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.TicketModels == null)
            {
                return NotFound();
            }

            var ticketModel = await _context.TicketModels.FindAsync(id);
            if (ticketModel == null)
            {
                return NotFound();
            }
            return View(ticketModel);
        }

        // POST: Ticket/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ticketID,ticketPID,departureP,arrivalP,departureD,arrivalD,passengerFullName,carriageNumber,seat,status,vacantDepatureID")] TicketModel ticketModel)
        {
            if (id != ticketModel.ticketID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ticketModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TicketModelExists(ticketModel.ticketID))
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
            return View(ticketModel);
        }

        public async Task<IActionResult> Cancel(int id)
        {
            if (id == null || _context.TicketModels == null)
            {
                return NotFound();
            }

            var ticket = _context.TicketModels.FirstOrDefault(t => t.ticketID == id);

            ticket.status = "cancelled";
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ticket);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TicketModelExists(ticket.ticketID))
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
            return View(ticket);
        }
        // GET: Ticket/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.TicketModels == null)
            {
                return NotFound();
            }

            var ticketModel = await _context.TicketModels
                .FirstOrDefaultAsync(m => m.ticketID == id);
            if (ticketModel == null)
            {
                return NotFound();
            }

            return View(ticketModel);
        }

        // POST: Ticket/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.TicketModels == null)
            {
                return Problem("Entity set 'ApplicationDbContext.TicketModels'  is null.");
            }
            var ticketModel = await _context.TicketModels.FindAsync(id);
            if (ticketModel != null)
            {
                _context.TicketModels.Remove(ticketModel);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TicketModelExists(int id)
        {
          return (_context.TicketModels?.Any(e => e.ticketID == id)).GetValueOrDefault();
        }
    }
}
