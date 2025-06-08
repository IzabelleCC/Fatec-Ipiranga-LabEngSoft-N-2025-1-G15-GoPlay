using GoPlay_Core.Entities;

public class CategoryEntity
{
    public int Id { get; set; }
    public string CategoryType { get; set; } = string.Empty;
    public int PlayerLimit { get; set; } = 0;
    public int TournamentId { get; set; } = 0;
    public bool IsDoubles { get; set; } = true;
    public ICollection<CategoryPlayerEntity> CategoryPlayers { get; set; } = new List<CategoryPlayerEntity>();
    public ICollection<MatchGroupEntity> MatchGroups { get; set; } = new List<MatchGroupEntity>();
    public ICollection<GameMatchEntity> GameMatches { get; set; } = new List<GameMatchEntity>();

}
