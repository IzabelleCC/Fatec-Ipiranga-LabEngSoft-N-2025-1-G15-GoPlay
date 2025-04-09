
using GoPlay_Core.Entities;

namespace GoPlay_Core.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailRegisterAsync(UserEntity user);
        Task SendPasswordResetLinkAsync(UserEntity user);
    }
}
