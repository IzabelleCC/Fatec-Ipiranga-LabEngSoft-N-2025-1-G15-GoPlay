namespace GoPlay_App.Api.Controllers.TournamentManager.Models
{
    public class CategoryCreateRequest
    {
        public string CategoryType { get; set; }
        public int PlayerLimit { get; set; }
        public bool IsDoubles { get; set; }
    }
}
