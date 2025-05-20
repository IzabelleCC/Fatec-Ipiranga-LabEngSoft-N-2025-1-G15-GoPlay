using GoPlay_Core.Entities;

namespace GoPlay_App.Api.Controllers.UserController.Models
{
    /// <summary>
    /// Classe de resposta do usuário
    /// </summary>
    public class UserResponse
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string? InstagramPage { get; set; }
        public string? Gender { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? TShirtSize { get; set; }
        public string? Phone { get; set; }
        public int? UserType { get; set; }

        /// <summary>
        /// Construtor da classe UserCreateRequest
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="email"></param>
        /// <param name="userName"></param>
        /// <param name="instagramPage"></param>
        /// <param name="gender"></param>
        /// <param name="birthDate"></param>
        /// <param name="tShirtSize"></param>
        /// <param name="phone"></param>
        /// <param name="userType"></param>
        public UserResponse( string id,
                                 string name,
                                 string email,
                                 string userName,
                                 string? instagramPage,
                                 string? gender,
                                 DateTime? birthDate,
                                 string? tShirtSize,
                                 string? phone,
                                 int? userType)
        {
            Id = id;
            Name = name;
            UserName = userName;
            Email = email;
            InstagramPage = instagramPage;
            Gender = gender;
            BirthDate = birthDate;
            TShirtSize = tShirtSize;
            Phone = phone;
            UserType = userType;
        }
        public static UserResponse ConvertToUserResponse(UserEntity userEntity)
        {
            if (userEntity == null)
            {
                throw new ArgumentNullException(nameof(userEntity));
            }

            return new UserResponse(
                userEntity.Id,
                userEntity.Name,
                userEntity.Email,
                userEntity.UserName,
                userEntity.InstagramPage,
                userEntity.Gender,
                userEntity.BirthDate,
                userEntity.TShirtSize,
                userEntity.PhoneNumber,
                (int?)userEntity.UserType
            );
        }

    }
}
