﻿using GoPlay_UserManagementService_Core.Entities;
using GoPlay_UserManagementService_Core.Enum;

namespace GoPlay_UserManagementService_App.Api.Controllers.Models
{
    public class UserCreateRequest
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public int UserType { get; set; }
        public string? InstagramPage { get; set; }
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
                                 string login,
                                 string password,
                                 int userType,
                                 string? instagramPage,
                                 string cpfCnpj,
                                 string? gender,
                                 DateTime? birthDate,
                                 string? tShirtSize)
        {
            Name = name;
            Email = email;
            Login = login;
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
                Name = Name,
                Email = Email,
                Login = Login,
                Password = Password,
                UserType = (UserTypeEnum)UserType,
                InstagramPage = InstagramPage,
                CpfCnpj = CpfCnpj,
                Gender = Gender,
                BirthDate = BirthDate?.ToUniversalTime(),
                TShirtSize = TShirtSize
            };
    }
}
