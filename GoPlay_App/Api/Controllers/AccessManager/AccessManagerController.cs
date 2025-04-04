using GoPlay_App.Api.Controllers.AccessManager.Models;
using GoPlay_App.Api.Controllers.UserController.Models;
using GoPlay_Core.Exceptions;
using GoPlay_Core.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

        /// <summary>
        /// Construtor do controlador de gerenciamento de acesso
        /// </summary>
        /// <param name="userService"></param>
        /// <param name="tokenService"></param>
        public AccessManagerController(IUserService userService, ITokenService tokenService)
        {
            _userService = userService;
            _tokenService = tokenService;
        }

        /// <summary>
        /// Permite acesso a usuários autenticados
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> ValidateUser()
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (_tokenService.ValidateToken(token))
            {
                return Ok(new { message = "Acesso permitido." });
            }
            else
            {
                return Unauthorized(new { message = "Token inválido." });
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
        public async Task<IActionResult> Login(UserRequestBase<UserLoginRequest> request, CancellationToken cancellationToken)
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
    }
}


