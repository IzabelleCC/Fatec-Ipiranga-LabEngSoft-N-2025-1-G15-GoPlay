using GoPlay_Core.Models.Dto;
using GoPlay_Core.Entities;

namespace GoPlay_Core.Business.Interfaces
{
    public interface IMatchGroupBusiness
    {
        Task<TournamentMatchesResultDto> GenerateMatchesForTournament(int tournamentId, CancellationToken cancellationToken);

        Task<TournamentEntity> GetTournament(int tournamentId);

        Task<List<CategoryEntity>> GetCategoriesByTournament(int tournamentId);

        Task<CategoryGroupsDto?> ProcessCategory(CategoryEntity category);

        List<List<CategoryPlayerEntity>> DistributeIntoGroups(List<CategoryPlayerEntity> confirmed);
    }
}
