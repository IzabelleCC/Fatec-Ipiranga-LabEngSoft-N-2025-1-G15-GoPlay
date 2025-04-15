using GoPlay_Core.Enum;
using Microsoft.AspNetCore.Identity;

namespace GoPlay_Core.Entities
{
    public class UserEntity : IdentityUser
    {
        public string Name { get; set; } = string.Empty;
        public UserTypeEnum UserType { get; set; }
        public string? InstagramPage { get; set; }
        public string CpfCnpj { get; set; } = string.Empty;
        public string? Gender { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? TShirtSize { get; set; }
        public bool IsActive { get; set; } = true;

        public UserEntity()
        {

        }
    }
}
