using GoPlay_Core.Entities;

namespace GoPlay_Core.Repository.Interfaces
{
    public interface IGameMatchRepository
    {
        Task<GameMatchEntity?> GetByIdAsync(int id);
        Task<List<GameMatchEntity>> GetAllAsync();
        Task AddAsync(GameMatchEntity match);
        Task UpdateAsync(GameMatchEntity match);
        Task DeleteAsync(int id);
        Task<List<GameMatchEntity>> GetMatchesByCategoryIdAsync(int categoryId);
    }
}
