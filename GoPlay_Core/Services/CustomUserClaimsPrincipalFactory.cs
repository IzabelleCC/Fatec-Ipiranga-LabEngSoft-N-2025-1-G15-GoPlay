using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using GoPlay_Core.Entities;

namespace GoPlay_Core.Services
{
    public class CustomUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<UserEntity, IdentityRole>
    {
        public CustomUserClaimsPrincipalFactory(
            UserManager<UserEntity> userManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<IdentityOptions> optionsAccessor)
            : base(userManager, roleManager, optionsAccessor)
        {
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(UserEntity user)
        {
            var identity = await base.GenerateClaimsAsync(user);

            if (!string.IsNullOrEmpty(user.UserType.ToString()))
            {
                identity.AddClaim(new Claim(ClaimTypes.Role, user.UserType.ToString()));
            }

            return identity;
        }
    }
}
