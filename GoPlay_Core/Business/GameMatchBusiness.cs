using System.Threading;
using GoPlay_Core.Business.Interfaces;
using GoPlay_Core.Entities;
using GoPlay_Core.Enum;
using GoPlay_Core.Models.Dto;
using GoPlay_Core.Repository.Interfaces;
using GoPlay_Core.Services;
using Microsoft.Extensions.Logging;

namespace GoPlay_Core.Business
{
    public class GameMatchBusiness : IGameMatchBusiness
    {
        private readonly ILogger<MatchGroupBusiness> _logger;
        private readonly IMatchGroupRepository _matchRepository;
        private readonly IGameMatchRepository _gameMatchRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICategoryPlayerRepository _categoryPlayerRepository;
        private readonly FirebaseNotificationService _firebaseService;

        public GameMatchBusiness(
            ILogger<MatchGroupBusiness> logger,
            IMatchGroupRepository matchRepository,
            IGameMatchRepository gameMatchRepository,
            IUserRepository userRepository,
            ICategoryPlayerRepository categoryPlayerRepository,
            FirebaseNotificationService firebaseService)
        {
            _logger = logger;
            _matchRepository = matchRepository;
            _gameMatchRepository = gameMatchRepository;
            _userRepository = userRepository;
            _categoryPlayerRepository = categoryPlayerRepository;
            _firebaseService = firebaseService;
        }

        public async Task<List<GameMatchEntity>> GenerateEliminationMatches(int categoryId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Generating elimination matches for category ID {CategoryId}...", categoryId);

            // Buscar os classificados
            var matches = await _matchRepository.GetByCategoryAsync(categoryId);
            var noZeroPositionMatches = matches
                .Where(m => m.Position == 0)
                .ToList();

            if (noZeroPositionMatches.Count == 0)
            {
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
                var matchStage = GetMatchStage(winners.Count);

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

                var matchesToCreate = new List<GameMatchEntity>();

                var groupedBye = new List<MatchGroupEntity>();
                if (winners.Count < matchStage.qtdCompetitor)
                {
                    groupedBye = firstPlaceds
                        .OrderByDescending(g => g.Wins)
                        .ThenByDescending(g => g.SetsBalance)
                        .ThenByDescending(g => g.GamesBalance)
                        .Take(matchStage.qtdCompetitor - winners.Count)
                        .ToList();
                }

                if (matchStage.qtdCompetitor == winners.Count)
                {
                    matchesToCreate = await GenerateFixedGroupCrossMatches(firstPlaceds, secondPlaceds, matchStage.matchStage, matchStage.qtdCompetitor, numberGame);
                }
                else
                {
                    matchesToCreate = await GenerateGroupCrossMatchesWithBye(firstPlaceds, secondPlaceds, groupedBye, matchStage.matchStage, matchStage.qtdCompetitor / 2, numberGame);
                }

                // Salvar confrontos
                foreach (var game in matchesToCreate)
                {
                    await _gameMatchRepository.AddAsync(game);
                }

                _logger.LogInformation("Elimination matches created for category ID {CategoryId}: {Count} match(es) created.", categoryId, matchesToCreate.Count);

                return matchesToCreate;
            }

            return new List<GameMatchEntity>();
        }

        private (MatchStageEnum matchStage, int qtdCompetitor) GetMatchStage(int winnersCount)
        {
            switch (winnersCount)
            {
                case 2:
                    return (MatchStageEnum.Final, 2);
                case int v when (v > 2 && v <= 4):
                    return (MatchStageEnum.SemiFinal, 4);
                case int v when (v > 4 && v <= 8):
                    return (MatchStageEnum.QuarterFinal, 8);
                case int v when (v > 8 && v <= 16):
                    return (MatchStageEnum.RoundOf16, 16);
                case int v when (v > 16 && v <= 32):
                    return (MatchStageEnum.RoundOf32, 32);
                default:
                    return (MatchStageEnum.Undefined, 0);
            }
        }

        private async Task<GameMatchEntity> CreateGameMatch(MatchGroupEntity competitor1, MatchGroupEntity? competitor2, MatchStageEnum matchStage, int numberGame)
        {
            return new GameMatchEntity
            {
                MatchStage = matchStage,
                NumberGame = numberGame,
                Competitor1Id = competitor1?.RegistrationCategoryId,
                Competitor2Id = competitor2?.RegistrationCategoryId ?? null,
                CategoryId = competitor1?.CategoryId ?? competitor2.CategoryId,
                Result = competitor1 == null && competitor2 != null
                                ? competitor2.RegistrationCategoryId
                                : competitor2 == null && competitor1 != null
                                    ? competitor1.RegistrationCategoryId
                                    : null,
            };
        }

        private async Task<List<GameMatchEntity>> GenerateFixedGroupCrossMatches(
            List<MatchGroupEntity> firstPlaceds,
            List<MatchGroupEntity> secondPlaceds,
            MatchStageEnum matchStage,
            int qtdCompetitor,
            int numberGame)
        {
            var matchesToCreate = new List<GameMatchEntity>();

            var fixedMatches = await GetFixedMatches(qtdCompetitor);

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

                var match = await CreateGameMatch(jogador1, jogador2, matchStage, numberGame);
                matchesToCreate.Add(match);
            }

            return matchesToCreate;
        }

        private async Task<List<(int pos1, int grupo1, int pos2, int grupo2)>> GetFixedMatches(int qtdCompetitor)
        {
            return qtdCompetitor switch
            {
                4 => new List<(int pos1, int grupo1, int pos2, int grupo2)>
                {
                    (1, 1, 2, 2),
                    (2, 1, 1, 2),
                },

                8 => new List<(int pos1, int grupo1, int pos2, int grupo2)>
                {
                    (1, 1, 2, 3),
                    (2, 2, 1, 4),
                    (1, 3, 2, 1),
                    (2, 4, 1, 2),
                },

                16 => new List<(int pos1, int grupo1, int pos2, int grupo2)>
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

                32 => new List<(int pos1, int grupo1, int pos2, int grupo2)>
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

                _ => throw new InvalidOperationException($"No fixed matches defined for {qtdCompetitor} groups.")
            };
        }

        private async Task<List<GameMatchEntity>> GenerateGroupCrossMatchesWithBye(
            List<MatchGroupEntity> firstPlaceds,
            List<MatchGroupEntity> secondPlaceds,
            List<MatchGroupEntity> groupedBye,
            MatchStageEnum matchStage,
            int qtdGrupos,
            int numberGame)
        {
            var matchesToCreate = new List<GameMatchEntity>();

            var orderedFirstPlaceds = firstPlaceds
                .OrderByDescending(g => g.Wins)
                .ThenByDescending(g => g.SetsBalance)
                .ThenByDescending(g => g.GamesBalance)
                .ToList();

            var orderedSecondPlaceds = secondPlaceds
                .OrderByDescending(g => g.Wins)
                .ThenByDescending(g => g.SetsBalance)
                .ThenByDescending(g => g.GamesBalance)
                .ToList();

            _logger.LogInformation("---- FIRST PLACEDS RANKING ----");
            for (int i = 0; i < orderedFirstPlaceds.Count; i++)
            {
                _logger.LogInformation($"{i + 1}ª melhor campanha: Grupo {orderedFirstPlaceds[i].GroupNumber}");
            }

            _logger.LogInformation("---- SECOND PLACEDS RANKING ----");
            for (int i = 0; i < orderedSecondPlaceds.Count; i++)
            {
                _logger.LogInformation($"{i + 1}º do grupo da {i + 1}ª melhor campanha: Grupo {orderedSecondPlaceds[i].GroupNumber}");
            }

            // CORREÇÃO → mappedSecondPlaceds conforme a planilha
            var mappedSecondPlaceds = orderedFirstPlaceds
                .Select(first => orderedSecondPlaceds.FirstOrDefault(s => s.GroupNumber == first.GroupNumber))
                .ToList();

            var templateMatches = await GetTemplateMatchesForGroupCount(orderedFirstPlaceds, mappedSecondPlaceds);

            foreach (var match in templateMatches)
            {
                numberGame++;
                matchesToCreate.Add(await CreateGameMatch(match.Competitor1, match.Competitor2, matchStage, numberGame));

                var comp1Name = match.Competitor1 != null ? $"Grupo {match.Competitor1.GroupNumber}" : "BYE";
                var comp2Name = match.Competitor2 != null ? $"Grupo {match.Competitor2.GroupNumber}" : "BYE";
                _logger.LogInformation($"Jogo {numberGame}: {comp1Name} X {comp2Name}");
            }

            return matchesToCreate;
        }

        private async Task<List<(MatchGroupEntity Competitor1, MatchGroupEntity? Competitor2)>> GetTemplateMatchesForGroupCount(
            List<MatchGroupEntity> orderedFirstPlaceds,
            List<MatchGroupEntity> mappedSecondPlaceds)
        {
            var matches = new List<(MatchGroupEntity? Competitor1, MatchGroupEntity? Competitor2)>();

            int groupCount = orderedFirstPlaceds.Count;

            switch (groupCount)
            {
                case 3:
                    matches.Add((orderedFirstPlaceds[1], null));
                    matches.Add((mappedSecondPlaceds[0], mappedSecondPlaceds[2]));
                    matches.Add((orderedFirstPlaceds[2], mappedSecondPlaceds[1]));
                    matches.Add((null, orderedFirstPlaceds[0]));
                    break;

                case 5:
                    matches.Add((orderedFirstPlaceds[0], null));
                    matches.Add((mappedSecondPlaceds[1], mappedSecondPlaceds[2]));
                    matches.Add((orderedFirstPlaceds[3], null));
                    matches.Add((null, orderedFirstPlaceds[4]));
                    matches.Add((mappedSecondPlaceds[0], null));
                    matches.Add((null, orderedFirstPlaceds[2]));
                    matches.Add((mappedSecondPlaceds[3], mappedSecondPlaceds[4]));
                    matches.Add((null, orderedFirstPlaceds[1]));
                    break;

                case 6:
                    throw new NotImplementedException("Template for 6 groups not implemented yet.");
                case 7:
                    throw new NotImplementedException("Template for 7 groups not implemented yet.");

                default:
                    throw new InvalidOperationException($"No template defined for {groupCount} groups.");
            }

            return matches;
        }

        public async Task<List<GameMatchEntity>> InsertEliminationResultsAndReturnWinners(GameMatchEntity results, CancellationToken cancellationToken)
        {
            var gameMatchExists = await _gameMatchRepository.GetMatchesByCategoryIdAsync(results.CategoryId ?? 0);

            if (gameMatchExists != null)
            {
                var matchToUpdate = gameMatchExists
                    .FirstOrDefault(m => m.Competitor1Id == results.Competitor1Id && m.Competitor2Id == results.Competitor2Id);
                if (matchToUpdate != null)
                {
                    matchToUpdate.QtdGames1 = results.QtdGames1;
                    matchToUpdate.QtdGames2 = results.QtdGames2;

                    matchToUpdate.Result = results.QtdGames1 > results.QtdGames2
                                                            ? results.Competitor1Id
                                                            : results.Competitor2Id;

                    await _gameMatchRepository.UpdateAsync(matchToUpdate);

                    var result = await _gameMatchRepository.GetMatchesByCategoryIdAsync(results.CategoryId ?? 0);
                    var noResultZeroOrNull = result
                        .Where(m => m.Result == 0 || m.Result == null)
                        .ToList();

                    var duble1 = await _categoryPlayerRepository.GetByIdAsync(results.Competitor1Id ?? 0);
                    var duble2 = await _categoryPlayerRepository.GetByIdAsync(results.Competitor2Id ?? 0);
                    var user1Duble1 = await _userRepository.GetById(duble1.FirstUserId);
                    var user1Duble2 = duble1.SecondUserId != null
                        ? await _userRepository.GetById(duble1.SecondUserId)
                        : null;
                    var user2Duble1 = await _userRepository.GetById(duble2.FirstUserId);
                    var user2Duble2 = duble2.SecondUserId != null
                        ? await _userRepository.GetById(duble2.SecondUserId)
                        : null;

                    if (!string.IsNullOrEmpty(user1Duble1.FCMToken))
                    {
                        try
                        {
                            _logger.LogInformation("Enviando notificação para {UserName} ({UserId}) sobre o resultado do jogo", user1Duble1.Name, user1Duble1.Id);
                            await _firebaseService.SendNotificationAsync(
                                    user1Duble1.FCMToken,
                                    "Resultado do Jogo",
                                    $"O jogo entre {user1Duble1.Name} e {user2Duble1.Name} terminou com o resultado {results.QtdGames1} X {results.QtdGames2}.");

                            _logger.LogInformation("Notificação enviada com sucesso para {UserName} ({UserId})", user1Duble1.Name, user1Duble1.Id);
                        }
                        catch
                        {
                            _logger.LogError("Erro ao enviar notificação para {UserName} ({UserId})", user1Duble1.Name, user1Duble1.Id);
                        }
                    }
                    if (!string.IsNullOrEmpty(user1Duble2?.FCMToken))
                    {
                        try
                        {
                            _logger.LogInformation("Enviando notificação para {UserName} ({UserId}) sobre o resultado do jogo", user1Duble2.Name, user1Duble2.Id);
                            await _firebaseService.SendNotificationAsync(
                                    user1Duble2.FCMToken,
                                    "Resultado do Jogo",
                                    $"O jogo entre {user1Duble1.Name} e {user2Duble1.Name} terminou com o resultado {results.QtdGames1} X {results.QtdGames2}.");
                            _logger.LogInformation("Notificação enviada com sucesso para {UserName} ({UserId})", user1Duble2.Name, user1Duble2.Id);
                        }
                        catch
                        {
                            _logger.LogError("Erro ao enviar notificação para {UserName} ({UserId})", user1Duble2.Name, user1Duble2.Id);
                        }
                    }
                    if (!string.IsNullOrEmpty(user2Duble1.FCMToken))
                    {
                        try
                        {
                            _logger.LogInformation("Enviando notificação para {UserName} ({UserId}) sobre o resultado do jogo", user2Duble1.Name, user2Duble1.Id);
                            await _firebaseService.SendNotificationAsync(
                                    user2Duble1.FCMToken,
                                    "Resultado do Jogo",
                                    $"O jogo entre {user1Duble1.Name} e {user2Duble1.Name} terminou com o resultado {results.QtdGames1} X {results.QtdGames2}.");
                            _logger.LogInformation("Notificação enviada com sucesso para {UserName} ({UserId})", user2Duble1.Name, user2Duble1.Id);
                        }
                        catch
                        {
                            _logger.LogError("Erro ao enviar notificação para {UserName} ({UserId})", user2Duble1.Name, user2Duble1.Id);
                        }
                    }
                    if (!string.IsNullOrEmpty(user2Duble2?.FCMToken))
                    {
                        try
                        {

                            _logger.LogInformation("Enviando notificação para {UserName} ({UserId}) sobre o resultado do jogo", user2Duble2.Name, user2Duble2.Id);
                            await _firebaseService.SendNotificationAsync(
                                    user2Duble2.FCMToken,
                                    "Resultado do Jogo",
                                    $"O jogo entre {user1Duble1.Name} e {user2Duble1.Name} terminou com o resultado {results.QtdGames1} X {results.QtdGames2}.");
                            _logger.LogInformation("Notificação enviada com sucesso para {UserName} ({UserId})", user2Duble2.Name, user2Duble2.Id);
                        }
                        catch
                        {
                            _logger.LogError("Erro ao enviar notificação para {UserName} ({UserId})", user2Duble2.Name, user2Duble2.Id);
                        }
                    }

                    if (noResultZeroOrNull.Count == 0)
                    {
                        await GenerateNewPhaseWithWinners(results.CategoryId ?? 0);
                    }
                    // Retornar os vencedores
                    return new List<GameMatchEntity> { matchToUpdate };
                }
                else
                {
                    _logger.LogWarning("Match not found for Competitor1Id: {Competitor1Id}, Competitor2Id: {Competitor2Id}",
                        results.Competitor1Id, results.Competitor2Id);
                    throw new InvalidOperationException("Match not found.");
                }
            }
            return new List<GameMatchEntity>();
        }

        public async Task<List<EliminationGameDto>> GetEliminationGamesByCategory(int categoryId, int matchStage, CancellationToken cancellationToken)
        {
            var matches = await _gameMatchRepository.GetEliminationGamesByCategory(categoryId, matchStage);

            if (matches == null || !matches.Any())
            {
                _logger.LogWarning("No elimination matches found for category ID {CategoryId}.", categoryId);
                return new List<EliminationGameDto>();
            }

            var result = new List<EliminationGameDto>();

            foreach (var match in matches)
            {
                var dto = new EliminationGameDto
                {
                    GameEliminationId = match.GameEliminationId,
                    Competitor1Id = match.Competitor1Id,
                    Competitor2Id = match.Competitor2Id,
                    MatchStage = match.MatchStage,
                    MatchTime = match.MatchTime,
                    CourtNumber = match.CourtNumber,
                    QtdGames1 = match.QtdGames1,
                    QtdGames2 = match.QtdGames2,
                    Result = match.Result,
                    NumberGame = match.NumberGame,
                    CategoryId = match.CategoryId,
                    Competitor1 = new GroupPlayerDto(),
                    Competitor2 = new GroupPlayerDto()
                };

                if (match.Competitor1 != null)
                {
                    dto.Competitor1.Id = match.Competitor1.Id;
                    dto.Competitor1.FirstUserId = match.Competitor1.FirstUserId;
                    dto.Competitor1.SecondUserId = match.Competitor1.SecondUserId;

                    var user1 = await _userRepository.GetById(match.Competitor1.FirstUserId);
                    var user2 = match.Competitor1.SecondUserId != null
                        ? await _userRepository.GetById(match.Competitor1.SecondUserId)
                        : null;

                    dto.Competitor1.FirstUserName = user1?.Name ?? string.Empty;
                    dto.Competitor1.FirstUserPictureUrl = user1?.ProfilePictureUrl ?? string.Empty;
                    dto.Competitor1.SecondUserName = user2?.Name ?? string.Empty;
                    dto.Competitor1.SecondUserPictureUrl = user2?.ProfilePictureUrl ?? string.Empty;

                }

                if (match.Competitor2 != null)
                {
                    dto.Competitor2.Id = match.Competitor2.Id;
                    dto.Competitor2.FirstUserId = match.Competitor2.FirstUserId;
                    dto.Competitor2.SecondUserId = match.Competitor2.SecondUserId;

                    var user1 = await _userRepository.GetById(match.Competitor2.FirstUserId);
                    var user2 = match.Competitor2.SecondUserId != null
                        ? await _userRepository.GetById(match.Competitor2.SecondUserId)
                        : null;

                    dto.Competitor2.FirstUserName = user1?.Name ?? string.Empty;
                    dto.Competitor2.FirstUserPictureUrl = user1?.ProfilePictureUrl ?? string.Empty;
                    dto.Competitor2.SecondUserName = user2?.Name ?? string.Empty;
                    dto.Competitor2.SecondUserPictureUrl = user2?.ProfilePictureUrl ?? string.Empty;
                }

                result.Add(dto);
            }

            return result;
        }

        private async Task GenerateNewPhaseWithWinners(int categoryId)
        {

            var winners = await _gameMatchRepository.GetMatchesByCategoryIdAsync(categoryId);
            var maxMatchStage = winners.Max(m => m.MatchStage);
            var winnersOrderByNuberGame = winners
                .Where(m => m.MatchStage == maxMatchStage)
                .OrderBy(m => m.NumberGame)
                .ToList();

            if (maxMatchStage == MatchStageEnum.Final) return;

            int? lastNumberGame = winnersOrderByNuberGame.Count > 0
                                                ? winnersOrderByNuberGame[^1].NumberGame
                                                : null;

            var matchStage = GetMatchStage(winnersOrderByNuberGame.Count);

            for (int i = 0; i < winnersOrderByNuberGame.Count; i += 2)
            {
                lastNumberGame++;
                await _gameMatchRepository.AddAsync(new GameMatchEntity
                {
                    Competitor1Id = winnersOrderByNuberGame[i].Result,
                    Competitor2Id = winnersOrderByNuberGame[i + 1].Result,
                    MatchStage = matchStage.matchStage,
                    CategoryId = winnersOrderByNuberGame[i].CategoryId,
                    NumberGame = lastNumberGame,
                });
            }
        }

        public async Task InsertCourtNumberElimination(int categoryId, int numberGame, int courtNumber, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Inserting court number for category ID {CategoryId}...", categoryId);

            var matches = await _gameMatchRepository.GetMatchesByCategoryIdAsync(categoryId);

            if (matches == null || !matches.Any())
            {
                _logger.LogWarning("No elimination matches found for category ID {CategoryId}.", categoryId);
                throw new KeyNotFoundException("No elimination matches found for the specified category and game number.");
            }

            var matchByNumberGame = matches
                .Where(m => m.NumberGame == numberGame)
                .ToList();

            foreach (var match in matchByNumberGame)
            {
                match.CourtNumber = courtNumber;
                await _gameMatchRepository.UpdateAsync(match);

                var users = await _categoryPlayerRepository.GetByIdAsync(match.Competitor1Id ?? 0);

                var firstUser = await _userRepository.GetById(users.FirstUserId);

                if (!string.IsNullOrEmpty(firstUser.FCMToken))
                {
                    try
                    {
                        _logger.LogInformation("Enviando notificação para {UserName} ({UserId}) sobre a quadra nº {CourtNumber}", firstUser.Name, firstUser.Id, courtNumber);
                        await _firebaseService.SendNotificationAsync(
                            firstUser.FCMToken,
                            "Chamada de Quadra",
                            $"Seu próximo jogo será na quadra nº {courtNumber}. \nLembre-se !! Você e seu parceiro terão 10min de aquecimento. \nApós o aquecimento o jogo deve ser iniciado imediatamente!"
                        );
                        _logger.LogInformation("Notificação enviada com sucesso para {UserName} ({UserId})", firstUser.Name, firstUser.Id);
                    }
                    catch
                    {
                        _logger.LogError("Erro ao enviar notificação para {UserName} ({UserId})", firstUser.Name, firstUser.Id);
                    }

                    if (users.SecondUser != null)
                    {
                        var secondUser = await _userRepository.GetById(users.SecondUserId);
                        if (!string.IsNullOrEmpty(secondUser.FCMToken))
                        {
                            try
                            {
                                _logger.LogInformation("Enviando notificação para {UserName} ({UserId}) sobre a quadra nº {CourtNumber}", secondUser.Name, secondUser.Id, courtNumber);
                                await _firebaseService.SendNotificationAsync(
                                    secondUser.FCMToken,
                                    "Court Number Assigned",
                                    $"Seu próximo jogo será na quadra nº {courtNumber}. \nLembre-se !! Você e seu parceiro terão 10min de aquecimento. \nApós o aquecimento o jogo deve ser iniciado imediatamente!"
                                );
                                _logger.LogInformation("Notificação enviada com sucesso para {UserName} ({UserId})", secondUser.Name, secondUser.Id);
                            }
                            catch
                            {
                                _logger.LogError("Erro ao enviar notificação para {UserName} ({UserId})", secondUser.Name, secondUser.Id);
                            }
                        }
                    }
                }
            }
            _logger.LogInformation("Court numbers inserted successfully for category ID {CategoryId} and game number {NumberGamer}.", categoryId, numberGame);
        }
    }
}
