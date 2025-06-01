using System.Text.Json.Serialization;
using GoPlay_Core.Enum;

namespace GoPlay_Core.Entities
{
    public class CategoryPlayerEntity
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string FirstUserId { get; set; } = null!;
        public string? SecondUserId { get; set; }
        public RegisterStatus RegisterStatus { get; set; } = RegisterStatus.InscricaoRealizada;
        public bool FirstUserPaymentConfirmed { get; set; } = false;
        public bool SecondUserPaymentConfirmed { get; set; }
        public string? FirstUserTxId { get; set; }
        public string? SecondUserTxId { get; set; }

        [JsonIgnore]
        public CategoryEntity Category { get; set; } = null!;

        [JsonIgnore]
        public UserEntity FirstUser { get; set; } = null!;

        [JsonIgnore]
        public UserEntity? SecondUser { get; set; }
    }
}
