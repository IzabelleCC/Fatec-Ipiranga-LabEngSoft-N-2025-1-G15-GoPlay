namespace GoPlay_Core.Entities
{
    public class MatchGroupEntity
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public int GroupNumber { get; set; }
        public int RegistrationCategoryId { get; set; } 
        public DateTime? ScheduledAt { get; set; }
        public string? Result { get; set; }
        public bool AttendanceConfirmed { get; set; } = false;
        public CategoryPlayerEntity RegistrationCategory { get; set; } = null!;
        public CategoryEntity Category { get; set; } = null!;
    }
}
