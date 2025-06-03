using GoPlay_Core.Business.Interfaces;
using GoPlay_Core.Entities;
using GoPlay_Core.Enum;
using GoPlay_Core.Repository.Interfaces;

namespace GoPlay_Core.Business
{
    public class CategoryPlayerBusiness : ICategoryPlayerBusiness
    {
        private readonly ICategoryPlayerRepository _categoryPlayerRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMatchRepository _matchRepository;
        private readonly ITournamentRepository _tournamentRepository;

        public CategoryPlayerBusiness(
            ICategoryPlayerRepository categoryPlayerRepository,
            ICategoryRepository categoryRepository,
            IMatchRepository matchRepository,
            ITournamentRepository tournamentRepository)
        {
            _categoryPlayerRepository = categoryPlayerRepository ?? throw new ArgumentNullException(nameof(categoryPlayerRepository));
            _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
            _matchRepository = matchRepository ?? throw new ArgumentNullException(nameof(matchRepository));
            _tournamentRepository = tournamentRepository;
        }

        public async Task<List<CategoryPlayerEntity>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _categoryPlayerRepository.GetAllAsync();
        }

        public async Task<CategoryPlayerEntity?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await _categoryPlayerRepository.GetByIdAsync(id);
        }

        public async Task<List<CategoryPlayerEntity>> GetByCategoryIdAsync(int categoryId, CancellationToken cancellationToken)
        {
            return await _categoryPlayerRepository.GetByCategoryIdAsync(categoryId);
        }

        public async Task<List<CategoryPlayerEntity>> GetByUserIdAsync(string userId, CancellationToken cancellationToken)
        {
            return await _categoryPlayerRepository.GetByUserIdAsync(userId);
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken)
        {
            await _categoryPlayerRepository.DeleteAsync(id);
        }

        public async Task UpdatePlayersAsync(CategoryPlayerEntity entity, CancellationToken cancellationToken)
        {
            if (entity.FirstUserId == entity.SecondUserId)
                throw new InvalidOperationException("O mesmo jogador não pode ocupar as duas posições na dupla.");

            bool firstPaid = entity.FirstUserPaymentConfirmed;
            bool secondPaid = entity.SecondUserPaymentConfirmed;
            bool isDupla = !string.IsNullOrEmpty(entity.SecondUserId);

            if ((firstPaid && secondPaid && isDupla) || (firstPaid && !isDupla))
            {
                entity.RegisterStatus = RegisterStatus.InscricaoConfirmada;
            }

            await _categoryPlayerRepository.UpdatePlayersAsync(entity);
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

            var newRegistration = new CategoryPlayerEntity
            {
                CategoryId = categoryId,
                FirstUserId = firstUserId,
                SecondUserId = secondUserId,
                FirstUserPaymentConfirmed = false,
                SecondUserPaymentConfirmed = false,
                RegisterStatus = RegisterStatus.InscricaoRealizada
            };

            await _categoryPlayerRepository.AddAsync(newRegistration);
        }

    }
}
