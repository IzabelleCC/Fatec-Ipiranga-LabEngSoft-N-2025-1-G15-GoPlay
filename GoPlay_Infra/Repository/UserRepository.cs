using GoPlay_Core.Entities;
using GoPlay_Core.Repository.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GoPlay_Infra.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ILogger<UserRepository> _logger;
        private readonly UserManager<UserEntity> _userManager;

        public UserRepository(ILogger<UserRepository> logger, UserManager<UserEntity> userManager)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        public async Task Add(UserEntity entity)
        {
            try
            {
                await _userManager.CreateAsync(entity, entity.PasswordHash ?? string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocorreu um erro ao salvar as alterações do usuário.");
                throw new InvalidOperationException("Erro ao salvar novo usuário.", ex);
            }
        }

        public async Task Delete(UserEntity entity)
        {
            try
            {
                await _userManager.DeleteAsync(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocorreu um erro ao excluir o usuário.");
                throw new InvalidOperationException("Ocorreu um erro ao excluir o usuário.", ex);
            }
        }

        public async Task<UserEntity?> GetById(string id)
        {
            try
            {
                return await _userManager.FindByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocorreu um erro ao recuperar o usuário.");
                throw new InvalidOperationException("Ocorreu um erro ao recuperar o usuário.", ex);
            }
        }

        public async Task Update(UserEntity entity)
        {
            try
            {
                await _userManager.UpdateAsync(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocorreu um erro ao atualizar o usuário.");
                throw new InvalidOperationException("Ocorreu um erro ao atualizar o usuário.", ex);
            }
        }

        public async Task<UserEntity?> GetByUserName(string userName)
        {
            try
            {
                return await _userManager.FindByNameAsync(userName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocorreu um erro ao recuperar o usuário.");
                throw new InvalidOperationException("Ocorreu um erro ao recuperar o usuário.", ex);
            }
        }

        public async Task<UserEntity?> GetByEmail(string email)
        {
            try
            {
                return await _userManager.FindByEmailAsync(email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocorreu um erro ao recuperar o usuário.");
                throw new InvalidOperationException("Ocorreu um erro ao recuperar o usuário.", ex);
            }
        }

        public async Task<UserEntity?> GetByCpfCnpj(string cpfCnpj)
        {
            try
            {
                var user = await _userManager.Users.FirstOrDefaultAsync(u => u.CpfCnpj == cpfCnpj);
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocorreu um erro ao recuperar o usuário.");
                throw new InvalidOperationException("Ocorreu um erro ao recuperar o usuário.", ex);
            }
        }

        public async Task<bool> UpDatePassword(string idUser, string password)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(idUser);
                if (user == null)
                    return false;

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, token, password);

                if (result.Succeeded)
                    return true;

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocorreu um erro ao atualizar a senha do usuário.");
                throw new InvalidOperationException("Ocorreu um erro ao atualizar a senha do usuário.", ex);
            }
        }
    }
}
