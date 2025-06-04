namespace GoPlay_App.Api.Controllers.TournamentManager.Models
{
    public class ConfirmAttendanceRequest
    {
        public int RegistrationCategoryId { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
    }
}