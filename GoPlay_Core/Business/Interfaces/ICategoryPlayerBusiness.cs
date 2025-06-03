using System.Threading;
using GoPlay_Core.Entities;

namespace GoPlay_Core.Business.Interfaces
{
    public interface ICategoryPlayerBusiness
    {
        Task<List<CategoryPlayerEntity>> GetAllAsync(CancellationToken cancellationToken);
        Task<CategoryPlayerEntity?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<List<CategoryPlayerEntity>> GetByCategoryIdAsync(int categoryId, CancellationToken cancellationToken);
        Task<List<CategoryPlayerEntity>> GetByUserIdAsync(string userId, CancellationToken cancellationToken);
        Task RegisterUserToCategory(int categoryId, string firstUserId, string? secondUserId, CancellationToken cancellationToken);
        Task DeleteAsync(int id, CancellationToken cancellationToken);
        Task UpdatePlayersAsync(CategoryPlayerEntity entity, CancellationToken cancellationToken);

    }
}
