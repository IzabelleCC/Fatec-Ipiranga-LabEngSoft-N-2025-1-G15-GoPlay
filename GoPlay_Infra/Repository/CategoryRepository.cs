using GoPlay_Core.Entities;
using GoPlay_Core.Repository.Interfaces;

namespace GoPlay_Infra.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        public Task Add(Category entity)
        {
            throw new NotImplementedException();
        }
        public Task Update(Category entity)
        {
            throw new NotImplementedException();
        }
        public Task Delete(Category entity)
        {
            throw new NotImplementedException();
        }
        public Task<List<Category?>> GetAllCategories()
        {
            throw new NotImplementedException();
        }
        public Task<Category?> GetById(int id)
        {
            throw new NotImplementedException();
        }
        public Task<Category?> GetByCategoryType(string categoryType)
        {
            throw new NotImplementedException();
        }
        public Task<List<Category?>> GetByTournamentId(int tournamentId)
        {
            throw new NotImplementedException();
        }
    }
}
