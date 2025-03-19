using System.ComponentModel.DataAnnotations;

namespace GoPlay_App.Api.Controllers.UserController.Models
{
    public class PasswordResetLinkRequest
    {
        [Required]
        public string Email { get; set; }
    }
}
