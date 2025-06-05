using GoPlay_Core.Entities;

namespace GoPlay_Core.Repository.Interfaces
{
    public interface IGameMatchRepository
    {
        Task<GameMatchEntity?> GetByIdAsync(int id);
        Task<List<GameMatchEntity>> GetAllAsync();
        Task AddAsync(GameMatchEntity match, CancellationToken cancellationToken);
        Task UpdateAsync(GameMatchEntity match, CancellationToken cancellationToken);
        Task DeleteAsync(int id, CancellationToken cancellationToken);
    }
}
