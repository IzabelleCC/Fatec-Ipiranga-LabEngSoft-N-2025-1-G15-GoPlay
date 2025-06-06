using GoPlay_Core.Enum;

namespace GoPlay_Core.Entities
{
    public class MatchGroupEntity
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public int GroupNumber { get; set; }
        public int RegistrationCategoryId { get; set; }
        public DateTime? ScheduledAt { get; set; }
        public bool AttendanceConfirmed { get; set; } = false;
        public int? Position { get; set; } = 0;
        public int? Wins { get; set; } = 0;
        public int? Losses { get; set; } = 0;
        public int? SetsBalance { get; set; } = 0;
        public int? Game1 { get; set; } = 0;
        public int? Game2 { get; set; } = 0;
        public int? Game3 { get; set; } = 0;
        public int? Game4 { get; set; } = 0;
        public int? Game5 { get; set; } = 0;
        public int? SumOfGames { get; set; } = 0;
        public int? GamesBalance { get; set; } = 0;
        public int? Tiebreaks { get; set; } = 0;
        public MatchStageEnum MatchStage { get; set; } = MatchStageEnum.Group;
        public CategoryPlayerEntity RegistrationCategory { get; set; } = null!;
        public CategoryEntity Category { get; set; } = null!;
    }
}
