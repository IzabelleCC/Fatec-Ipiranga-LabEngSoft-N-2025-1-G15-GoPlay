using GoPlay_Core.Business.Interfaces;
using GoPlay_Core.Entities;
using GoPlay_Core.Repository.Interfaces;
using FluentValidation;
using GoPlay_Core.Services;
using GoPlay_Core.Services.Interfaces;

namespace GoPlay_Core.Business
{
    /// <summary>
    /// Classe de negócio de usuário
    /// </summary>
    public class UserBusiness : IUserBusiness<UserEntity>
    {

        private readonly IUserRepository _repository;
        private readonly IValidator<UserEntity> _validator;
        private readonly IEmailService _emailService;

        public UserBusiness(IUserRepository repository, IValidator<UserEntity> validator, IEmailService emailService)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        }

        public async Task Add(UserEntity entity, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(entity, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }
            await _repository.Add(entity);
            await _emailService.SendEmailRegisterAsync(entity);
        }
        public async Task Update(UserEntity entity, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(entity, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var entityToUpdate = await _repository.GetById(entity.Id);

            if (entityToUpdate == null)
            {
                throw new InvalidOperationException("Usuário não encontrado.");
            }
            entityToUpdate.Name = entity.Name;
            entityToUpdate.UserName = entity.UserName;
            entityToUpdate.InstagramPage = entity.InstagramPage;
            entityToUpdate.Gender = entity.Gender;
            entityToUpdate.BirthDate = entity.BirthDate;
            entityToUpdate.TShirtSize = entity.TShirtSize;
            entityToUpdate.PhoneNumber = entity.PhoneNumber;

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
