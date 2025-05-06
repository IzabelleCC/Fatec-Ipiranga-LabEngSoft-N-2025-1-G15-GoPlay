using GoPlay_App.Api.Controllers.AccessManager.Models;
using GoPlay_Core.Entities;

namespace GoPlay_Core.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserLoginResponse> Login(LoginEntity entity);
        Task Logout();
    }

}
