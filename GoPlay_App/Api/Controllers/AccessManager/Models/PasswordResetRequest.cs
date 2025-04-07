using System.ComponentModel.DataAnnotations;

namespace GoPlay_App.Api.Controllers.AccessManager.Models
{
    /// <summary>
    /// Classe de requisição para redefinição de senha
    /// </summary>
    public class PasswordResetRequest
    {
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
