using GoPlay_Core.Entities;

namespace GoPlay_Core.Services.Interfaces
{
    public interface IUserService
    {
        Task<string> Login(LoginEntity entity);
        Task Logout();
    }

}
