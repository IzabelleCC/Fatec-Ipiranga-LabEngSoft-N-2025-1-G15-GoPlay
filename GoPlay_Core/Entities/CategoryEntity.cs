using GoPlay_Core.Entities;

public class CategoryEntity
{
    public int Id { get; set; }
    public string CategoryType { get; set; } = string.Empty;
    public int PlayerLimit { get; set; } = 0;
    public int TournamentId { get; set; } = 0;
    public bool IsActive { get; set; } = true;

    public List<UserEntity> Players { get; set; } = new List<UserEntity>();

    public CategoryEntity()
    {
    }
}
