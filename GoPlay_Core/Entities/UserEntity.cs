using System.Diagnostics.CodeAnalysis;
using GoPlay_Core.Enum;
using Microsoft.AspNetCore.Identity;

namespace GoPlay_Core.Entities
{
    /// <summary>
    /// Represents a user in the system.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class UserEntity : IdentityUser
    {
        public string Name { get; set; } = string.Empty;
        public UserTypeEnum UserType { get; set; } = UserTypeEnum.Player;
        public string? InstagramPage { get; set; }
        public string CpfCnpj { get; set; } = string.Empty;
        public string? Gender { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? TShirtSize { get; set; }
        public bool IsActive { get; set; } = true;

        public List<CategoryEntity> Categories { get; set; } = new List<CategoryEntity>();

        public UserEntity()
        {

        }
    }
}
