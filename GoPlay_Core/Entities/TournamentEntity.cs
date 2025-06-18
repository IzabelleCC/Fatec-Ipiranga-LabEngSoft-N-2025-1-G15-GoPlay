using GoPlay_Core.Entities;
using GoPlay_Core.Enum;

public class TournamentEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime GamesStartDate { get; set; } = DateTime.UtcNow;
    public DateTime GamesEndDate { get; set; } = DateTime.UtcNow;
    public DateTime RegistrationDeadline { get; set; } = DateTime.UtcNow;
    public DateTime PaymentDeadline { get; set; } = DateTime.UtcNow;
    public string Location { get; set; } = string.Empty;
    public decimal RegistrationFee { get; set; } = 0;
    public bool IsActive { get; set; } = true;
    public int CourtQuantity { get; set; } = 0;
    public string AdmUserId { get; set; } = string.Empty;
    public double Latitude { get; set; } = 0.0;
    public double Longitude { get; set; } = 0.0;
    public string? ProfilePictureUrl { get; set; }
    public TournamentStatusEnum Status { get; set; } = TournamentStatusEnum.Criado;
    public UserEntity AdmUser { get; set; } = null!;
    public List<CategoryEntity> Categories { get; set; } = new List<CategoryEntity>();
}
