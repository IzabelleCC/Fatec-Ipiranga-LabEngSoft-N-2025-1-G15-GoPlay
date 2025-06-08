using GoPlay_Core.Entities;

namespace GoPlay_App.Api.Controllers.TournamentManager.Models
{
    public class EliminationResultsRequest
    {
        public int CategoryId { get; set; }
        public int CompetitorId1 { get; set; }
        public int CompetitorId2 { get; set; }
        public int? Game1 { get; set; }
        public int? Game2 { get; set; }

        public EliminationResultsRequest(int categoryId, int competitorId1, int competitorId2,  int? game1, int? game2)
        {
            CategoryId = categoryId;
            CompetitorId1 = competitorId1;
            CompetitorId2 = competitorId2;
            Game1 = game1;
            Game2 = game2;
        }

        public GameMatchEntity ToGameMatchEntity()
        {
            return new GameMatchEntity
            {
                CategoryId = CategoryId,
                Competitor1Id = CompetitorId1,
                Competitor2Id = CompetitorId2,
                QtdGames1 = Game1,
                QtdGames2 = Game2,
            };
        }
    }
}