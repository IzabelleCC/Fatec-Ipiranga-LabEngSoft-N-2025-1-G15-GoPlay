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

        public async Task<CategoryGroupsDto> GetCategoryGroups(int categoryId)
        {
            var matchGroups = await _context.Matches
                .Where(m => m.CategoryId == categoryId)
                .Include(m => m.RegistrationCategory)
                    .ThenInclude(rc => rc.FirstUser)
                .Include(m => m.RegistrationCategory)
                    .ThenInclude(rc => rc.SecondUser)
                .ToListAsync();


            var categoryGroupsDto = new CategoryGroupsDto
            {
                CategoryId = categoryId,
                Groups = matchGroups
                    .GroupBy(m => m.GroupNumber)
                    .Select(g => new GroupDto
                    {
                        GroupNumber = g.Key,
                        Players = g.Select(m => new GroupPlayerDto
                        {
                            Id = m.Id,
                            FirstUserId = m.RegistrationCategory.FirstUserId.ToString(),
                            FirstUserName = m.RegistrationCategory.FirstUser.Name,
                            SecondUserId = m.RegistrationCategory.SecondUserId?.ToString(),
                            SecondUserName = m.RegistrationCategory.SecondUser != null
                                ? m.RegistrationCategory.SecondUser.Name
                                : null
                        }).ToList()
                    }).ToList()
            };
            return categoryGroupsDto;
        }

    }
}
