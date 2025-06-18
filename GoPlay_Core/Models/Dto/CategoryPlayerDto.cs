using GoPlay_Core.Enum;

namespace GoPlay_Core.Models.Dto
{
    public class CategoryPlayerDto
    {
        public int Id { get; set; }
        public int TournamentId { get; set; }
        public string TournamentName { get; set; }
        public string? TournamentPictureUrl { get; set; }
        public decimal RegistrationFee { get; set; } = 0;
        public DateTime PaymentDeadline { get; set; } = DateTime.UtcNow;
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string? FirstUserId { get; set; } = string.Empty;
        public string? SecondUserId { get; set; } = null;
        public string? FirstUserName { get; set;} = string.Empty;
        public string? SecondUserName { get; set; } = null;
        public bool FirstUserPaymentConfirmed { get; set; }
        public bool SecondUserPaymentConfirmed { get; set; }
        public string? FirstUserPictureUrl { get; set; }
        public string? SecondUserPictureUrl { get; set; }
        public int RegisterStatus { get; set; }
        public bool AttendanceConfirmed { get; set; } = false;
        public DateTime AttendanceTime { get; set; } = DateTime.UtcNow;
        public string? AttendanceConfirmedUserId { get; set; } = null;

    }
}
