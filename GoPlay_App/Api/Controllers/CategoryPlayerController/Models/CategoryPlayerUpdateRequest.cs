using GoPlay_Core.Entities;

namespace GoPlay_App.Api.Controllers.CategoryPlayerController.Models
{
    public class CategoryPlayerUpdateRequest
    {
        public int Id { get; set; }

        public string FirstUserId { get; set; } = null!;
        public string? SecondUserId { get; set; }

        public bool FirstUserPaymentConfirmed { get; set; }
        public bool SecondUserPaymentConfirmed { get; set; }

        public void ToEntity(CategoryPlayerEntity entity)
        {
            entity.FirstUserId = FirstUserId;
            entity.SecondUserId = SecondUserId;
            entity.FirstUserPaymentConfirmed = FirstUserPaymentConfirmed;
            entity.SecondUserPaymentConfirmed = SecondUserPaymentConfirmed;
        }
    }
}
