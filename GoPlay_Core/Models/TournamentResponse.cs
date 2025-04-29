using GoPlay_Core.Entities;

namespace GoPlay_App.Api.Controllers.TournamentManager.Models
{
    /// <summary>
    /// Classe de resposta do torneio
    /// </summary>
    public class TournamentResponse
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime GamesStartDate { get; set; }
        public DateTime GamesEndDate { get; set; }
        public DateTime RegistrationDeadline { get; set; }
        public DateTime PaymentDeadline { get; set; }
        public string Location { get; set; }
        public decimal RegistrationFee { get; set; }
        public int CourtQuantity { get; set; }
        public List<CategoryEntity> Categories { get; set; } = new List<CategoryEntity>();

        /// <summary>
        /// Construtor da classe TournamentResponse
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="createdAt"></param>
        /// <param name="gamesStartDate"></param>
        /// <param name="gamesEndDate"></param>
        /// <param name="registrationDeadline"></param>
        /// <param name="paymentDeadline"></param>
        /// <param name="location"></param>
        /// <param name="registrationFee"></param>
        /// <param name="courtQuantity"></param>
        /// <param name="categories"></param>
        public TournamentResponse(string name,
            string description,
            DateTime createdAt,
            DateTime gamesStartDate,
            DateTime gamesEndDate,
            DateTime registrationDeadline,
            DateTime paymentDeadline,
            string location,
            decimal registrationFee,
            int courtQuantity,
            List<CategoryEntity> categories)
        {
            Name = name;
            Description = description;
            CreatedAt = createdAt;
            GamesStartDate = gamesStartDate;
            GamesEndDate = gamesEndDate;
            RegistrationDeadline = registrationDeadline;
            PaymentDeadline = paymentDeadline;
            Location = location;
            RegistrationFee = registrationFee;
            CourtQuantity = courtQuantity;
            Categories = categories ?? new List<CategoryEntity>();
        }

        /// <summary>
        /// Converte um TournamentEntity para TournamentResponse
        /// </summary>
        /// <param name="tournamentEntity"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static TournamentResponse ConvertToTournamentResponse(TournamentEntity tournamentEntity)
        {
            if (tournamentEntity == null)
            {
                throw new ArgumentNullException(nameof(tournamentEntity));
            }
            return new TournamentResponse(
                tournamentEntity.Name,
                tournamentEntity.Description,
                tournamentEntity.CreatedAt,
                tournamentEntity.GamesStartDate,
                tournamentEntity.GamesEndDate,
                tournamentEntity.RegistrationDeadline,
                tournamentEntity.PaymentDeadline,
                tournamentEntity.Location,
                tournamentEntity.RegistrationFee,
                tournamentEntity.CourtQuantity,
                tournamentEntity.Categories);
        }
    }
}
