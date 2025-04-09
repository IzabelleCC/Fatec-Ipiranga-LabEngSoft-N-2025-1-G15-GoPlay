using GoPlay_App.Api.Controllers.AccessManager.Models;
using GoPlay_Core.Entities;
using GoPlay_Core.Exceptions;
using GoPlay_Core.Repository.Interfaces;
using GoPlay_Core.Services;
using GoPlay_Core.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace GoPlay_App.Api.Controllers.AccessManager
{
    /// <summary>
    /// Controlador responsável por gerenciar o acesso dos usuários
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AccessManagerController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly UserManager<UserEntity> _user;
        private readonly IUserRepository _repository;

        /// <summary>
        /// Construtor do controlador de gerenciamento de acesso
        /// </summary>
        /// <param name="userService"></param>
        /// <param name="tokenService"></param>
        public AccessManagerController(IUserService userService, ITokenService tokenService, IEmailService emailService, IConfiguration configuration, UserManager<UserEntity> user, IUserRepository repository)
        {
            _userService = userService;
            _tokenService = tokenService;
            _emailService = emailService;
            _configuration = configuration;
            _user = user;
            _repository = repository;
        }

        private IActionResult HandleException(Exception ex)
        {
            if (ex is NotFoundException)
                return NotFound(new { message = ex.Message });

            return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
        }

        /// <summary>
        /// Permite acesso a usuários autenticados
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> ValidateUser()
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (_tokenService.ValidateToken(token) == null )
            {
                return Unauthorized(new { message = "Token inválido." });
            }
            else
            {
                return Ok(new { message = "Acesso permitido." });
            }
        }

        /// <summary>
        /// Realiza login de um usuário
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost("Login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Login([FromBody] UserRequestBase<UserLoginRequest> request, CancellationToken cancellationToken)
        {
            try
            {
                var entity = request.Data.ToLoginEntity();
                var token = await _userService.Login(entity);

                return Ok(new
                {
                    message = "Login realizado com sucesso.",
                    token = token
                });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new
                {
                    message = "Usuário não encontrado.",
                    error = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = "Erro ao realizar login.",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Realiza logout de um usuário
        /// </summary>
        /// <returns></returns>
        [HttpPost("Logout")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await _userService.Logout();
                return Ok(new { message = "Logout realizado com sucesso." });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = "Erro ao realizar logout.",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Envia um link para redefinição de senha para o e-mail do usuário
        /// </summary>
        [HttpPost("SendPasswordResetLink")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SendPasswordResetLink([FromBody] UserRequestBase<PasswordResetLinkRequest> request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _user.FindByEmailAsync(request.Data.Email);
                if (user == null)
                    throw new NotFoundException("Email não cadastrado.");

                await _emailService.SendPasswordResetLinkAsync(user);

                return Ok(new { message = "Link de redefinição enviado com sucesso." });
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Redefine a senha do usuário
        /// </summary>
        [HttpPost("ResetPassword")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ResetPassword([FromQuery] string token, [FromBody] UserRequestBase<PasswordResetRequest> request, CancellationToken cancellationToken)
        {
            try
            {
                var validateToken = _tokenService.ValidateToken(token);

                if (validateToken == null)
                    return BadRequest(new { message = "Token inválido." });

                if (string.IsNullOrEmpty(request.Data.Password))
                    return BadRequest(new { message = "Senha Obrigatória." });

                if(request.Data.Password != request.Data.ConfirmPassword)
                    return BadRequest(new { message = "As senhas não coincidem." });

                var result = await _repository.UpDatePassword(validateToken, request.Data.Password);
               
                if (result)
                    return Ok(new { message = "Senha redefinida com sucesso." });
                else
                    return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Erro ao redefinir a senha." });
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}


