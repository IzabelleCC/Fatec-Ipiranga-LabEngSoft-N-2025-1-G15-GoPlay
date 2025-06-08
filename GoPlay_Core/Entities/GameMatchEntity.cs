using System.ComponentModel.DataAnnotations.Schema;
using GoPlay_Core.Enum;

namespace GoPlay_Core.Entities
{
    public class GameMatchEntity
    {
        public int Id { get; set; }
        public int Competitor1Id { get; set; }
        public int? Competitor2Id { get; set; }
        public MatchStageEnum MatchStage { get; set; }
        public DateTime? MatchTime { get; set; }
        public int? CourtNumber { get; set; }
        public int? QtdGames1 { get; set; }
        public int? QtdGames2 { get; set; }
        public int? Result { get; set; }
        public int? NumberGame { get; set; } = 0;
        public CategoryPlayerEntity Competitor1 { get; set; } = null!;
        public CategoryPlayerEntity Competitor2 { get; set; } = null!;
    }
}
