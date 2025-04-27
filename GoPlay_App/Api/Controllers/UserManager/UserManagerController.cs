using GoPlay_App.Api.Controllers.UserController.Models;
using GoPlay_Core.Business.Interfaces;
using GoPlay_Core.Entities;
using GoPlay_Core.Exceptions;
using GoPlay_Core.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GoPlay_App.Api.Controllers.UserController
{
    /// <summary>
    /// Controller para gerenciar usuários
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UserManagerController : ControllerBase
    {
        private readonly IUserBusiness<UserEntity, UserResponse> _business;
        private readonly IEmailService _emailService;
        private readonly UserManager<UserEntity> _user;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Construtor do UserManagerController
        /// </summary>
        public UserManagerController(
            IUserBusiness<UserEntity, UserResponse> business,
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
        public async Task<IActionResult> Add([FromBody] UserRequestBase<UserCreateRequest> request, CancellationToken cancellationToken)
        {
            try
            {
                if (request?.Data == null)
                    return BadRequest(new { message = "Dados enviados inválidos ." });

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
        /// Busca todos os jogadores
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotFoundException"></exception>
        [HttpGet("GetAllPlayers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllPlayers(CancellationToken cancellationToken)
        {
            try
            {
                var entity = await _business.GetAllPlayers(cancellationToken);
                if (entity == null)
                    throw new NotFoundException("Nenhum usuário encontrado.");

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
                    return BadRequest(new { message = "Dados enviados inválidos. " });

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
    }
}