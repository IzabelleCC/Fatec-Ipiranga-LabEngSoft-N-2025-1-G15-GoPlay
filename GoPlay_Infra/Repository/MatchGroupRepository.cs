using GoPlay_Core.Entities;
using GoPlay_Core.Models.Dto;
using GoPlay_Core.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GoPlay_Infra.Repository
{
    public class MatchGroupRepository : IMatchGroupRepository
    {
        private readonly GoPlayDbContext _context;

        public MatchGroupRepository(GoPlayDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(MatchGroupEntity matches)
        {
            await _context.Matches.AddAsync(matches);
            await _context.SaveChangesAsync();
        }

        public async Task<List<MatchGroupEntity>> GetByCategoryAsync(int categoryId)
        {
            return await _context.Matches
                .Where(m => m.CategoryId == categoryId)
                .Include(m => m.RegistrationCategory)
                .Include(m => m.Category)
                .ToListAsync();
        }

        public async Task<MatchGroupEntity?> GetByIdAsync(int id)
        {
            return await _context.Matches
                .Include(m => m.RegistrationCategory)
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

        public async Task<List<MatchGroupEntity>> GetByCategoryAndGroupAsync(int categoryId, int groupNumber)
        {
            return await _context.Matches
                .Where(m => m.CategoryId == categoryId && m.GroupNumber == groupNumber)
                .Include(m => m.Category)
                .ToListAsync();
        }

        public async Task<MatchGroupEntity> GetbyRegistrationCategoryAsync(int registrationCategory)
        {
            return await _context.Matches
                .Include(m => m.RegistrationCategory)
                .Include(m => m.Category)
                .FirstOrDefaultAsync(m => m.RegistrationCategoryId == registrationCategory)
                ?? throw new KeyNotFoundException("Match group not found for the given registration category.");

        }

        public async Task<TournamentMatchesResultDto> GetTournamentMatchesByCategory(int categoryId)
        {
            var matchGroups = await _context.Matches
                .Where(m => m.CategoryId == categoryId)
                .Include(m => m.Category)
                .Include(m => m.RegistrationCategory)
                    .ThenInclude(rc => rc.FirstUser)
                .Include(m => m.RegistrationCategory)
                    .ThenInclude(rc => rc.SecondUser)
                .ToListAsync();

            if (!matchGroups.Any())
            {
                throw new Exception($"No matches found for CategoryId {categoryId}.");
            }

            var tournamentId = matchGroups.First().Category.TournamentId;

            var tournament = await _context.Tournaments
                .Where(t => t.Id == tournamentId)
                .Select(t => new
                {
                    t.Id,
                    t.Name
                })
                .FirstOrDefaultAsync();

            if (tournament == null)
            {
                throw new Exception($"Tournament with Id {tournamentId} not found.");
            }

            var tournamentMatchesResultDto = new TournamentMatchesResultDto
            {
                TournamentId = tournament.Id,
                TournamentName = tournament.Name,
                Groups = new List<CategoryGroupsDto>
                {
                    new CategoryGroupsDto
                    {
                        CategoryId = categoryId,
                        CategoryName = matchGroups.First().Category.CategoryType,
                        Groups = matchGroups
                            .GroupBy(m => m.GroupNumber)
                            .Select(group => new GroupDto
                            {
                                GroupNumber = group.Key,
                                Players = group.Select(m => new GroupPlayerDto
                                {
                                    Id = m.RegistrationCategoryId,
                                    FirstUserId = m.RegistrationCategory.FirstUserId.ToString(),
                                    FirstUserName = m.RegistrationCategory.FirstUser.Name,
                                    SecondUserId = m.RegistrationCategory.SecondUserId?.ToString(),
                                    SecondUserName = m.RegistrationCategory.SecondUser != null
                                        ? m.RegistrationCategory.SecondUser.Name
                                        : null
                                }).ToList()
                            }).ToList()
                    }
                }
            };

            return tournamentMatchesResultDto;
        }

        public async Task<List<MatchDto>> GetGroupResultByCategoryId(int categoryId, int groupNumber)
        {
            var matches = await _context.Matches
                .Where(m => m.CategoryId == categoryId && m.GroupNumber == groupNumber)
                .Select(m => new MatchDto
                {
                    CategoryId = m.CategoryId,
                    GroupNumber = m.GroupNumber,
                    RegistrationCategoryId = m.RegistrationCategoryId,
                    ScheduledAt = m.ScheduledAt,
                    AttendanceConfirmed = m.AttendanceConfirmed,
                    Position = m.Position,
                    Wins = m.Wins,
                    Losses = m.Losses,
                    SetsBalance = m.SetsBalance,
                    Game1 = m.Game1,
                    Game2 = m.Game2,
                    Game3 = m.Game3,
                    Game4 = m.Game4,
                    Game5 = m.Game5,
                    SumOfGamesWon = m.SumOfGamesWon,
                    SumOfGamesLost = m.SumOfGamesLost,
                    GamesBalance = m.GamesBalance,
                    Tiebreaks = m.Tiebreaks,
                    MatchStage = m.MatchStage
                })
                .ToListAsync();

            if (!matches.Any())
                throw new KeyNotFoundException($"Nenhuma partida encontrada para Categoria {categoryId} e Grupo {groupNumber}.");

            return matches;
        }



    }
}
