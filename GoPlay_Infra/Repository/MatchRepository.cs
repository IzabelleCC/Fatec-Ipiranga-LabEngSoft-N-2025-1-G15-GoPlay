using GoPlay_Core.Entities;
using GoPlay_Core.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GoPlay_Infra.Repository
{
    public class MatchRepository : IMatchRepository
    {
        private readonly GoPlayDbContext _context;

        public MatchRepository(GoPlayDbContext context)
        {
            _context = context;
        }

        public async Task AddRangeAsync(List<MatchGroupEntity> matches)
        {
            await _context.Matches.AddRangeAsync(matches);
            await _context.SaveChangesAsync();
        }

        public async Task<List<MatchGroupEntity>> GetByCategoryAsync(int categoryId)
        {
            return await _context.Matches
                .Where(m => m.CategoryId == categoryId)
                .Include(m => m.Category)
                .ToListAsync();
        }

        public async Task<MatchGroupEntity?> GetByIdAsync(int id)
        {
            return await _context.Matches
                .Include(m => m.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task UpdateAsync(MatchGroupEntity match)
        {
            _context.Matches.Update(match);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(MatchGroupEntity match)
        {
            _context.Matches.Remove(match);
            await _context.SaveChangesAsync();
        }
    }
}
