namespace GoPlay_Core.Models.Dto
{
    public class TournamentDetailsDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Status { get; set; }
        public DateTime GamesStartDate { get; set; }
        public DateTime GamesEndDate { get; set; }
        public DateTime RegistrationDeadline { get; set; }

        public List<CategorySummaryDto> Categories { get; set; } = new();
    }
}
