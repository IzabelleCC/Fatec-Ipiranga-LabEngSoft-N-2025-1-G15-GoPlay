namespace GoPlay_App.Api.Controllers.TournamentManager.Models
{
    public class TournamentUpdateRequest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime GamesStartDate { get; set; }
        public DateTime GamesEndDate { get; set; }
        public DateTime RegistrationDeadline { get; set; }
        public DateTime PaymentDeadline { get; set; }
        public string Location { get; set; }
        public decimal RegistrationFee { get; set; }
        public int CourtQuantity { get; set; }
        public IEnumerable<CategoryCreateRequest> Categories { get; set; }

        public TournamentUpdateRequest(int id,
                                     string name,
                                     string description,
                                     DateTime gamesStartDate,
                                     DateTime gamesEndDate,
                                     DateTime registrationDeadline,
                                     DateTime paymentDeadline,
                                     string location,
                                     decimal registrationFee,
                                     int courtQuantity,
                                     IEnumerable<CategoryCreateRequest> categories)
        {
            Id = id;
            Name = name;
            Description = description;
            GamesStartDate = gamesStartDate;
            GamesEndDate = gamesEndDate;
            RegistrationDeadline = registrationDeadline;
            PaymentDeadline = paymentDeadline;
            Location = location;
            RegistrationFee = registrationFee;
            CourtQuantity = courtQuantity;
            Categories = categories;
        }

        public TournamentEntity ToTournamentEntity()
        {
            var tournament = new TournamentEntity
            {
                Id = Id,
                Name = Name,
                Description = Description,
                GamesStartDate = GamesStartDate.ToUniversalTime(),
                GamesEndDate = GamesEndDate.ToUniversalTime(),
                RegistrationDeadline = RegistrationDeadline.ToUniversalTime(),
                PaymentDeadline = PaymentDeadline.ToUniversalTime(),
                Location = Location,
                RegistrationFee = RegistrationFee,
                CourtQuantity = CourtQuantity,
                Categories = new List<CategoryEntity>()
            };

            if (Categories != null)
            {
                foreach (var category in Categories)
                {
                    tournament.Categories.Add(new CategoryEntity
                    {
                        CategoryType = category.CategoryType,
                        PlayerLimit = category.PlayerLimit
                    });
                }
            }

            return tournament;
        }
    }
}
