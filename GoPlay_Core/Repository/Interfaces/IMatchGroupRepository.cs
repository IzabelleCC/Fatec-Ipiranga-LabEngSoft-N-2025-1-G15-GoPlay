using GoPlay_Core.Entities;
using GoPlay_Core.Models.Dto;

namespace GoPlay_Core.Repository.Interfaces
{
    public interface IMatchGroupRepository
    {
        Task AddAsync(MatchGroupEntity matches);
        Task<List<MatchGroupEntity>> GetByCategoryAsync(int categoryId);
        Task<MatchGroupEntity?> GetByIdAsync(int id);
        Task UpdateAsync(MatchGroupEntity match);
        Task DeleteAsync(MatchGroupEntity match);
        Task<List<MatchGroupEntity>> GetByCategoryAndGroupAsync(int categoryId, int groupNumber);
        Task<MatchGroupEntity> GetbyRegistrationCategoryAsync(int registrationCategory);
        Task<TournamentMatchesResultDto> GetTournamentMatchesByCategory(int categoryId);
        Task<List<MatchDto>> GetGroupResultByCategoryId(int categoryId, int groupNumber);
    }
}
