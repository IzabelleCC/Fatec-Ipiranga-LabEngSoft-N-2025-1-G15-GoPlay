namespace GoPlay_App.Api.Controllers.TournamentManager.Models
{
    public class ConfirmAttendanceRequest
    {
        public int RegistrationCategoryId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}