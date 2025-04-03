using GoPlay_Core.Entities;

namespace GoPlay_App.Api.Controllers.UserController.Models
{
    public class UserUpDateRequest
    {
        public string Id { get; set; }
        public string? Name { get; set; }
        public string? UserName { get; set; }
        public string? InstagramPage { get; set; }
        public string? Gender { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? TShirtSize { get; set; }
        public string? Phone { get; set; }

        /// <summary>
        /// Construtor da classe UserCreateRequest
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="userName"></param>
        /// <param name="instagramPage"></param>
        /// <param name="gender"></param>  
        /// <param name="birthDate"></param>
        /// <param name="tShirtSize"></param>
        /// <param name="phone"></param>
        public UserUpDateRequest(string id,
                                 string name,
                                 string? userName,
                                 string? instagramPage,
                                 string? gender,
                                 DateTime? birthDate,
                                 string? tShirtSize,
                                 string? phone)
        {
            Id = id;
            Name = name;
            UserName = userName;
            InstagramPage = instagramPage;
            Gender = gender;
            BirthDate = birthDate;
            TShirtSize = tShirtSize;
            Phone = phone;
        }

        public UserEntity ToUserEntity()
            => new()
            {
                Id = Id,
                Name = Name,
                UserName = UserName,
                InstagramPage = InstagramPage,
                Gender = Gender,
                BirthDate = BirthDate?.ToUniversalTime(),
                TShirtSize = TShirtSize,
                PhoneNumber = Phone
            };
    }
}
