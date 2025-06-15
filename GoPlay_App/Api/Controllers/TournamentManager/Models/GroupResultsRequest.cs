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
        public int? Game6 { get; set; }
        public int? Game7 { get; set; }
        public int? Game8 { get; set; }
        public int? Game9 { get; set; }
        public int? Game10 { get; set; }


        public GroupResultsRequest(int registrationCategoryId, int? game1, int? game2, int? game3, int? game4, int? game5, int? game6, int? game7, int? game8, int? game9, int? game10)
        {
            RegistrationCategoryId = registrationCategoryId;
            Game1 = game1;
            Game2 = game2;
            Game3 = game3;
            Game4 = game4;
            Game5 = game5;
            Game6 = game6;
            Game7 = game7;
            Game8 = game8;
            Game9 = game9;
            Game10 = game10;
        }

        public MatchGroupEntity ToMatchGroupEntity()
        {
            return new MatchGroupEntity
            {
                RegistrationCategoryId = RegistrationCategoryId,
                Game1 = Game1,
                Game2 = Game2,
                Game3 = Game3,
                Game4 = Game4,
                Game5 = Game5,
                Game6 = Game6,
                Game7 = Game7,
                Game8 = Game8,
                Game9 = Game9,

            };
        }
    }
}