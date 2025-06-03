namespace GoPlay_Core.Entities
{
    public class MatchEntity
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }

        /// <summary>
        /// Identificadores de registro dos jogadores na categoria.
        /// </summary>
        public int Player1RegistrationId { get; set; } 
        public int Player2RegistrationId { get; set; }

        public DateTime? ScheduledAt { get; set; }
        public string? Result { get; set; }

        public CategoryPlayerEntity Player1 { get; set; } = null!;
        public CategoryPlayerEntity Player2 { get; set; } = null!;
        public CategoryEntity Category { get; set; } = null!;
    }

}
