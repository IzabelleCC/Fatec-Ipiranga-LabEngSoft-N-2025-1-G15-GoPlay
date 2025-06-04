using GoPlay_Core.Business.Interfaces;
using GoPlay_Core.Entities;
using GoPlay_Core.Enum;
using GoPlay_Core.Models;
using GoPlay_Core.Models.Dto;
using GoPlay_Core.Repository.Interfaces;
using Microsoft.Extensions.Logging;

namespace GoPlay_Core.Business
{
    public class MatchGroupBusiness : IMatchGroupBusiness
    {
        private readonly ILogger<MatchGroupBusiness> _logger;
        private readonly ITournamentRepository _tournamentRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMatchGroupRepository _matchRepository;

        public MatchGroupBusiness(
            ITournamentRepository tournamentRepository,
            ICategoryRepository categoryRepository,
            ILogger<MatchGroupBusiness> logger,
            IUserRepository userRepository,
            IMatchGroupRepository matchRepository)
        {
            _tournamentRepository = tournamentRepository;
            _categoryRepository = categoryRepository;
            _logger = logger;
            _userRepository = userRepository;
            _matchRepository = matchRepository;
        }

        public async Task<TournamentMatchesResultDto> GenerateMatchesForTournament(int tournamentId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting match group generation for tournament {TournamentId}...", tournamentId);

            var tournament = await GetTournament(tournamentId);
            var categories = await GetCategoriesByTournament(tournamentId);

            var result = new TournamentMatchesResultDto
            {
                TournamentName = tournament.Name,
                Groups = new List<CategoryGroupsDto>()
            };

            foreach (var category in categories)
            {
                var categoryDto = await ProcessCategory(category);
                if (categoryDto != null)
                    result.Groups.Add(categoryDto);
            }

            if (!result.Groups.Any())
            {
                _logger.LogWarning("No groups could be generated for tournament {TournamentId}.", tournamentId);
                throw new InvalidOperationException("No groups could be generated from the provided data.");
            }

            _logger.LogInformation("Match group generation completed for tournament {TournamentId}.", tournamentId);
            return result;
        }

        public async Task<TournamentEntity> GetTournament(int tournamentId)
        {
            var tournament = await _tournamentRepository.GetById(tournamentId);
            if (tournament == null)
            {
                _logger.LogWarning("Tournament with ID {TournamentId} not found.", tournamentId);
                throw new KeyNotFoundException("Tournament not found.");
            }
            return tournament;
        }

        public async Task<List<CategoryEntity>> GetCategoriesByTournament(int tournamentId)
        {
            var categories = await _categoryRepository.GetByTournamentId(tournamentId);
            if (categories == null || !categories.Any())
            {
                _logger.LogWarning("No categories found for tournament {TournamentId}.", tournamentId);
                throw new InvalidOperationException("No categories found for this tournament.");
            }
            return categories;
        }

        public async Task<CategoryGroupsDto?> ProcessCategory(CategoryEntity category)
        {
            _logger.LogInformation("Processing category ID {CategoryId}...", category.Id);

            var confirmed = category.CategoryPlayers
                .Where(p => p.RegisterStatus == RegisterStatus.InscricaoConfirmada)
                .ToList();

            if (confirmed.Count < 3)
            {
                _logger.LogInformation("Category ID {CategoryId} skipped: fewer than 3 confirmed registrations.", category.Id);
                return null;
            }

            var distributedGroups = DistributeIntoGroups(confirmed);

            _logger.LogInformation("Category ID {CategoryId}: {GroupCount} group(s) generated.", category.Id, distributedGroups.Count);

            var categoryDto = new CategoryGroupsDto
            {
                CategoryId = category.Id,
                Groups = new List<GroupDto>()
            };

            for (int i = 0; i < distributedGroups.Count; i++)
            {
                var group = distributedGroups[i];
                var groupDto = new GroupDto
                {
                    GroupNumber = i + 1,
                    Players = new List<GroupPlayerDto>()
                };

                foreach (var p in group)
                {
                    var firstUser = await _userRepository.GetById(p.FirstUserId);
                    var secondUser = !string.IsNullOrEmpty(p.SecondUserId) ? await _userRepository.GetById(p.SecondUserId) : null;

                    groupDto.Players.Add(new GroupPlayerDto
                    {
                        Id = p.Id,
                        FirstUserId = p.FirstUserId,
                        FirstUserName = firstUser?.UserName ?? "Unknown",
                        SecondUserId = p.SecondUserId,
                        SecondUserName = secondUser?.UserName
                    });

                    var registration = new MatchGroupEntity
                    {
                        CategoryId = category.Id,
                        GroupNumber = i + 1,
                        RegistrationCategoryId = p.Id,
                        ScheduledAt = null, // Scheduled time can be set later
                        Result = null,
                        AttendanceConfirmed = false,
                    };

                    await _matchRepository.AddAsync(registration);
                }

                categoryDto.Groups.Add(groupDto);
            }

            return categoryDto;
        }

        public List<List<CategoryPlayerEntity>> DistributeIntoGroups(List<CategoryPlayerEntity> confirmed)
        {
            var total = confirmed.Count;
            var groups = new List<List<CategoryPlayerEntity>>();

            if (total < 3)
                return groups;

            var shuffledList = confirmed.OrderBy(_ => Guid.NewGuid()).ToList();

            if (total == 4 || total == 5)
            {
                groups.Add(shuffledList);
                return groups;
            }

            int groupOf3 = total / 3;
            int remainder = total % 3;
            int groupOf4 = 0;

            if (remainder == 1 && groupOf3 >= 1)
            {
                groupOf3 -= 1;
                groupOf4 += 1;
            }
            else if (remainder == 2 && groupOf3 >= 2)
            {
                groupOf3 -= 2;
                groupOf4 += 2;
            }

            int index = 0;

            for (int i = 0; i < groupOf4; i++)
            {
                groups.Add(shuffledList.Skip(index).Take(4).ToList());
                index += 4;
            }

            for (int i = 0; i < groupOf3; i++)
            {
                groups.Add(shuffledList.Skip(index).Take(3).ToList());
                index += 3;
            }

            return groups;
        }

        public async Task<bool> ConfirmAttendance(int registrationCategoryId, double latitude, double longitude, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Confirming attendance for registration ID {RegistrationCategoryId}...", registrationCategoryId);

            var registration = await _matchRepository.GetByIdAsync(registrationCategoryId);
            if (registration == null)
            {
                _logger.LogWarning("Registration with ID {RegistrationCategoryId} not found.", registrationCategoryId);
                throw new KeyNotFoundException("Registration not found.");
            }

            var category = await _categoryRepository.GetById(registration.CategoryId);

            var confirmed = await ValidateProximityForCheckIn(latitude, longitude, category.TournamentId);

            if (!confirmed)
            {
                _logger.LogWarning("Proximity validation failed for registration ID {RegistrationCategoryId}.", registrationCategoryId);
                throw new InvalidOperationException("Proximity validation failed. Attendance cannot be confirmed.");
            }
            registration.AttendanceConfirmed = true;
            await _matchRepository.UpdateAsync(registration);

            _logger.LogInformation("Attendance confirmed for registration ID {RegistrationCategoryId}.", registrationCategoryId);
            return true;

        }

        public async Task<bool> ValidateProximityForCheckIn(double latitude, double longitude, int tournamentId)
        {
            _logger.LogInformation("Validating proximity for check-in at coordinates ({Latitude}, {Longitude})...", latitude, longitude);
            var tournament = await _tournamentRepository.GetById(tournamentId);


            // Here you would implement the logic to validate the proximity based on your requirements.
            // For now, we will assume the validation is successful.
            // You can replace this with actual logic to check if the coordinates are within a valid range.
            _logger.LogInformation("Proximity validation successful for coordinates ({Latitude}, {Longitude}).", latitude, longitude);
            return true;

        }

    }
}
