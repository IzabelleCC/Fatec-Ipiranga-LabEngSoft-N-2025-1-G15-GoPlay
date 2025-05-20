using System.ComponentModel.DataAnnotations;
using GoPlay_Core.Entities;
using GoPlay_Core.Enum;

namespace GoPlay_App.Api.Controllers.UserController.Models
{
    /// <summary>
    /// Classe de requisição para criação de usuário
    /// </summary>
    public class UserCreateRequest
    {
        public string Name { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Compare ("Password")]
        public string ConfirmPassword { get; set; }
        public int UserType { get; set; }
        public string? InstagramPage { get; set; }
        public string CpfCnpj { get; set; }
        public string? Gender { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? TShirtSize { get; set; }
        public string? Phone { get; set; }

        /// <summary>
        /// Construtor da classe UserCreateRequest
        /// </summary>
        /// <param name="name"></param>
        /// <param name="email"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="userType"></param>
        /// <param name="instagramPage"></param>
        /// <param name="cpfCnpj"></param>
        /// <param name="gender"></param>
        /// <param name="birthDate"></param>
        /// <param name="tShirtSize"></param>
        /// <param name="phone"></param>
        public UserCreateRequest(string name,
                                 string email,
                                 string userName,
                                 string password,
                                 int userType,
                                 string? instagramPage,
                                 string cpfCnpj,
                                 string? gender,
                                 DateTime? birthDate,
                                 string? tShirtSize,
                                 string? phone)
        {
            Name = name;
            UserName = userName;
            Email = email;
            Password = password;
            UserType = userType;
            InstagramPage = instagramPage;
            CpfCnpj = cpfCnpj;
            Gender = gender;
            BirthDate = birthDate;
            TShirtSize = tShirtSize;
            Phone = phone;
        }

        /// <summary>
        /// Converte a classe UserCreateRequest em um objeto UserEntity
        /// </summary>
        /// <returns></returns>
        public UserEntity ToUserEntity()
            => new()
            {                
                Email = Email,
                UserName = UserName,
                Name = Name,
                PasswordHash = Password,
                UserType = (UserTypeEnum)UserType,
                InstagramPage = InstagramPage,
                CpfCnpj = CpfCnpj,
                Gender = Gender,
                BirthDate = BirthDate?.ToUniversalTime() ?? null,
                TShirtSize = TShirtSize,
                PhoneNumber = Phone
            };
    }
}
