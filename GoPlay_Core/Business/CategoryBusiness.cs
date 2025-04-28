using GoPlay_Core.Business.Interfaces;
using GoPlay_Core.Repository.Interfaces;

namespace GoPlay_Core.Business
{
    public class CategoryBusiness : ICategoryBusiness<CategoryEntity>
    {
        private readonly ICategoryRepository _repository;
        public CategoryBusiness(ICategoryRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }
        public async Task Add(CategoryEntity entity, CancellationToken cancellationToken)
        {
            await _repository.Add(entity);
        }
        public async Task Delete(CategoryEntity entity, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
        public async Task<List<CategoryEntity?>> GetAllCategories(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
        public async Task<CategoryEntity?> GetByCategoryType(string categoryType, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
        public async Task<CategoryEntity?> GetById(int id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
        public async Task<List<CategoryEntity?>> GetByTournamentId(int tournamentId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
        public async Task Update(CategoryEntity entity, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
