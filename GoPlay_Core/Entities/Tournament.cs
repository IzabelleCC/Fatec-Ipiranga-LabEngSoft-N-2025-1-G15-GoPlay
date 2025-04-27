namespace GoPlay_Core.Entities
{
    public class Tournament
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
        public IEnumerable<Category> Categories { get; set; } = new List<Category>();

        public Tournament()
        {
            
        }
    }
}
