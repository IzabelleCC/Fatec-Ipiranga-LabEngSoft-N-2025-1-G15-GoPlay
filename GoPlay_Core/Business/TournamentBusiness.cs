using FluentValidation;
using GoPlay_App.Api.Controllers.TournamentManager;
using GoPlay_Core.Entities;
using GoPlay_Core.Repository.Interfaces;

namespace GoPlay_Core.Business
{
    public class TournamentBusiness : ITournamentBusiness<TournamentEntity>
    {
        private readonly ITournamentRepository _repository;
        private readonly IValidator<TournamentEntity> _validator;

        public TournamentBusiness(ITournamentRepository repository, IValidator<TournamentEntity> validator)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _validator = validator ?? throw new ArgumentNullException(nameof(validator)); ;
        }
        public async Task Add(TournamentEntity entity, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(entity, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            await _repository.Add(entity);
        }

        public async Task Delete(string tournamentName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
        public async Task<List<TournamentEntity>> GetAllTournaments(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
        public async Task<TournamentEntity> GetByTournamentName(string tournamentName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
        public async Task<TournamentEntity> GetTournamentById(int tournamentId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
        public async Task Update(TournamentEntity entity, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
