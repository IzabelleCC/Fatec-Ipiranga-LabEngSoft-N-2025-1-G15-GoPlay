using GoPlay_Core.Enum;

namespace GoPlay_Core.Models.Dto
{
    public class EliminationGameDto
    {
        public int GameEliminationId { get; set; }
        public int? Competitor1Id { get; set; }
        public int? Competitor2Id { get; set; }
        public MatchStageEnum MatchStage { get; set; }
        public DateTime? MatchTime { get; set; }
        public int? CourtNumber { get; set; }
        public int? QtdGames1 { get; set; }
        public int? QtdGames2 { get; set; }
        public int? Result { get; set; }
        public int? NumberGame { get; set; } = 0;
        public int? CategoryId { get; set; }
        public GroupPlayerDto Competitor1 { get; set; } = null!;
        public GroupPlayerDto Competitor2 { get; set; } = null!;
    }
}
