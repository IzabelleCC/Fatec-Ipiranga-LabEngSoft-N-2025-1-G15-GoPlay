using GoPlay_Core.Entities;

namespace GoPlay_Core.Repository.Interfaces
{
    public interface ICategoryPlayerRepository
    {
        Task<List<CategoryPlayerEntity>> GetAllAsync();
        Task<List<CategoryPlayerEntity>> GetByCategoryIdAsync(int categoryId);
        Task<CategoryPlayerEntity?> GetByIdAsync(int id);
        Task<List<CategoryPlayerEntity>> GetByUserIdAsync(string userId);
        Task AddAsync(CategoryPlayerEntity entity);
        Task DeleteAsync(int id);
        Task UpdatePlayersAsync(CategoryPlayerEntity entity);
        Task<CategoryPlayerEntity?> GetByTxIdAsync(string txid);
    }
}
