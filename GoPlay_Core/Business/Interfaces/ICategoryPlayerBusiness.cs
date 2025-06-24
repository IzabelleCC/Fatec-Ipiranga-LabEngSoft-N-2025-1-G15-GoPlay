using GoPlay_Core.Entities;
using GoPlay_Core.Models.Dto;

namespace GoPlay_Core.Business.Interfaces
{
    public interface ICategoryPlayerBusiness
    {
        Task<List<CategoryPlayerEntity>> GetAllAsync(CancellationToken cancellationToken);
        Task<CategoryPlayerEntity?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<List<CategoryPlayerDto>> GetByCategoryIdAsync(int categoryId, CancellationToken cancellationToken);
        Task<List<CategoryPlayerEntity>> GetByUserIdAsync(string userId, CancellationToken cancellationToken);
        Task<CategoryPlayerEntity> RegisterUserToCategory(int categoryId, string firstUserId, string? secondUserId, CancellationToken cancellationToken);
        Task DeleteAsync(int id, CancellationToken cancellationToken);
        Task UpdatePlayersAsync(CategoryPlayerEntity entity, CancellationToken cancellationToken);
        Task<List<CategoryPlayerFullInfoDto>> GetByUserIdAndReturnsFullInfoAsync(string userId, CancellationToken cancellationToken);
        Task<CategoryPlayerDto?> GetRegistrationDetails(int id, CancellationToken cancellationToken);
        

    }
}
