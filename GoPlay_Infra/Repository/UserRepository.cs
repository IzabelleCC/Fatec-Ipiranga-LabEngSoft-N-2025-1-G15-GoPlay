using GoPlay_Core.Entities;
using GoPlay_Core.Repository.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;

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
                _logger.LogError(ex, "An error occurred while saving the entity changes.");
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
                _logger.LogError(ex, "An error occurred while deleting the entity.");
                throw new InvalidOperationException("An error occurred while deleting the entity.", ex);
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
                _logger.LogError(ex, "An error occurred while retrieving the entity.");
                throw new InvalidOperationException("An error occurred while retrieving the entity.", ex);
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
                _logger.LogError(ex, "An error occurred while updating the entity.");
                throw new InvalidOperationException("An error occurred while updating the entity.", ex);
            }
        }

        public async Task<UserEntity?> GetByUserName(string userName)
        {
            try
            {
                return await _userManager.FindByNameAsync(userName); ;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the entity.");
                throw new InvalidOperationException("An error occurred while retrieving the entity.", ex);
            }
        }

    }
}
