using GoPlay_Core.Entities;

namespace GoPlay_Core.Repository.Interfaces
{
    public interface ITournamentRepository
    {
        Task Add(Tournament entity);
        Task Update(Tournament entity);
        Task Delete(Tournament entity);
        Task<List<Tournament?>> GetAllTournaments();
        Task<Tournament?> GetById(int id);
        Task<Tournament?> GetByName(string name);
        Task<List<Tournament?>> GetByLocation(string location);
        Task<List<Tournament?>> GetByDate(DateTime date);
    }
}
