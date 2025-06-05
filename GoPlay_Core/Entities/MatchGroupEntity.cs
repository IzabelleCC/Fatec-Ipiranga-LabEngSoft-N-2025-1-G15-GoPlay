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
        public int? Position { get; set; }
        public int? Wins { get; set; }
        public int? Losses { get; set; }
        public int? SetsBalance { get; set; }
        public int? Game1 { get; set; }
        public int? Game2 { get; set; }
        public int? Game3 { get; set; }
        public int? Game4 { get; set; }
        public int? Game5 { get; set; }
        public int? GamesBalance { get; set; }
        public int? Tiebreaks { get; set; }

        public CategoryPlayerEntity RegistrationCategory { get; set; } = null!;
        public CategoryEntity Category { get; set; } = null!;
    }
}
