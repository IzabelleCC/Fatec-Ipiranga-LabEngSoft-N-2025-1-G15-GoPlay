using GoPlay_App.Api.Controllers.AccessManager.Models;
using GoPlay_App.Api.Controllers.UserController.Models;
using GoPlay_Core.Business.Interfaces;
using GoPlay_Core.Entities;
using GoPlay_Core.Repository.Interfaces;
using GoPlay_Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace GoPlay_App.Api.Controllers.UserController
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserManagerController : ControllerBase
    {
        private readonly IUserBusiness<UserEntity> _business;
        private readonly EmailService _emailService;
        private readonly UserManager<UserEntity> _user;
        private readonly IConfiguration _configuration;

        public UserManagerController(IUserBusiness<UserEntity> business, EmailService emailService, UserManager<UserEntity> user, IConfiguration configuration)
        {
            _business = business ?? throw new ArgumentNullException(nameof(business));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _user = user ?? throw new ArgumentNullException(nameof(user));
            _configuration = configuration;
        }

        /// <summary>
        /// Adiciona um usuário
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [AllowAnonymous]
        public async Task<IActionResult> Add([FromBody] UserRequestBase<UserCreateRequest> request, CancellationToken cancellationToken)
        {
            try
            {
                var entity = request.Data.ToUserEntity();

                await _business.Add(entity, cancellationToken);

                await _emailService.SendEmailRegisterAsync(entity);

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Busca um usuário por UserName
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet("GetByUserName/{userName}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByUserName(string userName, CancellationToken cancellationToken)
        {
            try
            {
                var entity = await _business.GetByUserName(userName, cancellationToken);
                return Ok(entity);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Atualiza um usuário
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Update([FromBody] UserRequestBase<UserUpDateRequest> request, CancellationToken cancellationToken)
        {
            try
            {
                var entity = request.Data.ToUserEntity();
                await _business.Update(entity, cancellationToken);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Deleta um usuário
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpDelete("{userName}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Delete(string userName, CancellationToken cancellationToken)
        {
            try
            {
                await _business.Delete(userName, cancellationToken);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Confirmação de e-mail
        /// </summary>
        /// <param name="email"></param>
        /// <param name="token"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet("emailConfirmation")]
        [AllowAnonymous]
        public async Task<IActionResult> EmailConfirmation([FromQuery] string email, [FromQuery] string token, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _user.FindByEmailAsync(email);
                if (user == null)
                {
                    return NotFound();
                }
                var result = await _user.ConfirmEmailAsync(user, token);
                if (result.Succeeded)
                {
                    return Ok();
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Envia um link para redefinição de senha
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost("SendPasswordResetLink")]
        [AllowAnonymous]
        public async Task<IActionResult> SendPasswordResetLink([FromBody] UserRequestBase<PasswordResetLinkRequest> request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _user.FindByEmailAsync(request.Data.Email);
                if (user == null)
                {
                    return NotFound();
                }
                var token = await _user.GeneratePasswordResetTokenAsync(user);
                var param = new Dictionary<string, string?>
                {
                    {"token", token }
                };

                var baseUrl = _configuration["Backend:BaseUrl"];
                var resetLink = QueryHelpers.AddQueryString($"{baseUrl}/api/UserManager/ResetPassword", param);
                await _emailService.SendPasswordResetLinkAsync(user, user.Email, resetLink);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Redefine a senha do usuário
        /// </summary>
        /// <param name="token"></param>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost("ResetPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromQuery] string token, [FromBody] UserRequestBase<PasswordResetRequest> request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _user.FindByEmailAsync(request.Data.Email);
                if (user == null)
                {
                    return NotFound();
                }
                var result = await _user.ResetPasswordAsync(user, token, request.Data.Password);
                if (result.Succeeded)
                {
                    return Ok();
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
