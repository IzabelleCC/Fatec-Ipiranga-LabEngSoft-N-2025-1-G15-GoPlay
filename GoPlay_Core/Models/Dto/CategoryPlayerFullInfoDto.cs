using GoPlay_Core.Enum;

namespace GoPlay_Core.Models.Dto
{
    public class CategoryPlayerFullInfoDto
    {
        // CategoryPlayer fields
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string UserId { get; set; }
        public RegisterStatusEnum RegisterStatus { get; set; }

        // Category fields
        public string CategoryType { get; set; } = string.Empty;
        public bool IsDoubles { get; set; }

        // Tournament fields
        public int Tournament_Id { get; set; }
        public string TournamentName { get; set; } = string.Empty;
    }
}
