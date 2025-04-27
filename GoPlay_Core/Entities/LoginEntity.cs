using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace GoPlay_Core.Entities
{
    /// <summary>
    /// LoginEntity is used to represent the login information of a user.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class LoginEntity
    {
        [Required]
        public required string UserName { get; set; }
        [Required]
        public required string Password { get; set; }

        /// <summary>
        /// Default constructor for LoginEntity.
        /// </summary>
        public LoginEntity()
        {
        }
    }
}
