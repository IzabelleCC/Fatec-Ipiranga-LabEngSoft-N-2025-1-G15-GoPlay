using GoPlay_Core.Entities;
using GoPlay_Core.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GoPlay_Infra.Repository
{
    public class TournamentRepository : ITournamentRepository
    {
        private readonly GoPlayDbContext _context;

        public TournamentRepository(GoPlayDbContext context)
        {
            _context = context;
        }

        public async Task Add(TournamentEntity entity)
        {
            await _context.Tournaments.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task Update(TournamentEntity entity)
        {
            throw new NotImplementedException();
        }

        public async Task Delete(TournamentEntity entity)
        {
            throw new NotImplementedException();
        }

        public async Task<List<TournamentEntity?>> GetAllTournaments()
        {
            throw new NotImplementedException();
        }

        public async Task<TournamentEntity?> GetById(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<TournamentEntity?> GetByName(string name)
        {
            throw new NotImplementedException();
        }

        public async Task<List<TournamentEntity?>> GetByLocation(string location)
        {
            throw new NotImplementedException();
        }

        public async Task<List<TournamentEntity?>> GetByDate(DateTime date)
        {
            throw new NotImplementedException();
        }
    }
}
