using GoPlay_Core.Entities;

namespace GoPlay_Core.Repository.Interfaces
{
    public interface IMatchGroupRepository
    {
        Task AddAsync(MatchGroupEntity matches);
        Task<List<MatchGroupEntity>> GetByCategoryAsync(int categoryId);
        Task<MatchGroupEntity?> GetByIdAsync(int id);
        Task UpdateAsync(MatchGroupEntity match);
        Task DeleteAsync(MatchGroupEntity match);
    }
}
