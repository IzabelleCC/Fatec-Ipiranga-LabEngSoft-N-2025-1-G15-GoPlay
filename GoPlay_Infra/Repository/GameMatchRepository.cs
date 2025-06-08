using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoPlay_Core.Entities;
using GoPlay_Core.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GoPlay_Infra.Repository
{
    public class GameMatchRepository : IGameMatchRepository
    {
        private readonly GoPlayDbContext _context;

        public GameMatchRepository(GoPlayDbContext context)
        {
            _context = context;
        }

        public async Task<GameMatchEntity?> GetByIdAsync(int id)
        {
            return await _context.GameMatches
                .Include(m => m.Competitor1)
                .Include(m => m.Competitor2)
                .Include(m => m.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<List<GameMatchEntity>> GetAllAsync()
        {
            return await _context.GameMatches
                .Include(m => m.Competitor1)
                .Include(m => m.Competitor2)
                .Include(m => m.Category)
                .ToListAsync();
        }

        public async Task AddAsync(GameMatchEntity match)
        {
            await _context.GameMatches.AddAsync(match);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(GameMatchEntity match)
        {
            _context.GameMatches.Update(match);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var match = await _context.GameMatches.FindAsync(id);
            if (match != null)
            {
                _context.GameMatches.Remove(match);
                await _context.SaveChangesAsync();
            }
        }
    }
}
