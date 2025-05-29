namespace GoPlay_App.Api.Controllers.CategoryPlayerController.Models
{
    public class CategoryRegistrationRequest
    {
        public int CategoryId { get; set; }

        public string FirstUserId { get; set; }
        public string? SecondUserId { get; set; }
    }
}
