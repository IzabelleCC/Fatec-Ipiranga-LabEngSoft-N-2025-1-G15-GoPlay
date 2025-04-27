using GoPlay_Core.Entities;
using GoPlay_Core.Repository.Interfaces;

namespace GoPlay_Infra.Repository
{
    public class TournamentRepository : ITournamentRepository
    {
        public Task Add(Tournament entity)
        {
            throw new NotImplementedException();
        }
        public Task Update(Tournament entity)
        {
            throw new NotImplementedException();
        }
        public Task Delete(Tournament entity)
        {
            throw new NotImplementedException();
        }
        public Task<List<Tournament?>> GetAllTournaments()
        {
            throw new NotImplementedException();
        }
        public Task<Tournament?> GetById(int id)
        {
            throw new NotImplementedException();
        }
        public Task<Tournament?> GetByName(string name)
        {
            throw new NotImplementedException();
        }
        public Task<List<Tournament?>> GetByLocation(string location)
        {
            throw new NotImplementedException();
        }
        public Task<List<Tournament?>> GetByDate(DateTime date)
        {
            throw new NotImplementedException();
        }
    }
}
