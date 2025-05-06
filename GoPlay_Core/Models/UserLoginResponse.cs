using GoPlay_App.Api.Controllers.UserController.Models;
using GoPlay_Core.Entities;

namespace GoPlay_App.Api.Controllers.AccessManager.Models
{
    public class UserLoginResponse
    {
        public string Token { get; set; }
        public UserResponse User { get; set; }

        public UserLoginResponse(string token, UserResponse user)
        {
            Token = token;
            User = user;
        }
    }
}
