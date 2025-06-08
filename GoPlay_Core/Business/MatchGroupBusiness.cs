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
        private readonly IGameMatchRepository _gameMatchRepository;

        public MatchGroupBusiness(
            ITournamentRepository tournamentRepository,
            ICategoryRepository categoryRepository,
            ILogger<MatchGroupBusiness> logger,
            IUserRepository userRepository,
            IMatchGroupRepository matchRepository,
            IGameMatchRepository gameMatchRepository)
        {
            _tournamentRepository = tournamentRepository;
            _categoryRepository = categoryRepository;
            _logger = logger;
            _userRepository = userRepository;
            _matchRepository = matchRepository;
            _gameMatchRepository = gameMatchRepository;
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

        public async Task<List<int>> InsertGroupResultsAndReturnWinners(List<MatchGroupEntity> results, CancellationToken cancellationToken)
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


            return groupedResults.SelectMany(g => g.Winners).Take(2).ToList();
        }

        private async Task InsertSetsAndGamesBalance(List<MatchGroupEntity> doublesOrSinglesGroup)
        {
            var game1 = doublesOrSinglesGroup.Where(r => r.Game1 != null).ToList();
            var game2 = doublesOrSinglesGroup.Where(r => r.Game2 != null).ToList();
            var game3 = doublesOrSinglesGroup.Where(r => r.Game3 != null).ToList();
            var game4 = doublesOrSinglesGroup.Where(r => r.Game4 != null).ToList();
            var game5 = doublesOrSinglesGroup.Where(r => r.Game5 != null).ToList();

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

            foreach (var match in doublesOrSinglesGroup)
            {
                match.GamesBalance = (match.SumOfGamesWon) - (match.SumOfGamesLost);
                match.SetsBalance = (match.Wins) - (match.Losses);
                await _matchRepository.UpdateAsync(match);
            }
        }

        public async Task<List<GameMatchEntity>> GenerateEliminationMatches(int categoryId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Generating elimination matches for category ID {CategoryId}...", categoryId);

            // Buscar os classificados
            var matches = await _matchRepository.GetByCategoryAsync(categoryId);
            var numberGame = matches.Count;

            if (matches == null || !matches.Any())
            {
                _logger.LogWarning("No matches found for category ID {CategoryId}.", categoryId);
                throw new InvalidOperationException("No matches found for this category.");
            }

            var winners = matches
                .Where(m => m.Position == 1 || m.Position == 2)
                .ToList();

            if (!winners.Any())
            {
                _logger.LogWarning("No winners found for category ID {CategoryId}.", categoryId);
                throw new InvalidOperationException("No winners found for this category.");
            }

            // Definir MatchStage com base no número de classificados
            var matchStage = MatchStageEnum.Undefined;
            switch (winners.Count)
            {
                case 2: matchStage = MatchStageEnum.Final; break;
                case 4: matchStage = MatchStageEnum.SemiFinal; break;
                case 8: matchStage = MatchStageEnum.QuarterFinal; break;
                case 16: matchStage = MatchStageEnum.RoundOf16; break;
                case 32: matchStage = MatchStageEnum.RoundOf32; break;
            }

            // Separar 1º e 2º de cada grupo
            var groupedWinners = winners
                .GroupBy(w => w.GroupNumber)
                .ToDictionary(g => g.Key, g => g.OrderBy(w => w.Position).ToList());

            var firstPlaceds = new List<MatchGroupEntity>();
            var secondPlaceds = new List<MatchGroupEntity>();

            foreach (var group in groupedWinners)
            {
                if (group.Value.Count < 2)
                    throw new InvalidOperationException($"Group {group.Key} does not have at least 2 winners.");

                firstPlaceds.Add(group.Value.First(w => w.Position == 1));
                secondPlaceds.Add(group.Value.First(w => w.Position == 2));
            }

            List<GameMatchEntity> matchesToCreate;

            // Se for o cenário 4 grupos / 8 classificados => aplicar a lógica fixa
            if (firstPlaceds.Count == 4 && secondPlaceds.Count == 4)
            {
                _logger.LogInformation("Applying fixed group cross rule for 4 groups...");
                matchesToCreate = GenerateFixedGroupCrossMatches(firstPlaceds, secondPlaceds, matchStage, ref numberGame);
            }
            else
            {
                // Caso contrário, manter lógica genérica (se desejar você pode criar outro método ou aqui fazer sua lógica padrão)
                throw new InvalidOperationException("This version supports only the fixed group cross rule for 4 groups.");
            }

            // Salvar confrontos
            foreach (var game in matchesToCreate)
            {
                await _gameMatchRepository.AddAsync(game);
            }

            _logger.LogInformation("Elimination matches created for category ID {CategoryId}: {Count} match(es) created.", categoryId, matchesToCreate.Count);

            return matchesToCreate;
        }


        private GameMatchEntity CreateGameMatch(MatchGroupEntity competitor1, MatchGroupEntity? competitor2, MatchStageEnum matchStage, int numberGame)
        {
            return new GameMatchEntity
            {
                MatchStage = matchStage,
                NumberGame = numberGame,
                Competitor1Id = competitor1.RegistrationCategoryId,
                Competitor2Id = competitor2?.RegistrationCategoryId ?? null,
            };
        }

        private List<GameMatchEntity> GenerateFixedGroupCrossMatches(
            List<MatchGroupEntity> firstPlaceds,
            List<MatchGroupEntity> secondPlaceds,
            MatchStageEnum matchStage,
            ref int numberGame)
        {
            var matchesToCreate = new List<GameMatchEntity>();

            int expectedGroupCount = matchStage switch
            {
                MatchStageEnum.RoundOf32 => 16,
                MatchStageEnum.RoundOf16 => 8,
                MatchStageEnum.QuarterFinal => 4,
                _ => throw new InvalidOperationException("Unsupported match stage for fixed group cross rule.")
            };

            if (firstPlaceds.Count != expectedGroupCount || secondPlaceds.Count != expectedGroupCount)
            {
                throw new InvalidOperationException($"Fixed group cross rule requires exactly {expectedGroupCount} groups with 2 players each.");
            }

            // Mapeamentos fixos para cada quantidade de grupos
            var fixedMatches = expectedGroupCount switch
            {
                        4 => new List<(int pos1, int grupo1, int pos2, int grupo2)>
                {
                    (1, 1, 2, 3),
                    (2, 2, 1, 4),
                    (1, 3, 2, 1),
                    (2, 4, 1, 2),
                },
                        8 => new List<(int pos1, int grupo1, int pos2, int grupo2)>
                {
                    (1, 1, 2, 7),
                    (2, 2, 1, 8),
                    (1, 3, 2, 5),
                    (2, 4, 1, 6),
                    (1, 5, 2, 3),
                    (2, 6, 1, 4),
                    (1, 7, 2, 1),
                    (2, 8, 1, 2),
                },
                        16 => new List<(int pos1, int grupo1, int pos2, int grupo2)>
                {
                    (1, 1, 2, 9),
                    (2, 2, 1, 10),
                    (1, 3, 2, 11),
                    (2, 4, 1, 12),
                    (1, 5, 2, 13),
                    (2, 6, 1, 14),
                    (1, 7, 2, 15),
                    (2, 8, 1, 16),
                    (1, 9, 2, 1),
                    (2, 10, 1, 2),
                    (1, 11, 2, 3),
                    (2, 12, 1, 4),
                    (1, 13, 2, 5),
                    (2, 14, 1, 6),
                    (1, 15, 2, 7),
                    (2, 16, 1, 8),
                },
                _ => throw new InvalidOperationException($"No fixed matches defined for {expectedGroupCount} groups.")
            };

            foreach (var (pos1, grupo1, pos2, grupo2) in fixedMatches)
            {
                numberGame++;

                var jogador1 = pos1 == 1
                    ? firstPlaceds.FirstOrDefault(g => g.GroupNumber == grupo1)
                    : secondPlaceds.FirstOrDefault(g => g.GroupNumber == grupo1);

                var jogador2 = pos2 == 1
                    ? firstPlaceds.FirstOrDefault(g => g.GroupNumber == grupo2)
                    : secondPlaceds.FirstOrDefault(g => g.GroupNumber == grupo2);

                if (jogador1 == null || jogador2 == null)
                {
                    throw new InvalidOperationException($"Missing player for group match: Grupo {grupo1} x Grupo {grupo2}");
                }

                var match = CreateGameMatch(jogador1, jogador2, matchStage, numberGame);
                matchesToCreate.Add(match);
            }

            return matchesToCreate;
        }



    }
}

