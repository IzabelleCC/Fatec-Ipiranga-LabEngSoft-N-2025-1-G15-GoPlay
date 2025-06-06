using GoPlay_Core.Entities;

namespace GoPlay_App.Api.Controllers.TournamentManager.Models
{
    public class GroupResultsRequest
    {
        public int RegistrationCategoryId { get; set; }
        public int? Game1 { get; set; }
        public int? Game2 { get; set; }
        public int? Game3 { get; set; }
        public int? Game4 { get; set; }
        public int? Game5 { get; set; }

        public GroupResultsRequest(int registrationCategoryId, int? game1, int? game2, int? game3, int? game4, int? game5)
        {
            RegistrationCategoryId = registrationCategoryId;
            Game1 = game1;
            Game2 = game2;
            Game3 = game3;
            Game4 = game4;
            Game5 = game5;
        }

        public MatchGroupEntity ToMatchGroupEntity()
        {
            return new MatchGroupEntity
            {
                RegistrationCategoryId = RegistrationCategoryId,
                Game1 = Game1 ?? 0,
                Game2 = Game2 ?? 0,
                Game3 = Game3 ?? 0,
                Game4 = Game4 ?? 0,
                Game5 = Game5 ??0
            };
        }
    }
}