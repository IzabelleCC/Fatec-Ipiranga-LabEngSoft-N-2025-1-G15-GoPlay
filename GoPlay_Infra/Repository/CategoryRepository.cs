using GoPlay_Core.Entities;
using GoPlay_Core.Repository.Interfaces;

namespace GoPlay_Infra.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        public Task Add(CategoryEntity entity)
        {
            throw new NotImplementedException();
        }
        public Task Update(CategoryEntity entity)
        {
            throw new NotImplementedException();
        }
        public Task Delete(CategoryEntity entity)
        {
            throw new NotImplementedException();
        }
        public Task<List<CategoryEntity?>> GetAllCategories()
        {
            throw new NotImplementedException();
        }
        public Task<CategoryEntity?> GetById(int id)
        {
            throw new NotImplementedException();
        }
        public Task<CategoryEntity?> GetByCategoryType(string categoryType)
        {
            throw new NotImplementedException();
        }
        public Task<List<CategoryEntity?>> GetByTournamentId(int tournamentId)
        {
            throw new NotImplementedException();
        }
    }
}
