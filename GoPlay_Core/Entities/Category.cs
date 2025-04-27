using System.Diagnostics.CodeAnalysis;

namespace GoPlay_Core.Entities
{
    [ExcludeFromCodeCoverage]
    public class Category
    {
        public int Id { get; set; }
        public string CategoryType { get; set; } = string.Empty;
        public int PlayerLimit { get; set; } = 0;
        public int TournamentId { get; set; } = 0;
        public bool IsActive { get; set; } = true;
        public IEnumerable<UserEntity> Players { get; set; } = new List<UserEntity>();

        public Category()
        {

        }
    }
}
