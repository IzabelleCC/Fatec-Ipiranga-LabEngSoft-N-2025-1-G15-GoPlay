using GoPlay_Core.Entities;
using GoPlay_Core.Enum;

namespace GoPlay_Core.Models.Dto
{
    public class CategoryPlayerFullInfoDto
    {
        // CategoryPlayer fields
        public CategoryPlayerEntity CategoryPlayer { get; set; } = new CategoryPlayerEntity();
        public string? FirstUserName { get; set; } = string.Empty;
        public string? SecondUserName { get; set; } = string.Empty;
        // Category fields
        public CategoryEntity Category { get; set; } = new CategoryEntity();
        public int? RegisterCount { get; set; } = 0;

        // Tournament fields
        public TournamentEntity Tournament { get; set; } = new TournamentEntity();
    }
}
