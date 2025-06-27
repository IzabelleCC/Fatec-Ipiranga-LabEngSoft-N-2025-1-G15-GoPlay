using GoPlay_Core.Business.Interfaces;
using GoPlay_Core.Entities;
using GoPlay_Core.Enum;
using GoPlay_Core.Models.Dto;
using GoPlay_Core.Repository.Interfaces;
using GoPlay_Core.Services;
using Microsoft.Extensions.Logging;

namespace GoPlay_Core.Business
{
    public class CategoryPlayerBusiness : ICategoryPlayerBusiness
    {
        private readonly ICategoryPlayerRepository _categoryPlayerRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMatchGroupRepository _matchRepository;
        private readonly ITournamentRepository _tournamentRepository;
        private readonly IUserRepository _userRepository;
        private readonly FirebaseNotificationService _firebaseService;
        private readonly ILogger<CategoryPlayerBusiness> _logger;

        public CategoryPlayerBusiness(
            ICategoryPlayerRepository categoryPlayerRepository,
            ICategoryRepository categoryRepository,
            IMatchGroupRepository matchRepository,
            ITournamentRepository tournamentRepository,
            IUserRepository userRepository,
            FirebaseNotificationService firebaseService,
            ILogger<CategoryPlayerBusiness> logger)
        {
            _categoryPlayerRepository = categoryPlayerRepository ?? throw new ArgumentNullException(nameof(categoryPlayerRepository));
            _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
            _matchRepository = matchRepository ?? throw new ArgumentNullException(nameof(matchRepository));
            _tournamentRepository = tournamentRepository;
            _userRepository = userRepository;
            _firebaseService = firebaseService;
            _logger = logger;
        }

        public async Task<List<CategoryPlayerEntity>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _categoryPlayerRepository.GetAllAsync();
        }

        public async Task<CategoryPlayerEntity?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await _categoryPlayerRepository.GetByIdAsync(id);
        }
        public async Task<CategoryPlayerDto> GetRegistrationDetails(int id, CancellationToken cancellationToken)
        {
            var categoryPlayer = await _categoryPlayerRepository.GetByIdAsync(id);

            if (categoryPlayer == null)
                return new CategoryPlayerDto();

            var firstUser = await _userRepository.GetById(categoryPlayer.FirstUserId) ?? new UserEntity();
            var secondUser = await _userRepository.GetById(categoryPlayer.SecondUserId) ?? new UserEntity();
            var category = await _categoryRepository.GetById(categoryPlayer.CategoryId) ?? new CategoryEntity();
            var tournament = await _tournamentRepository.GetById(category.TournamentId) ?? new TournamentEntity();
            var matchGroups = await _matchRepository.GetbyRegistrationCategoryAsync(categoryPlayer.Id) ?? new MatchGroupEntity();


            return new CategoryPlayerDto
            {
                Id = categoryPlayer.Id,
                TournamentId = tournament.Id,
                TournamentName = tournament.Name ?? string.Empty,
                TournamentPictureUrl = tournament.ProfilePictureUrl ?? string.Empty,
                RegistrationFee = tournament.RegistrationFee,
                PaymentDeadline = tournament.PaymentDeadline,
                CategoryId = category.Id,
                CategoryName = category.CategoryType ?? string.Empty,
                FirstUserId = categoryPlayer.FirstUserId,
                FirstUserName = firstUser.Name ?? string.Empty,
                FirstUserPictureUrl = firstUser.ProfilePictureUrl ?? string.Empty,
                FirstUserPaymentConfirmed = categoryPlayer.FirstUserPaymentConfirmed,
                SecondUserId = categoryPlayer.SecondUserId,
                SecondUserName = secondUser.Name ?? string.Empty,
                SecondUserPictureUrl = secondUser.ProfilePictureUrl ?? string.Empty,
                SecondUserPaymentConfirmed = categoryPlayer.SecondUserPaymentConfirmed,
                RegisterStatus = (int)categoryPlayer.RegisterStatus,
                AttendanceConfirmed = matchGroups.AttendanceConfirmed,
                AttendanceTime = matchGroups.AttendanceTime,
                AttendanceConfirmedUserId = matchGroups.AttendanceConfirmedUserId,
            };
        }

        public async Task<List<CategoryPlayerDto>> GetByCategoryIdAsync(int categoryId, CancellationToken cancellationToken)
        {
            var categoryPlayerList = await _categoryPlayerRepository.GetByCategoryIdAsync(categoryId);

            var categoryPlayerDtoList = new List<CategoryPlayerDto>();
            foreach (var player in categoryPlayerList)
            {
                var firstUser = await _userRepository.GetById(player.FirstUserId);
                var secondUser = await _userRepository.GetById(player.SecondUserId);
                var category = await _categoryRepository.GetById(player.CategoryId);
                var tournament = await _tournamentRepository.GetById(category?.TournamentId ?? 0);
                var dto = new CategoryPlayerDto
                {
                    Id = player.Id,
                    TournamentId = tournament?.Id ?? 0,
                    TournamentName = tournament?.Name ?? string.Empty,
                    TournamentPictureUrl = tournament?.ProfilePictureUrl ?? string.Empty,
                    RegistrationFee = tournament?.RegistrationFee ?? 0,
                    PaymentDeadline = tournament?.PaymentDeadline ?? DateTime.MinValue,
                    CategoryId = category?.Id ?? 0,
                    CategoryName = category?.CategoryType ?? string.Empty,
                    FirstUserId = player.FirstUserId,
                    FirstUserName = firstUser?.Name ?? string.Empty,
                    FirstUserPictureUrl = firstUser?.ProfilePictureUrl ?? string.Empty,
                    FirstUserPaymentConfirmed = player.FirstUserPaymentConfirmed,
                    SecondUserId = player.SecondUserId,
                    SecondUserName = secondUser?.Name ?? string.Empty,
                    SecondUserPictureUrl = secondUser?.ProfilePictureUrl ?? string.Empty,
                    SecondUserPaymentConfirmed = player.SecondUserPaymentConfirmed,
                    RegisterStatus = (int)player.RegisterStatus
                };
                categoryPlayerDtoList.Add(dto);
            }
            return categoryPlayerDtoList;
        }

        public async Task<List<CategoryPlayerEntity>> GetByUserIdAsync(string userId, CancellationToken cancellationToken)
        {
            return await _categoryPlayerRepository.GetByUserIdAsync(userId);
        }

        public async Task<List<CategoryPlayerFullInfoDto>> GetByUserIdAndReturnsFullInfoAsync(string userId, CancellationToken cancellationToken)
        {
            var categoryPlayer = await _categoryPlayerRepository.GetByUserIdAsync(userId);

            var categoryPlayerFullInfo = new List<CategoryPlayerFullInfoDto>();

            foreach (var player in categoryPlayer)
            {
                var dto = new CategoryPlayerFullInfoDto
                {
                    CategoryPlayer = new CategoryPlayerEntity
                    {
                        Id = player.Id,
                        CategoryId = player.CategoryId,
                        FirstUserId = player.FirstUserId,
                        SecondUserId = player.SecondUserId,
                        FirstUserPaymentConfirmed = player.FirstUserPaymentConfirmed,
                        SecondUserPaymentConfirmed = player.SecondUserPaymentConfirmed,
                        RegisterStatus = player.RegisterStatus
                    },
                };
                var firstUser = await _userRepository.GetById(player.FirstUserId);
                var secondUser = await _userRepository.GetById(player.SecondUserId);
                dto.FirstUserName = firstUser.Name ?? string.Empty;
                dto.FirstUserPictureUrl = firstUser.ProfilePictureUrl ?? string.Empty;
                dto.SecondUserName = secondUser.Name ?? string.Empty;
                dto.SecondUserPictureUrl = secondUser.ProfilePictureUrl ?? string.Empty;

                var category = await _categoryRepository.GetById(dto.CategoryPlayer.CategoryId);
                dto.Category = new CategoryEntity
                {
                    Id = category?.Id ?? 0,
                    CategoryType = category?.CategoryType ?? string.Empty,
                    IsDoubles = category?.IsDoubles ?? false,
                    TournamentId = category?.TournamentId ?? 0,
                };
                dto.RegisterCount = category?.CategoryPlayers.Count ?? 0;

                var tournament = await _tournamentRepository.GetById(dto.Category.TournamentId);
                dto.Tournament = new TournamentEntity
                {
                    Id = tournament?.Id ?? 0,
                    Name = tournament?.Name ?? string.Empty,
                    ProfilePictureUrl = tournament?.ProfilePictureUrl ?? string.Empty,
                    Status = tournament.Status,
                    GamesStartDate = tournament?.GamesStartDate ?? DateTime.MinValue,
                    GamesEndDate = tournament?.GamesEndDate ?? DateTime.MinValue,
                    RegistrationDeadline = tournament?.RegistrationDeadline ?? DateTime.MinValue,
                };

                categoryPlayerFullInfo.Add(dto);
            }

            return categoryPlayerFullInfo;
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
                entity.RegisterStatus = RegisterStatusEnum.InscricaoConfirmada;
            }

            await _categoryPlayerRepository.UpdatePlayersAsync(entity);
        }

        public async Task<CategoryPlayerEntity> RegisterUserToCategory(int categoryId, string firstUserId, string? secondUserId, CancellationToken cancellationToken)
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
                RegisterStatus = RegisterStatusEnum.InscricaoRealizada
            };

            await _categoryPlayerRepository.AddAsync(newRegistration);

            var firstUser = await _userRepository.GetById(firstUserId);
            var secondUser = await _userRepository.GetById(secondUserId);
            var tournament = await _tournamentRepository.GetById(category.TournamentId);

            if (secondUser != null)
            {
                if (!string.IsNullOrEmpty(secondUser.FCMToken))
                    throw new InvalidOperationException("O usuário 2 não possui um token Firebase registrado.");
                try
                {
                    await _firebaseService.SendNotificationAsync(
                        secondUser.FCMToken,
                        $"Nova Inscrição -  {tournament.Name}",
                        $"Você foi inscrito por {firstUser.Name} na categoria {category.CategoryType}."
                    );
                }
                catch
                {
                    _logger.LogError(categoryId, "Erro ao enviar notificação para o usuário 2: {UserId}", secondUserId);
                }
            }

            return newRegistration;
        }

    }
}
