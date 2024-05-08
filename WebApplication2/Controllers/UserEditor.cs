using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Net.Sockets;
using System.Security.Policy;
using WebApplication2.Data;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    public class UserEditor : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public UserEditor(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: UserEditor
        public async Task<IActionResult> Index()
        {
            var authUsers = _context.Users.Where(user => !string.IsNullOrEmpty(user.UserName)).Count();
            ViewBag.Authenticated = authUsers;
            var totalUsers = _context.Users.Count();
            ViewBag.TotalUsers = totalUsers;
            ViewBag.Unauthenticated = totalUsers - authUsers;
            return _context.Users != null ?
                         View(await _context.Users.ToListAsync()) :
                         Problem("Entity set 'ApplicationDbContext.Users'  is null.");
        }

        public async Task<IActionResult> Roles()
        {
            return _context.Roles != null ?
                         View(await _context.Roles.ToListAsync()) :
                         Problem("Entity set 'ApplicationDbContext.Roles'  is null.");
        }

            // GET: UserEditor/Details/5
        public async Task<IActionResult> UsersInRole(string id)
        {
            List<IdentityUserRole<string>> UsersIdInRoleList = await _context.UserRoles.Where(ur => ur.RoleId== id).ToListAsync();
            
            List<IdentityUser> users = new List<IdentityUser>();
            foreach (var item in UsersIdInRoleList)
            {
                users.Add(_context.Users.FirstOrDefault(u => u.Id == item.UserId));
            }

            var role = _context.Roles.FirstOrDefault(u => u.Id == id);
            ViewBag.RoleName = role.Name;

            return View(users);
        }

        // GET: UserEditor/Create
        public ActionResult UserCreator()
        {
            List<IdentityRole> roleList = _context.Roles.ToList();
            ViewBag.Roles = new SelectList(roleList, "Id", "Name");
            return View();
        }

        // POST: UserEditor/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UserCreator(IdentityUser user, string password, string? roleId)
        {

            PasswordHasher<IdentityUser> ph = new PasswordHasher<IdentityUser>();
            user.PasswordHash = ph.HashPassword(user, password);

            user.NormalizedUserName = user.UserName.ToUpper();
            user.EmailConfirmed = true;
            user.NormalizedEmail = user.Email.ToUpper();
            user.PhoneNumberConfirmed = true;

            if (ModelState.IsValid)
            {
                _context.Add(user);
                await _context.SaveChangesAsync();

                if (!string.IsNullOrEmpty(roleId))
                {
                    var uRole = new IdentityUserRole<string>
                    {
                        UserId = user.Id,
                        RoleId = roleId
                    };

                    _context.Add(uRole);
                    await _context.SaveChangesAsync();
                }
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

            return View(user);
        }

        public async Task<IActionResult> UserDetails(string id)
        {
            if (id == null || _context.TicketModels == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: UserEditor/Delete/5
        public async Task<IActionResult> DeleteUser(string id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: UserEditor/Delete/5
        [HttpPost, ActionName("DeleteUser")]
        public async Task<IActionResult> DeleteConfirm(string id)
        {
            if (_context.Users == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Users'  is null.");
            }
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }



        public async Task<IActionResult> Dashboard()
        {
            var id = _userManager.GetUserId(User);
            var providerDeparts = _context.VacantDepartures.Where(vd => vd.provider.Id == id).ToList();
            List<string> soldTicketNumber = new List<string>();
            List<string> cancelledTicketNumber = new List<string>();

            foreach (var item in providerDeparts)
            {
                cancelledTicketNumber.Add(_context.TicketModels.Count(t => t.vacantDepatureID == item.vdID && t.status == "cancelled").ToString());
                soldTicketNumber.Add(_context.TicketModels.Count(t => t.vacantDepatureID == item.vdID && t.status == "sold").ToString());
            }

            GetAmountOfTicketsByMonths(providerDeparts);

            ViewBag.TicketCount = soldTicketNumber;
            ViewBag.CancelledTicketCount = cancelledTicketNumber;
            return View(providerDeparts);
        }

        public async Task<IActionResult> AdminDashboard()
        {
            List<TicketModel> soldTickets = _context.TicketModels.Where(t => t.status == "sold").ToList();
            ViewBag.soldTickets = soldTickets.Count;

            List<TicketModel> cancelledTickets = _context.TicketModels.Where(t => t.status == "cancelled").ToList();
            ViewBag.cancelledTickets = cancelledTickets.Count;

            GetAmountOfTicketsByMonths(soldTickets, cancelledTickets);
            return View();
        }

        //Additional methods

        public void GetAmountOfTicketsByMonths(List<VacantDeparture> vd)
        {
            var currentMonth = DateTime.Now.Month;
            var soldTicketPerMonth = new int[currentMonth];
            var cancelledTicketPerMonth = new int[currentMonth];
            foreach (var item in vd)
            {
                var tickets = _context.TicketModels.Where(t => t.vacantDepatureID == item.vdID);
                foreach (var ticket in tickets)
                {
                    if (ticket.PurchaseDate.Year == DateTime.Now.Year && ticket.PurchaseDate.Month <= currentMonth)
                    {
                        var monthIndex = ticket.PurchaseDate.Month - 1; // Month index starts from 0
                        if (ticket.status == "sold")
                        {
                            soldTicketPerMonth[monthIndex]++;
                        }
                        else if (ticket.status == "cancelled")
                        {
                            cancelledTicketPerMonth[monthIndex]++;
                        }
                    }
                }
            }
            ViewBag.soldTicketPerMonth = soldTicketPerMonth;
            ViewBag.cancelledTicketPerMonth = cancelledTicketPerMonth;
        }
        public void GetAmountOfTicketsByMonths(List<TicketModel> st, List<TicketModel> ct)
        {
            var currentMonth = DateTime.Now.Month;
            var soldTicketPerMonth = new int[currentMonth];
            var cancelledTicketPerMonth = new int[currentMonth];
            foreach (var ticket in st)
            {
                if (ticket.PurchaseDate.Year == DateTime.Now.Year && ticket.PurchaseDate.Month <= currentMonth)
                {
                    var monthIndex = ticket.PurchaseDate.Month - 1; // Month index starts from 0
                    soldTicketPerMonth[monthIndex]++;
                }
            }

            foreach (var ticket in ct)
            {
                if (ticket.PurchaseDate.Year == DateTime.Now.Year && ticket.PurchaseDate.Month <= currentMonth)
                {
                    var monthIndex = ticket.PurchaseDate.Month - 1; // Month index starts from 0
                    cancelledTicketPerMonth[monthIndex]++;
                }
            }
            ViewBag.soldTicketPerMonth = soldTicketPerMonth;
            ViewBag.cancelledTicketPerMonth = cancelledTicketPerMonth;
        }
    }
}
