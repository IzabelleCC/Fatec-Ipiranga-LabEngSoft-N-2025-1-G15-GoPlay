using GoPlay_Core.Business.Interfaces;
using GoPlay_Core.Entities;
using GoPlay_Core.Repository.Interfaces;

namespace GoPlay_Core.Business
{
    public class CategoryBusiness : ICategoryBusiness<CategoryEntity>
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ICategoryPlayerRepository _categoryPlayerRepository;

        public CategoryBusiness(ICategoryRepository categoryRepository, ICategoryPlayerRepository categoryPlayerRepository)
        {
            _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
            _categoryPlayerRepository = categoryPlayerRepository ?? throw new ArgumentNullException(nameof(categoryPlayerRepository));
        }

        public async Task Add(CategoryEntity entity, CancellationToken cancellationToken)
        {
            await _categoryRepository.Add(entity);
        }

        public async Task Delete(CategoryEntity entity, CancellationToken cancellationToken)
        {
            await _categoryRepository.Delete(entity);
        }

        public async Task<List<CategoryEntity?>> GetAllCategories(CancellationToken cancellationToken)
        {
            return await _categoryRepository.GetAll();
        }

        public async Task<CategoryEntity?> GetByCategoryType(string categoryType, CancellationToken cancellationToken)
        {
            return await _categoryRepository.GetByCategoryType(categoryType);
        }

        public async Task<CategoryEntity?> GetById(int id, CancellationToken cancellationToken)
        {
            return await _categoryRepository.GetById(id);
        }

        public async Task<List<CategoryEntity?>> GetByTournamentId(int tournamentId, CancellationToken cancellationToken)
        {
            return await _categoryRepository.GetByTournamentId(tournamentId);
        }

        public async Task Update(CategoryEntity entity, CancellationToken cancellationToken)
        {
            await _categoryRepository.Update(entity);
        }

        public async Task RegisterUserToCategory(int categoryId, string firstUserId, string? secondUserId, CancellationToken cancellationToken)
        {
            var category = await _categoryRepository.GetById(categoryId);
            if (category == null)
                throw new KeyNotFoundException("Categoria não encontrada.");

            if (category.CategoryPlayers.Count >= category.PlayerLimit)
                throw new InvalidOperationException("Limite de duplas atingido para essa categoria.");

            bool jogador1JaInscrito = category.CategoryPlayers
                .Any(cp => cp.FirstUserId == firstUserId || cp.SecondUserId == firstUserId);

            if (jogador1JaInscrito)
                throw new InvalidOperationException("O jogador 1 já está inscrito nesta categoria.");

            if (!string.IsNullOrEmpty(secondUserId))
            {
                if (firstUserId == secondUserId)
                    throw new InvalidOperationException("O mesmo jogador não pode formar uma dupla com ele mesmo.");

                bool jogador2JaInscrito = category.CategoryPlayers
                    .Any(cp => cp.FirstUserId == secondUserId || cp.SecondUserId == secondUserId);

                if (jogador2JaInscrito)
                    throw new InvalidOperationException("O jogador 2 já está inscrito nesta categoria.");
            }

            var novaInscricao = new CategoryPlayerEntity
            {
                CategoryId = categoryId,
                FirstUserId = firstUserId,
                SecondUserId = secondUserId
            };

            await _categoryPlayerRepository.AddAsync(novaInscricao);
        }
    }
}
