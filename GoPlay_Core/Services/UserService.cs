using GoPlay_App.Api.Controllers.AccessManager.Models;
using GoPlay_App.Api.Controllers.UserController.Models;
using GoPlay_Core.Entities;
using GoPlay_Core.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GoPlay_Core.Services
{
    public class UserService : IUserService
    {
        private readonly ILogger<UserEntity> _logger;
        private readonly SignInManager<UserEntity> _signInManeger;
        private readonly ITokenService _tokenService;

        public UserService(ILogger<UserEntity> logger, SignInManager<UserEntity> signInManager, ITokenService tokenService)
        {
            _signInManeger = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        }

        public async Task<UserLoginResponse> Login(LoginEntity entity)
        {
            try
            {
                if (string.IsNullOrEmpty(entity.UserName) || string.IsNullOrEmpty(entity.Password))
                {
                    throw new ArgumentException("Usuário e senha são campos obrigatórios.");
                }

                var result = await _signInManeger.PasswordSignInAsync(entity.UserName, entity.Password, false, false);

                if (!result.Succeeded)
                {
                    throw new InvalidOperationException("Usuário ou senha inválidos.");
                }

                var user = await _signInManeger.UserManager.Users
                    .FirstOrDefaultAsync(u => u.NormalizedUserName == entity.UserName.ToUpper());


                if (user == null)
                {
                    throw new InvalidOperationException("Usuário não encontrado.");
                }
                var userResponse = UserResponse.ConvertToUserResponse(user);

                var token = await _tokenService.GenerateToken(user);

                return new UserLoginResponse(token, userResponse);
            }
            catch
            {
                throw;
            }
        }

        public async Task Logout()
        {
            await _signInManeger.SignOutAsync();
        }
    }
}
