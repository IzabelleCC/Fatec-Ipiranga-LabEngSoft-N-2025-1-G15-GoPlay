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
                return Ok(new { message = "Usuário criado com sucesso." });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

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
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Update([FromBody] UserRequestBase<UserUpDateRequest> request, CancellationToken cancellationToken)
        {
            try
            {
                var entity = request.Data.ToUserEntity();
                await _business.Update(entity, cancellationToken);
                return Ok(new { message = "Usuário atualizado com sucesso." });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

        [HttpDelete("{userName}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Delete(string userName, CancellationToken cancellationToken)
        {
            try
            {
                await _business.Delete(userName, cancellationToken);
                return Ok(new { message = "Usuário deletado com sucesso." });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

        [HttpGet("emailConfirmation")]
        [AllowAnonymous]
        public async Task<IActionResult> EmailConfirmation([FromQuery] string email, [FromQuery] string token, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _user.FindByEmailAsync(email);
                if (user == null)
                {
                    return NotFound(new { message = "Usuário não encontrado." });
                }
                var result = await _user.ConfirmEmailAsync(user, token);
                if (result.Succeeded)
                {
                    return Ok(new { message = "E-mail confirmado com sucesso." });
                }
                return BadRequest(new { message = "Não foi possível confirmar o e-mail." });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

        [HttpPost("SendPasswordResetLink")]
        [AllowAnonymous]
        public async Task<IActionResult> SendPasswordResetLink([FromBody] UserRequestBase<PasswordResetLinkRequest> request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _user.FindByEmailAsync(request.Data.Email);
                if (user == null)
                {
                    return NotFound(new { message = "Usuário não encontrado." });
                }
                var token = await _user.GeneratePasswordResetTokenAsync(user);
                var param = new Dictionary<string, string?> { { "token", token } };

                var baseUrl = _configuration["Backend:BaseUrl"];
                var resetLink = QueryHelpers.AddQueryString($"{baseUrl}/api/UserManager/ResetPassword", param);
                await _emailService.SendPasswordResetLinkAsync(user, user.Email, resetLink);

                return Ok(new { message = "Link de redefinição enviado com sucesso." });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

        [HttpPost("ResetPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromQuery] string token, [FromBody] UserRequestBase<PasswordResetRequest> request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _user.FindByEmailAsync(request.Data.Email);
                if (user == null)
                {
                    return NotFound(new { message = "Usuário não encontrado." });
                }
                var result = await _user.ResetPasswordAsync(user, token, request.Data.Password);
                if (result.Succeeded)
                {
                    return Ok(new { message = "Senha redefinida com sucesso." });
                }
                return BadRequest(new { message = "Não foi possível redefinir a senha." });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }
    }
}
