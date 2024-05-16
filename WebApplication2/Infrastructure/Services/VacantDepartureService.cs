using Microsoft.EntityFrameworkCore;
using WebApplication2.Data;
using WebApplication2.Models;

namespace WebApplication2.Infrastructure.Services
{
    public interface IVacantDepartureService
    {
        Task<List<VacantDeparture>> GetVacantDepartures(string? id = null);
        Task CreateAsync(VacantDeparture vacantDeparture, string providerId);
        int CalculateTotalSeatsInDeparture(int carsNumber, int seatsInOneCar);
        VacantDeparture GetVacantDepartureDetails(int? id);
        Task EditAsync(VacantDeparture vacantDeparture);
        Task DeleteAsync(int id);
    }
    public class VacantDepartureService: IVacantDepartureService
    {
        private readonly ApplicationDbContext _context;

        public VacantDepartureService(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task<List<VacantDeparture>> GetVacantDepartures(string? id = null)
        {
            if(id == null)
            {
                return _context.VacantDepartures.ToListAsync();
            }
            else
            {
                return _context.VacantDepartures.Where(v=> v.provider.Id == id).ToListAsync();
            }
        }
        public int CalculateTotalSeatsInDeparture(int carsNumber, int seatsInOneCar)
        {
            return carsNumber * seatsInOneCar;
        }
        public async Task CreateAsync(VacantDeparture departure, string providerId)
        {
            departure.provider = _context.Users.FirstOrDefault(u => u.Id == providerId);
            _context.Add(departure);
            await _context.SaveChangesAsync();
        }
        public VacantDeparture GetVacantDepartureDetails(int? id)
        {
            return _context.VacantDepartures
                            .Include(vd => vd.Train) // Include the Train navigation property
                            .Include(vd => vd.provider) // Include the provider navigation property
                            .First(m => m.vdID == id);
        }
        public async Task EditAsync(VacantDeparture vacantDeparture)
        {
            _context.Update(vacantDeparture);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var vacantDeparture = await _context.VacantDepartures.FindAsync(id);
            if (vacantDeparture != null)
            {
                _context.VacantDepartures.Remove(vacantDeparture);
            }
            await _context.SaveChangesAsync();
        }

    }
}
