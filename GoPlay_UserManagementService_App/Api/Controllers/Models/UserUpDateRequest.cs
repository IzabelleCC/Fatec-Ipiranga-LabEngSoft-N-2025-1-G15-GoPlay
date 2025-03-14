using GoPlay_UserManagementService_Core.Entities;
using GoPlay_UserManagementService_Core.Enum;

namespace GoPlay_UserManagementService_App.Api.Controllers.Models
{
    public class UserUpDateRequest
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string? InstagramPage { get; set; }
        public string? Gender { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? TShirtSize { get; set; }

        /// <summary>
        /// Construtor da classe UserCreateRequest
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="login"></param>
        /// <param name="userType"></param>
        /// <param name="instagramPage"></param>
        /// <param name="cpf"></param>
        /// <param name="cnpj"></param>
        /// <param name="birthDate"></param>
        /// <param name="tShirtSize"></param>
        public UserUpDateRequest(string id,
                                 string name,
                                 string? instagramPage,
                                 string? gender,
                                 DateTime? birthDate,
                                 string? tShirtSize)
        {
            Id = id;
            Name = name;
            InstagramPage = instagramPage;
            Gender = gender;
            BirthDate = birthDate;
            TShirtSize = tShirtSize;
        }

        public UserEntity ToUserEntity()
            => new()
            {
                Id = Id,
                Name = Name,
                InstagramPage = InstagramPage,
                Gender = Gender,
                BirthDate = BirthDate?.ToUniversalTime(),
                TShirtSize = TShirtSize
            };
    }
}
