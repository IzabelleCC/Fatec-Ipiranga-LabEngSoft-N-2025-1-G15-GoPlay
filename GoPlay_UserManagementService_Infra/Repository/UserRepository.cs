using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoPlay_UserManagementService_Core.Entities;
using GoPlay_UserManagementService_Core.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GoPlay_UserManagementService_Infra.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly GoPlayContext _context;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(GoPlayContext context, ILogger<UserRepository> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Add(UserEntity entity)
        {
            try
            {
                await _context.AddAsync(entity);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while saving the entity changes.");
                throw new InvalidOperationException("An error occurred while saving the entity changes.", ex);
            }
        }

        public async Task Delete(UserEntity entity)
        {
            try
            {
                _context.Remove(entity);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the entity.");
                throw new InvalidOperationException("An error occurred while deleting the entity.", ex);
            }
        }

        public async Task<UserEntity?> GetById(int id)
        {
            try
            {
                return await _context.User.FirstOrDefaultAsync(u => u.IdUser == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the entity.");
                throw new InvalidOperationException("An error occurred while retrieving the entity.", ex);
            }
        }

        public async Task Update(UserEntity entity, int id)
        {
            try
            {
                _logger.LogInformation("Attempting to update user with ID: {IdUser}", id);

                var user = await _context.User.AsNoTracking().FirstOrDefaultAsync(u => u.IdUser == id);

                if (user != null)
                {
                    _logger.LogInformation("User found. Updating details.");
                    user.Name = entity.Name;
                    user.Email = entity.Email;
                    user.Password = entity.Password;
                    user.InstagramPage = entity.InstagramPage;
                    user.Gender = entity.Gender;
                    user.BirthDate = entity.BirthDate;
                    user.TShirtSize = entity.TShirtSize;
                    _context.User.Update(user);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    _logger.LogWarning("User with ID: {IdUser} not found.", entity.IdUser);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the entity.");
                throw new InvalidOperationException("An error occurred while updating the entity.", ex);
            }
        }
    }
}
