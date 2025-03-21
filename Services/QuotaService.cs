using RailwayReservationMVC.Data;
using RailwayReservationMVC.Models;
using System.Linq;

namespace RailwayReservationMVC.Services
{
    public class QuotaService
    {
        private readonly ApplicationDbContext _context;

        public QuotaService(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool CheckSeatAvailability(int trainId, int quotaId, int requestedSeats)
        {
            var quota = _context.Quota.FirstOrDefault(q => q.TrainID == trainId && q.QuotaID == quotaId);
            return quota != null && quota.SeatsAvailable >= requestedSeats;
        }

        public void AllocateSeats(int trainId, int quotaId, int requestedSeats)
        {
            var quota = _context.Quota.FirstOrDefault(q => q.TrainID == trainId && q.QuotaID == quotaId);
            if (quota == null || quota.SeatsAvailable < requestedSeats) return;

            quota.SeatsAvailable -= requestedSeats;
            _context.SaveChanges();
        }
    }
}
