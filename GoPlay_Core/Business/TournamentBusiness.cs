using System.Threading;
using FluentValidation;
using GoPlay_App.Api.Controllers.TournamentManager;
using GoPlay_App.Api.Controllers.TournamentManager.Models;
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
        public async Task Delete(TournamentEntity entity, CancellationToken cancellationToken)
        {
            await _repository.Delete(entity);
        }
        public async Task<List<TournamentEntity>> GetAllTournaments(CancellationToken cancellationToken)
        {
            var result = await _repository.GetAllTournaments();
            if (result == null)
            {
                throw new InvalidOperationException("Nenhum torneio encontrado.");
            }

            var tournaments = result
                .Where(t => t != null && t.IsActive == true)
                .ToList();

            return tournaments;
        }
        public async Task<List<TournamentEntity>> GetAllByTournamentName(string tournamentName, CancellationToken cancellationToken)
        {
            var result = await _repository.GetAllByName(tournamentName);

            if (result == null)
            {
                throw new InvalidOperationException("Nenhum torneio encontrado.");
            }

            var tournaments = result
                .Where(t => t != null && t.IsActive == true)
                .ToList();

            return tournaments;
        }
        public async Task<TournamentEntity> GetTournamentById(int tournamentId, CancellationToken cancellationToken)
        {
            var result = await _repository.GetById(tournamentId);

            if (result == null)
            {
                throw new InvalidOperationException("Nenhum torneio encontrado.");
            }

            return result;
        }
        public async Task<List<TournamentEntity>> GetTournamentByAdmUserId(string id, CancellationToken cancellationToken)
        {
            var result = await _repository.GetTournamentByAdmUserId(id);
            if (result == null)
            {
                throw new InvalidOperationException("Nenhum torneio encontrado.");
            }
            var tournaments = result
                .Where(t => t != null && t.IsActive == true)
                .ToList();

            return tournaments;

        }
        public async Task Update(TournamentEntity entity, CancellationToken cancellationToken)
        {
            var existingTournament = await _repository.GetById(entity.Id);
            if (existingTournament == null)
            {
                throw new InvalidOperationException("Torneio não encontrado para atualização.");
            }

            await _repository.Update(entity);
        }
    }
}
