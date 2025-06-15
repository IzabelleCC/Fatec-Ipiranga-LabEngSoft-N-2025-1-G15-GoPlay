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
        public int? Game1 { get; set; }
        public int? Game2 { get; set; }
        public int? Game3 { get; set; }
        public int? Game4 { get; set; }
        public int? Game5 { get; set; }
        public int? Game6 { get; set; }
        public int? Game7 { get; set; }
        public int? Game8 { get; set; }
        public int? Game9 { get; set; }
        public int? Game10 { get; set; }

        public int? SumOfGamesWon { get; set; } = 0;
        public int? SumOfGamesLost { get; set; } = 0;
        public int? GamesBalance { get; set; } = 0;
        public int? Tiebreaks { get; set; } = 0;
        public MatchStageEnum MatchStage { get; set; } = MatchStageEnum.Group;
        public CategoryPlayerEntity RegistrationCategory { get; set; } = null!;
        public CategoryEntity Category { get; set; } = null!;
    }
}
