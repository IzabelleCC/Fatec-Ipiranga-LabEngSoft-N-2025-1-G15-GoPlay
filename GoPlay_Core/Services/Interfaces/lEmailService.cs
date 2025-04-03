using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoPlay_Core.Entities;

namespace GoPlay_Core.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailRegisterAsync(UserEntity user);
        Task SendPasswordResetLinkAsync(UserEntity user, string email, string token);
    }
}
