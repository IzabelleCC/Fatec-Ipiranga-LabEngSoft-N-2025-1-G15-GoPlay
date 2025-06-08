using GoPlay_Core.Business.Interfaces;
using GoPlay_Core.Entities;
using GoPlay_Core.Enum;
using GoPlay_Core.Repository.Interfaces;
using Microsoft.Extensions.Logging;

namespace GoPlay_Core.Business
{
    public class GameMatchBusiness : IGameMatchBusiness
    {
        private readonly ILogger<MatchGroupBusiness> _logger;
        private readonly IMatchGroupRepository _matchRepository;
        private readonly IGameMatchRepository _gameMatchRepository;

        public GameMatchBusiness(
            ILogger<MatchGroupBusiness> logger,
            IMatchGroupRepository matchRepository,
            IGameMatchRepository gameMatchRepository)
        {
            _logger = logger;
            _matchRepository = matchRepository;
            _gameMatchRepository = gameMatchRepository;
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

            List<GameMatchEntity> matchesToCreate;

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

            matchesToCreate = await GenerateFixedGroupCrossMatches(firstPlaceds, secondPlaceds, groupedBye, matchStage.matchStage, matchStage.qtdCompetitor / 2, numberGame);

            // Salvar confrontos
            foreach (var game in matchesToCreate)

            {
                await _gameMatchRepository.AddAsync(game);
            }

            _logger.LogInformation("Elimination matches created for category ID {CategoryId}: {Count} match(es) created.", categoryId, matchesToCreate.Count);

            return matchesToCreate;
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
                Competitor1Id = competitor1.RegistrationCategoryId,
                Competitor2Id = competitor2?.RegistrationCategoryId ?? null,
                CategoryId = competitor1.CategoryId,
            };
        }

        private async Task<List<GameMatchEntity>> GenerateFixedGroupCrossMatches(
            List<MatchGroupEntity> firstPlaceds,
            List<MatchGroupEntity> secondPlaceds,
            List<MatchGroupEntity> groupedBye,
            MatchStageEnum matchStage,
            int qtdCompetitor,
            int numberGame)
        {

            var matchesToCreate = new List<GameMatchEntity>();

            // Mapeamentos fixos para cada quantidade de grupos
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

                _ => throw new InvalidOperationException($"No fixed matches defined for {qtdCompetitor} groups.")
            };
        }

    }
}
