using GoPlay_Core.Enum;

namespace GoPlay_Core.Models.Dto
{
    public class CategoryPlayerDto
    {
        public int Id { get; set; }
        public int TournamentId { get; set; }
        public string TournamentName { get; set; }
        public decimal RegistrationFee { get; set; } = 0;
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string? FirstUserId { get; set; } = string.Empty;
        public string? SecondUserId { get; set; } = null;
        public string? FirstUserName { get; set;} = string.Empty;
        public string? SecondUserName { get; set; } = null;
        public bool FirstUserPaymentConfirmed { get; set; }
        public bool SecondUserPaymentConfirmed { get; set; }
        public int RegisterStatus { get; set; }
    }
}
