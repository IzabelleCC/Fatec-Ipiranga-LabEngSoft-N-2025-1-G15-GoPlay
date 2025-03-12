using GoPlay_UserManagementService_Core.Entities;
using GoPlay_UserManagementService_Core.Enum;

namespace GoPlay_UserManagementService_App.Api.Controllers.Models
{
    public class UserUpDateRequest
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string? InstagramPage { get; set; }
        public string? Gender { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? TShirtSize { get; set; }

        /// <summary>
        /// Construtor da classe UserCreateRequest
        /// </summary>
        /// <param name="name"></param>
        /// <param name="email"></param>
        /// <param name="login"></param>
        /// <param name="password"></param>
        /// <param name="userType"></param>
        /// <param name="instagramPage"></param>
        /// <param name="cpf"></param>
        /// <param name="cnpj"></param>
        /// <param name="birthDate"></param>
        /// <param name="tShirtSize"></param>
        public UserUpDateRequest(string name,
                                 string email,
                                 string password,
                                 string? instagramPage,
                                 string? gender,
                                 DateTime? birthDate,
                                 string? tShirtSize)
        {
            Name = name;
            Email = email;
            Password = password;
            InstagramPage = instagramPage;
            Gender = gender;
            BirthDate = birthDate;
            TShirtSize = tShirtSize;
        }

        public UserEntity ToUserEntity()
            => new()
            {
                UserName = Name,
                Email = Email,
                PasswordHash = Password,
                InstagramPage = InstagramPage,
                Gender = Gender,
                BirthDate = BirthDate?.ToUniversalTime(),
                TShirtSize = TShirtSize
            };
    }
}
