using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoPlay_UserManagementService_Core.Business.Interfaces;
using GoPlay_UserManagementService_Core.Entities;
using GoPlay_UserManagementService_Core.Repository.Interfaces;
using Microsoft.EntityFrameworkCore.Infrastructure;
using FluentValidation;
using GoPlay_UserManagementService_Core.Services;

namespace GoPlay_UserManagementService_Core.Business
{
    /// <summary>
    /// Classe de negócio de usuário
    /// </summary>
    public class UserBusiness : IUserBusiness
    {

        private readonly IUserRepository _repository;

        private readonly IValidator<UserEntity> _validator;


        public UserBusiness(IUserRepository repository, IValidator<UserEntity> validator)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }

        public async Task Add(UserEntity entity, CancellationToken cancellationToken)
        {
            var validationResult = _validator.Validate(entity);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }
            await _repository.Add(entity);
        }
        public async Task Update(UserEntity entity, CancellationToken cancellationToken)
        {
            var entityToUpdate = await _repository.GetById(entity.Id);

            if (entityToUpdate == null)
            {
                throw new InvalidOperationException("Usuário não encontrado.");
            }
            entityToUpdate.Name = entity.Name;
            entityToUpdate.Email = entity.Email;
            entityToUpdate.InstagramPage = entity.InstagramPage;
            entityToUpdate.Gender = entity.Gender;
            entityToUpdate.BirthDate = entity.BirthDate;
            entityToUpdate.TShirtSize = entity.TShirtSize;

            await _repository.Update(entityToUpdate);
        }

        public async Task Delete(string userName, CancellationToken cancellationToken)
        {
            var entity = await _repository.GetByUserName(userName);

            if (entity == null)
            {
                throw new InvalidOperationException("Usuário não encontrado.");
            }
            await _repository.Delete(entity);
        }

        public async Task<UserEntity> GetByUserName(string userName, CancellationToken cancellationToken)
        {
            return await _repository.GetByUserName(userName);
        }

    }
}
