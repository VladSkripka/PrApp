using Microsoft.AspNetCore.Http.HttpResults;
using WebApplication2.Data;
using WebApplication2.Models;

namespace WebApplication2.Infrastructure.Services
{
    public interface ITrainService
    {
        List<Train> GetTrains();
        Train GetTrainById(int id);
       
    }
    public class TrainService:ITrainService
    {
        private readonly ApplicationDbContext _context;

        public TrainService(ApplicationDbContext context)
        {
            _context = context;
        }
        public List<Train> GetTrains()
        {
            return _context.Trains.ToList();
        }
        public Train GetTrainById(int id)
        {
            return _context.Trains.FirstOrDefault(train => train.trainTypeID == id);
        }
    }
}
