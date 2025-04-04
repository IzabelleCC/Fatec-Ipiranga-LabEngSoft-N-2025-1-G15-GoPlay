using GoPlay_Core.Entities;

namespace GoPlay_Core.Services.Interfaces
{
    public interface ITokenService
    {
        Task<string> GenerateToken(UserEntity user);
        bool ValidateToken(string token);
    }

}
