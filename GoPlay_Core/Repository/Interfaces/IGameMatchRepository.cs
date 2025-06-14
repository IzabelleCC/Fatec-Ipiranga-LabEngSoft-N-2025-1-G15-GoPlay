using GoPlay_Core.Entities;
using GoPlay_Core.Models.Dto;

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
        Task<List<EliminationGameDto>> GetEliminationGamesByCategory(int categoryId, int matchStage);
    }
}
