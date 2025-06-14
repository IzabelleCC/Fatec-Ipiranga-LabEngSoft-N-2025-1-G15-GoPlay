using GoPlay_Core.Entities;
using GoPlay_Core.Models.Dto;
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

        public async Task<List<GameMatchEntity>> GetMatchesByCategoryIdAsync(int categoryId)
        {
            return await _context.GameMatches
                .Where(m => m.CategoryId == categoryId)
                .Include(m => m.Competitor1)
                .Include(m => m.Competitor2)
                .Include(m => m.Category)
                .ToListAsync();
        }

        public async Task<List<EliminationGameDto>> GetEliminationGamesByCategory(int categoryId, int matchStage)
        {
            var matches = await _context.GameMatches
                                        .Where(m => m.CategoryId == categoryId && (int)m.MatchStage == matchStage)
                                        .Include(m => m.Competitor1)
                                        .Include(m => m.Competitor2)
                                        .ToListAsync();
 
            var result = matches.Select(m => new EliminationGameDto
            {
                Competitor1Id = m.Competitor1Id,
                Competitor2Id = m.Competitor2Id,
                MatchStage = m.MatchStage,
                MatchTime = m.MatchTime,
                CourtNumber = m.CourtNumber,
                QtdGames1 = m.QtdGames1,
                QtdGames2 = m.QtdGames2,
                Result = m.Result,
                NumberGame = m.NumberGame,
                CategoryId = m.CategoryId,
                Competitor1 = m.Competitor1 == null ? new GroupPlayerDto() : new GroupPlayerDto
                {
                    Id = m.Competitor1.Id,
                    FirstUserId = m.Competitor1.FirstUserId,
                    SecondUserId = m.Competitor1.SecondUserId
                },
                Competitor2 = m.Competitor2 == null ? new GroupPlayerDto() : new GroupPlayerDto
                {
                    Id = m.Competitor2.Id,
                    FirstUserId = m.Competitor2.FirstUserId,
                    SecondUserId = m.Competitor2.SecondUserId
                }
            }).ToList();

            return result;
        }

    }
}
