using GoPlay_Core.Entities;

namespace GoPlay_Core.Repository.Interfaces
{
    public interface ICategoryRepository
    {
        Task Add(CategoryEntity entity);
        Task Update(CategoryEntity entity);
        Task Delete(CategoryEntity entity);
        Task<List<CategoryEntity?>> GetAllCategories();
        Task<CategoryEntity?> GetById(int id);
        Task<CategoryEntity?> GetByCategoryType(string categoryType);
        Task<List<CategoryEntity?>> GetByTournamentId(int tournamentId);
    }
}
