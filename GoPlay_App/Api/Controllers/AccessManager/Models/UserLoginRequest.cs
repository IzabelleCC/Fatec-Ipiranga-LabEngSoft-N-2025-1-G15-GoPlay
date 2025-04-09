using GoPlay_Core.Entities;

namespace GoPlay_App.Api.Controllers.AccessManager.Models
{
    /// <summary>
    /// Classe de requisição para login de usuário
    /// </summary>
    public class UserLoginRequest
    {
        public string UserName { get; set; }
        public string Password { get; set; }

        /// <summary>
        /// Construtor da classe UserLoginRequest
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        public UserLoginRequest(string userName, string password)
        {
            UserName = userName;
            Password = password;
        }

        /// <summary>
        /// Converte a classe UserLoginRequest em um objeto LoginEntity
        /// </summary>
        /// <returns></returns>
        public LoginEntity ToLoginEntity()
            => new()
            {
                UserName = UserName ?? string.Empty,
                Password = Password ?? string.Empty
            };
    }
}

