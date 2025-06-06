using GoPlay_Core.Entities;
using GoPlay_Core.Models;
using GoPlay_Core.Models.Dto;


namespace GoPlay_Core.Business.Interfaces
{
    public interface IMatchGroupBusiness
    {
        Task<TournamentMatchesResultDto> GenerateMatchesForTournament(int tournamentId, CancellationToken cancellationToken);

        Task<TournamentEntity> GetTournament(int tournamentId);

        Task<List<CategoryEntity>> GetCategoriesByTournament(int tournamentId);

        Task<CategoryGroupsDto?> ProcessCategory(CategoryEntity category);

        List<List<CategoryPlayerEntity>> DistributeIntoGroups(List<CategoryPlayerEntity> confirmed);

        Task<bool> ConfirmAttendance(int registrationCategoryId, double latitude, double longitude, CancellationToken cancellationToken);

        Task<List<int>> InsertGroupResultsAndReturnWinners(List<MatchGroupEntity> results, CancellationToken cancellationToken);
    }
}
