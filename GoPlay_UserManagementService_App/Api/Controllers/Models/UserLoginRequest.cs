using System.ComponentModel.DataAnnotations;
using GoPlay_UserManagementService_Core.Entities;

namespace GoPlay_UserManagementService_App.Api.Controllers.Models
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

