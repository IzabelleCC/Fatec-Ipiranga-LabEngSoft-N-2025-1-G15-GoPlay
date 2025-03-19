using System.ComponentModel.DataAnnotations;

namespace GoPlay_App.Api.Controllers.AccessManager.Models
{
    public class PasswordResetLinkRequest
    {
        [Required]
        public string Email { get; set; }
    }
}
