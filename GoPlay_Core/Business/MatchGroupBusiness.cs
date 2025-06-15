using System.Threading;
using GoPlay_Core.Business.Interfaces;
using GoPlay_Core.Entities;
using GoPlay_Core.Enum;
using GoPlay_Core.Models;
using GoPlay_Core.Models.Dto;
using GoPlay_Core.Repository.Interfaces;
using GoPlay_Core.Utils;
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
        private readonly IGameMatchBusiness _gameMatchBusiness;

        public MatchGroupBusiness(
            ITournamentRepository tournamentRepository,
            ICategoryRepository categoryRepository,
            ILogger<MatchGroupBusiness> logger,
            IUserRepository userRepository,
            IMatchGroupRepository matchRepository,
            IGameMatchBusiness gameMatchBusiness)
        {
            _tournamentRepository = tournamentRepository;
            _categoryRepository = categoryRepository;
            _logger = logger;
            _userRepository = userRepository;
            _matchRepository = matchRepository;
            _gameMatchBusiness = gameMatchBusiness;
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
            tournament.Status = TournamentStatusEnum.ChavesPublicadas;
            await _tournamentRepository.Update(tournament);
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
                .Where(p => p.RegisterStatus == RegisterStatusEnum.InscricaoConfirmada)
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
                        AttendanceConfirmed = false,
                        MatchStage = MatchStageEnum.Group
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
            _logger.LogInformation("Validando proximidade para check-in nas coordenadas ({Latitude}, {Longitude})...", latitude, longitude);

            var tournament = await _tournamentRepository.GetById(tournamentId);
            if (tournament == null)
                throw new InvalidOperationException("Torneio não encontrado.");

            var distance = GeoUtils.CalculateDistanceInMeters(latitude, longitude, tournament.Latitude, tournament.Longitude);

            if (distance > 300)
            {
                _logger.LogWarning("Usuário está a {Distance} metros do torneio (limite: 300m).", distance);
                throw new InvalidOperationException("Não é possível confirmar a presença a uma distância maior que 300 metros do torneio.");
            }

            _logger.LogInformation("Proximidade validada com sucesso: {Distance} metros.", distance);
            return true;
        }

        public async Task<List<GameMatchEntity>> InsertGroupResultsAndReturnWinners(List<MatchGroupEntity> results, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Inserting group results and determining winners...");
            if (results == null || !results.Any())
            {
                _logger.LogWarning("No results provided for group matches.");
                throw new InvalidOperationException("No results provided for group matches.");
            }

            var doublesOrSinglesGroup = new List<MatchGroupEntity>();

            foreach (var result in results)
            {
                var DoublesOrSingles = await _matchRepository.GetbyRegistrationCategoryAsync(result.RegistrationCategoryId);
                if (DoublesOrSingles == null)
                {
                    _logger.LogWarning("Player with RegistrationCategoryId {RegistrationCategoryId} not found.", result.RegistrationCategoryId);
                    throw new KeyNotFoundException($"Player with RegistrationCategoryId {result.RegistrationCategoryId} not found.");
                }
                DoublesOrSingles.Game1 = result.Game1;
                DoublesOrSingles.Game2 = result.Game2;
                DoublesOrSingles.Game3 = result.Game3;
                DoublesOrSingles.Game4 = result.Game4;
                DoublesOrSingles.Game5 = result.Game5;
                DoublesOrSingles.Game6 = result.Game6;
                DoublesOrSingles.Game7 = result.Game7;
                DoublesOrSingles.Game8 = result.Game8;
                DoublesOrSingles.Game9 = result.Game9;
                DoublesOrSingles.Game10 = result.Game10;
                await _matchRepository.UpdateAsync(DoublesOrSingles);

                var updatedDoublesOrSingles = await _matchRepository.GetbyRegistrationCategoryAsync(result.RegistrationCategoryId);
                doublesOrSinglesGroup.Add(updatedDoublesOrSingles);
            }

            await InsertSetsAndGamesBalance(doublesOrSinglesGroup);

            var groupedResults = doublesOrSinglesGroup
                .GroupBy(r => new { r.CategoryId, r.GroupNumber })
                .Select(g => new
                {
                    CategoryId = g.Key.CategoryId,
                    GroupNumber = g.Key.GroupNumber,
                    Winners = g.OrderByDescending(r => r.Wins)
                               .ThenByDescending(r => r.SetsBalance)
                               .ThenByDescending(r => r.GamesBalance)
                               .ThenByDescending(r => r.Tiebreaks)
                               .Select(r => r.RegistrationCategoryId)
                               .ToList()
                });

            var groupPositions = groupedResults.SelectMany(g => g.Winners).ToList();

            for (int i = 0; i < groupPositions.Count; i++)
            {

                var matchGroup = doublesOrSinglesGroup.FirstOrDefault(r => r.RegistrationCategoryId == groupPositions[i]);
                if (matchGroup != null)
                {
                    matchGroup.Position = i + 1;
                    await _matchRepository.UpdateAsync(matchGroup);
                }
            }

            return await _gameMatchBusiness.GenerateEliminationMatches(groupedResults.FirstOrDefault().CategoryId, CancellationToken.None);
        }

        private async Task InsertSetsAndGamesBalance(List<MatchGroupEntity> doublesOrSinglesGroup)
        {
            var game1 = doublesOrSinglesGroup.Where(r => r.Game1 != null).ToList();
            var game2 = doublesOrSinglesGroup.Where(r => r.Game2 != null).ToList();
            var game3 = doublesOrSinglesGroup.Where(r => r.Game3 != null).ToList();
            var game4 = doublesOrSinglesGroup.Where(r => r.Game4 != null).ToList();
            var game5 = doublesOrSinglesGroup.Where(r => r.Game5 != null).ToList();
            var game6 = doublesOrSinglesGroup.Where(r => r.Game6 != null).ToList();
            var game7 = doublesOrSinglesGroup.Where(r => r.Game7 != null).ToList();
            var game8 = doublesOrSinglesGroup.Where(r => r.Game8 != null).ToList();
            var game9 = doublesOrSinglesGroup.Where(r => r.Game9 != null).ToList();
            var game10 = doublesOrSinglesGroup.Where(r => r.Game10 != null).ToList();

            if (game1.Count == 2)
            {
                game1[0].SumOfGamesWon = (game1[0].SumOfGamesWon ?? 0) + (game1[0].Game1 ?? 0);
                game1[0].SumOfGamesLost = (game1[0].SumOfGamesLost ?? 0) + (game1[1].Game1 ?? 0);
                game1[1].SumOfGamesWon = (game1[1].SumOfGamesWon ?? 0) + (game1[1].Game1 ?? 0);
                game1[1].SumOfGamesLost = (game1[1].SumOfGamesLost ?? 0) + (game1[0].Game1 ?? 0);
                if ((game1[0].Game1 ?? 0) > (game1[1].Game1 ?? 0))
                {
                    game1[0].Wins++;
                    game1[1].Losses++;
                }
                else
                {
                    game1[0].Losses++;
                    game1[1].Wins++;
                }
                await _matchRepository.UpdateAsync(game1[0]);
                await _matchRepository.UpdateAsync(game1[1]);
            }
            if (game2.Count == 2)
            {
                game2[0].SumOfGamesWon = (game2[0].SumOfGamesWon ?? 0) + (game2[0].Game2 ?? 0);
                game2[0].SumOfGamesLost = (game2[0].SumOfGamesLost ?? 0) + (game2[1].Game2 ?? 0);
                game2[1].SumOfGamesWon = (game2[1].SumOfGamesWon ?? 0) + (game2[1].Game2 ?? 0);
                game2[1].SumOfGamesLost = (game2[1].SumOfGamesLost ?? 0) + (game2[0].Game2 ?? 0);
                if ((game2[0].Game2 ?? 0) > (game2[1].Game2 ?? 0))
                {
                    game2[0].Wins++;
                    game2[1].Losses++;
                }
                else
                {
                    game2[0].Losses++;
                    game2[1].Wins++;
                }
                await _matchRepository.UpdateAsync(game2[0]);
                await _matchRepository.UpdateAsync(game2[1]);
            }
            if (game3.Count == 2)
            {
                game3[0].SumOfGamesWon = (game3[0].SumOfGamesWon ?? 0) + (game3[0].Game3 ?? 0);
                game3[0].SumOfGamesLost = (game3[0].SumOfGamesLost ?? 0) + (game3[1].Game3 ?? 0);
                game3[1].SumOfGamesWon = (game3[1].SumOfGamesWon ?? 0) + (game3[1].Game3 ?? 0);
                game3[1].SumOfGamesLost = (game3[1].SumOfGamesLost ?? 0) + (game3[0].Game3 ?? 0);
                if ((game3[0].Game3 ?? 0) > (game3[1].Game3 ?? 0))
                {
                    game3[0].Wins++;
                    game3[1].Losses++;
                }
                else
                {
                    game3[0].Losses++;
                    game3[1].Wins++;
                }
                await _matchRepository.UpdateAsync(game3[0]);
                await _matchRepository.UpdateAsync(game3[1]);
            }
            if (game4.Count == 2)
            {
                game4[0].SumOfGamesWon = (game4[0].SumOfGamesWon ?? 0) + (game4[0].Game4 ?? 0);
                game4[0].SumOfGamesLost = (game4[0].SumOfGamesLost ?? 0) + (game4[1].Game4 ?? 0);
                game4[1].SumOfGamesWon = (game4[1].SumOfGamesWon ?? 0) + (game4[1].Game4 ?? 0);
                game4[1].SumOfGamesLost = (game4[1].SumOfGamesLost ?? 0) + (game4[0].Game4 ?? 0);
                if ((game4[0].Game4 ?? 0) > (game4[1].Game4 ?? 0))
                {
                    game4[0].Wins++;
                    game4[1].Losses++;
                }
                else
                {
                    game4[0].Losses++;
                    game4[1].Wins++;
                }
                await _matchRepository.UpdateAsync(game4[0]);
                await _matchRepository.UpdateAsync(game4[1]);
            }
            if (game5.Count == 2)
            {
                game5[0].SumOfGamesWon = (game5[0].SumOfGamesWon ?? 0) + (game5[0].Game5 ?? 0);
                game5[0].SumOfGamesLost = (game5[0].SumOfGamesLost ?? 0) + (game5[1].Game5 ?? 0);
                game5[1].SumOfGamesWon = (game5[1].SumOfGamesWon ?? 0) + (game5[1].Game5 ?? 0);
                game5[1].SumOfGamesLost = (game5[1].SumOfGamesLost ?? 0) + (game5[0].Game5 ?? 0);
                if ((game5[0].Game5 ?? 0) > (game5[1].Game5 ?? 0))
                {
                    game5[0].Wins++;
                    game5[1].Losses++;
                }
                else
                {
                    game5[0].Losses++;
                    game5[1].Wins++;
                }
                await _matchRepository.UpdateAsync(game5[0]);
                await _matchRepository.UpdateAsync(game5[1]);
            }
            if (game6.Count == 2)
            {
                game6[0].SumOfGamesWon = (game6[0].SumOfGamesWon ?? 0) + (game6[0].Game6 ?? 0);
                game6[0].SumOfGamesLost = (game6[0].SumOfGamesLost ?? 0) + (game6[1].Game6 ?? 0);
                game6[1].SumOfGamesWon = (game6[1].SumOfGamesWon ?? 0) + (game6[1].Game6 ?? 0);
                game6[1].SumOfGamesLost = (game6[1].SumOfGamesLost ?? 0) + (game6[0].Game6 ?? 0);
                if ((game6[0].Game6 ?? 0) > (game6[1].Game6 ?? 0))
                {
                    game6[0].Wins++;
                    game6[1].Losses++;
                }
                else
                {
                    game6[0].Losses++;
                    game6[1].Wins++;
                }
                await _matchRepository.UpdateAsync(game6[0]);
                await _matchRepository.UpdateAsync(game6[1]);
            }
            if (game7.Count == 2)
            {
                game7[0].SumOfGamesWon = (game7[0].SumOfGamesWon ?? 0) + (game7[0].Game7 ?? 0);
                game7[0].SumOfGamesLost = (game7[0].SumOfGamesLost ?? 0) + (game7[1].Game7 ?? 0);
                game7[1].SumOfGamesWon = (game7[1].SumOfGamesWon ?? 0) + (game7[1].Game7 ?? 0);
                game7[1].SumOfGamesLost = (game7[1].SumOfGamesLost ?? 0) + (game7[0].Game7 ?? 0);
                if ((game7[0].Game7 ?? 0) > (game7[1].Game7 ?? 0))
                {
                    game7[0].Wins++;
                    game7[1].Losses++;
                }
                else
                {
                    game7[0].Losses++;
                    game7[1].Wins++;
                }
                await _matchRepository.UpdateAsync(game7[0]);
                await _matchRepository.UpdateAsync(game7[1]);
            }
            if (game8.Count == 2)
            {
                game8[0].SumOfGamesWon = (game8[0].SumOfGamesWon ?? 0) + (game8[0].Game8 ?? 0);
                game8[0].SumOfGamesLost = (game8[0].SumOfGamesLost ?? 0) + (game8[1].Game8 ?? 0);
                game8[1].SumOfGamesWon = (game8[1].SumOfGamesWon ?? 0) + (game8[1].Game8 ?? 0);
                game8[1].SumOfGamesLost = (game8[1].SumOfGamesLost ?? 0) + (game8[0].Game8 ?? 0);
                if ((game8[0].Game8 ?? 0) > (game8[1].Game8 ?? 0))
                {
                    game8[0].Wins++;
                    game8[1].Losses++;
                }
                else
                {
                    game8[0].Losses++;
                    game8[1].Wins++;
                }
                await _matchRepository.UpdateAsync(game8[0]);
                await _matchRepository.UpdateAsync(game8[1]);
            }
            if (game9.Count == 2)
            {
                game9[0].SumOfGamesWon = (game9[0].SumOfGamesWon ?? 0) + (game9[0].Game9 ?? 0);
                game9[0].SumOfGamesLost = (game9[0].SumOfGamesLost ?? 0) + (game9[1].Game9 ?? 0);
                game9[1].SumOfGamesWon = (game9[1].SumOfGamesWon ?? 0) + (game9[1].Game9 ?? 0);
                game9[1].SumOfGamesLost = (game9[1].SumOfGamesLost ?? 0) + (game9[0].Game9 ?? 0);
                if ((game9[0].Game9 ?? 0) > (game9[1].Game9 ?? 0))
                {
                    game9[0].Wins++;
                    game9[1].Losses++;
                }
                else
                {
                    game9[0].Losses++;
                    game9[1].Wins++;
                }
                await _matchRepository.UpdateAsync(game9[0]);
                await _matchRepository.UpdateAsync(game9[1]);
            }
            if (game10.Count == 2)
            {
                game10[0].SumOfGamesWon = (game10[0].SumOfGamesWon ?? 0) + (game10[0].Game10 ?? 0);
                game10[0].SumOfGamesLost = (game10[0].SumOfGamesLost ?? 0) + (game10[1].Game10 ?? 0);
                game10[1].SumOfGamesWon = (game10[1].SumOfGamesWon ?? 0) + (game10[1].Game10 ?? 0);
                game10[1].SumOfGamesLost = (game10[1].SumOfGamesLost ?? 0) + (game10[0].Game10 ?? 0);
                if ((game10[0].Game10 ?? 0) > (game10[1].Game10 ?? 0))
                {
                    game10[0].Wins++;
                    game10[1].Losses++;
                }
                else
                {
                    game10[0].Losses++;
                    game10[1].Wins++;
                }
                await _matchRepository.UpdateAsync(game10[0]);
                await _matchRepository.UpdateAsync(game10[1]);
            }

            foreach (var match in doublesOrSinglesGroup)
            {
                match.GamesBalance = (match.SumOfGamesWon) - (match.SumOfGamesLost);
                match.SetsBalance = (match.Wins) - (match.Losses);
                await _matchRepository.UpdateAsync(match);
            }
        }

        public async Task<TournamentMatchesResultDto> GetTournamentMatchesByCategory(int categoryId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching match groups for category ID {CategoryId}...", categoryId);

            var matchGroups = await _matchRepository.GetTournamentMatchesByCategory(categoryId);

            if (matchGroups == null)
            {
                _logger.LogWarning("No match groups found for category ID {CategoryId}.", categoryId);
                throw new KeyNotFoundException("No match groups found for the specified category.");
            }
            return matchGroups;
        }

        public async Task<List<MatchDto>> GetGroupResultByCategoryId(int categoryId, int groupNumber, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching group result for category ID {CategoryId} and group number {GroupNumber}...", categoryId, groupNumber);

            var matchGroup = await _matchRepository.GetGroupResultByCategoryId(categoryId,  groupNumber);

            if (matchGroup == null)
            {
                _logger.LogWarning("No match group found for category ID {CategoryId} and group number {GroupNumber}.", categoryId, groupNumber);
                throw new KeyNotFoundException("No match group found for the specified category and group number.");
            }

            return matchGroup;
        }
    }
}

