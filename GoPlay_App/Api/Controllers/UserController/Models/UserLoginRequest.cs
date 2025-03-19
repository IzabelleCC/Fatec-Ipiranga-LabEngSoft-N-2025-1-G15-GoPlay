using System.ComponentModel.DataAnnotations;
using GoPlay_Core.Entities;

namespace GoPlay_App.Api.Controllers.UserController.Models
{
    public class UserLoginRequest
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }


        public UserLoginRequest(string userName, string password)
        {
            UserName = userName;
            Password = password;
        }
        public LoginEntity ToLoginEntity()
            => new()
            {
                UserName = UserName,
                Password = Password
            };
    }
}

