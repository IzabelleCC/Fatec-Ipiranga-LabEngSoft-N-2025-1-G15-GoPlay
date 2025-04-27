using GoPlay_Core.Entities;
using GoPlay_Core.Repository.Interfaces;

namespace GoPlay_Infra.Repository
{
    public class TournamentRepository : ITournamentRepository
    {
        public async Task Add(TournamentEntity entity)
        {

         }
        public async Task Update(TournamentEntity entity)
        {
            throw new NotImplementedException();
        }
        public async Task Delete(TournamentEntity entity)
        {
            throw new NotImplementedException();
        }
        public async Task<List<TournamentEntity?>> GetAllTournaments()
        {
            throw new NotImplementedException();
        }
        public async Task<TournamentEntity?> GetById(int id)
        {
            throw new NotImplementedException();
        }
        public async Task<TournamentEntity?> GetByName(string name)
        {
            throw new NotImplementedException();
        }
        public async Task<List<TournamentEntity?>> GetByLocation(string location)
        {
            throw new NotImplementedException();
        }
        public async Task<List<TournamentEntity?>> GetByDate(DateTime date)
        {
            throw new NotImplementedException();
        }
    }
}
