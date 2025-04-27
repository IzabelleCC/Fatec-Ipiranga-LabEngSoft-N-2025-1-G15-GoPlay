using GoPlay_App.Api.Controllers.TournamentManager;
using GoPlay_Core.Entities;
using GoPlay_Core.Repository.Interfaces;

namespace GoPlay_Core.Business
{
    public class TournamentBusiness : ITournamentBusiness<TournamentEntity>
    {
        private readonly ITournamentRepository _repository;
        public TournamentBusiness(ITournamentRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }
        public async Task Add(TournamentEntity entity, CancellationToken cancellationToken)
        {
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
