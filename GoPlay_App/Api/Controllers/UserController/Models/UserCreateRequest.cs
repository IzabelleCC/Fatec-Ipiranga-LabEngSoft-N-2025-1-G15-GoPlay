using System.ComponentModel.DataAnnotations;
using GoPlay_UserManagementService_Core.Entities;
using GoPlay_UserManagementService_Core.Enum;
using GoPlay_UserManagementService_Core.Models.Dto;

namespace GoPlay_App.Api.Controllers.UserController.Models
{
    public class UserCreateRequest
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [Compare ("Password")]
        public string ConfirmPassword { get; set; }
        [Required]
        public int UserType { get; set; }
        public string? InstagramPage { get; set; }
        [Required]
        public string CpfCnpj { get; set; }
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
        public UserCreateRequest(string name,
                                 string email,
                                 string userName,
                                 string password,
                                 int userType,
                                 string? instagramPage,
                                 string cpfCnpj,
                                 string? gender,
                                 DateTime? birthDate,
                                 string? tShirtSize)
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
        }

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
                BirthDate = BirthDate?.ToUniversalTime(),
                TShirtSize = TShirtSize
            };
    }
}
