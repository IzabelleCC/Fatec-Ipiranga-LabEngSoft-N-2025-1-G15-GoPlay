using GoPlay_App.Api.Controllers.TournamentManager.Models;
using GoPlay_Core.Entities;

public class TournamentCreateRequest
{
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime GamesStartDate { get; set; }
    public DateTime GamesEndDate { get; set; }
    public DateTime RegistrationDeadline { get; set; }
    public DateTime PaymentDeadline { get; set; }
    public string Location { get; set; }
    public decimal RegistrationFee { get; set; }
    public int CourtQuantity { get; set; }
    public string AdmUserId { get; set; } = string.Empty;
    public double Latitude { get; set; } = 0.0;
    public double Longitude { get; set; } = 0.0;
    public IEnumerable<CategoryCreateRequest> Categories { get; set; }

    public TournamentCreateRequest(string name,
                                 string description,
                                 DateTime gamesStartDate,
                                 DateTime gamesEndDate,
                                 DateTime registrationDeadline,
                                 DateTime paymentDeadline,
                                 string location,
                                 decimal registrationFee,
                                 int courtQuantity,
                                 string admUserId,
                                 double latitude,
                                 double longitude,
                                 IEnumerable<CategoryCreateRequest> categories)
    {
        Name = name;
        Description = description;
        GamesStartDate = gamesStartDate;
        GamesEndDate = gamesEndDate;
        RegistrationDeadline = registrationDeadline;
        PaymentDeadline = paymentDeadline;
        Location = location;
        RegistrationFee = registrationFee;
        CourtQuantity = courtQuantity;
        AdmUserId = admUserId;
        Latitude = latitude;
        Longitude = longitude;
        Categories = categories;
    }

    public TournamentEntity ToTournamentEntity()
    {
        var tournament = new TournamentEntity
        {
            Name = Name,
            Description = Description,
            GamesStartDate = GamesStartDate.ToUniversalTime(),
            GamesEndDate = GamesEndDate.ToUniversalTime(),
            RegistrationDeadline = RegistrationDeadline.ToUniversalTime(),
            PaymentDeadline = PaymentDeadline.ToUniversalTime(),
            Location = Location,
            RegistrationFee = RegistrationFee,
            CourtQuantity = CourtQuantity,
            AdmUserId = AdmUserId,
            Latitude = Latitude,
            Longitude = Longitude,
            Categories = new List<CategoryEntity>()
        };

        if (Categories != null)
        {
            foreach (var category in Categories)
            {
                tournament.Categories.Add(new CategoryEntity
                {
                    CategoryType = category.CategoryType,
                    PlayerLimit = category.PlayerLimit,
                    IsDoubles = category.IsDoubles,
                });
            }
        }

        return tournament;
    }
}
