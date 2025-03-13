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
        public async Task Update(UserEntity entity, int id, CancellationToken cancellationToken)
        {
            var validationResult = _validator.Validate(entity);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }
            await _repository.Update(entity, id);
        }

        public async Task Delete(UserEntity entity, CancellationToken cancellationToken)
        {
            await _repository.Delete(entity);
        }

    }
}
