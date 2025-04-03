using GoPlay_App.Api.Controllers.AccessManager.Models;
using GoPlay_App.Api.Controllers.UserController.Models;
using GoPlay_Core.Business.Interfaces;
using GoPlay_Core.Entities;
using GoPlay_Core.Exceptions;
using GoPlay_Core.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace GoPlay_App.Api.Controllers.UserController
{
    /// <summary>
    /// Controller para gerenciar usuários
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserManagerController : ControllerBase
    {
        private readonly IUserBusiness<UserEntity> _business;
        private readonly IEmailService _emailService;
        private readonly UserManager<UserEntity> _user;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Construtor do UserManagerController
        /// </summary>
        public UserManagerController(
            IUserBusiness<UserEntity> business,
            IEmailService emailService,
            UserManager<UserEntity> user,
            IConfiguration configuration)
        {
            _business = business ?? throw new ArgumentNullException(nameof(business));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _user = user ?? throw new ArgumentNullException(nameof(user));
            _configuration = configuration;
        }

        private IActionResult HandleException(Exception ex)
        {
            if (ex is NotFoundException)
                return NotFound(new { message = ex.Message });

            return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
        }

        /// <summary>
        /// Adiciona um novo usuário
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AllowAnonymous]
        public async Task<IActionResult> Add([FromBody] UserRequestBase<UserCreateRequest> request, CancellationToken cancellationToken)
        {
            try
            {
                if (request?.Data == null)
                    return BadRequest(new { message = "Dados inválidos enviados." });

                var entity = request.Data.ToUserEntity();
                await _business.Add(entity, cancellationToken);
                return Ok(new { message = "Usuário criado com sucesso." });
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Busca um usuário pelo UserName
        /// </summary>
        [HttpGet("GetByUserName/{userName}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByUserName(string userName, CancellationToken cancellationToken)
        {
            try
            {
                var entity = await _business.GetByUserName(userName, cancellationToken);
                if (entity == null)
                    throw new NotFoundException("Usuário não encontrado.");

                return Ok(entity);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Atualiza os dados de um usuário
        /// </summary>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update([FromBody] UserRequestBase<UserUpDateRequest> request, CancellationToken cancellationToken)
        {
            try
            {
                if (request?.Data == null)
                    return BadRequest(new { message = "Dados inválidos enviados." });

                var entity = request.Data.ToUserEntity();
                await _business.Update(entity, cancellationToken);
                return Ok(new { message = "Usuário atualizado com sucesso." });
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Deleta um usuário pelo UserName
        /// </summary>
        [HttpDelete("{userName}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(string userName, CancellationToken cancellationToken)
        {
            try
            {
                await _business.Delete(userName, cancellationToken);
                return Ok(new { message = "Usuário deletado com sucesso." });
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Confirma o e-mail do usuário
        /// </summary>
        [HttpGet("emailConfirmation")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> EmailConfirmation([FromQuery] string email, [FromQuery] string token, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _user.FindByEmailAsync(email);
                if (user == null)
                    throw new NotFoundException("Usuário não encontrado.");

                var result = await _user.ConfirmEmailAsync(user, token);
                if (result.Succeeded)
                    return Ok(new { message = "E-mail confirmado com sucesso." });

                return BadRequest(new { message = "Não foi possível confirmar o e-mail." });
            }
            catch (Exception ex)
            {
                return HandleException(ex);
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
                    throw new NotFoundException("Usuário não encontrado.");

                var token = await _user.GeneratePasswordResetTokenAsync(user);
                var param = new Dictionary<string, string?> { { "token", token } };

                var appBaseUrl = _configuration["Frontend:AppUrl"];
                var resetLink = QueryHelpers.AddQueryString($"{appBaseUrl}/reset-password", param);

                await _emailService.SendPasswordResetLinkAsync(user, user.Email, resetLink);

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
                var user = await _user.FindByEmailAsync(request.Data.Email);
                if (user == null)
                    throw new NotFoundException("Usuário não encontrado.");

                var result = await _user.ResetPasswordAsync(user, token, request.Data.Password);
                if (result.Succeeded)
                    return Ok(new { message = "Senha redefinida com sucesso." });

                return BadRequest(new { message = "Não foi possível redefinir a senha." });
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}