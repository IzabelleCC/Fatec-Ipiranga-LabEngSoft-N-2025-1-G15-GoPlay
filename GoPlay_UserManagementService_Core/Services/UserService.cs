using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoPlay_UserManagementService_Core.Entities;
using GoPlay_UserManagementService_Core.Repository.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GoPlay_UserManagementService_Core.Services
{
    public class UserService
    {
        private readonly ILogger<UserEntity> _logger;
        private readonly SignInManager<UserEntity> _signInManeger;
        private readonly TokenService _tokenService;

        public UserService(ILogger<UserEntity> logger, SignInManager<UserEntity> signInManager, TokenService tokenService)
        {
            _signInManeger = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        }

        public async Task<string> Login(LoginEntity entity)
        {
            try
            {
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

                var token = await _tokenService.GenerateToken(user);

                return token;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while logging in.");
                throw new InvalidOperationException("An error occurred while logging in.", ex);
            }
        }
    }
}
