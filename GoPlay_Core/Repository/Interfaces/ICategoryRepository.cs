using GoPlay_Core.Entities;

namespace GoPlay_Core.Repository.Interfaces
{
    public interface ICategoryRepository
    {
        Task Add(Category entity);
        Task Update(Category entity);
        Task Delete(Category entity);
        Task<List<Category?>> GetAllCategories();
        Task<Category?> GetById(int id);
        Task<Category?> GetByCategoryType(string categoryType);
        Task<List<Category?>> GetByTournamentId(int tournamentId);
    }
}
