namespace GoPlay_Core.Models.Dto
{
    public class TournamentMatchesResultDto
    {
        public string TournamentName { get; set; } = string.Empty;
        public List<CategoryGroupsDto> Groups { get; set; } = new();
    }

}
