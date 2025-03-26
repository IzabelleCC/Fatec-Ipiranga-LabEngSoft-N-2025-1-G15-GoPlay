using GoPlay_App.Api.Controllers.AccessManager.Models;
using GoPlay_App.Api.Controllers.UserController.Models;
using GoPlay_Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoPlay_App.Api.Controllers.AccessManager
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccessManagerController : ControllerBase
    {
        private readonly UserService _service;
        private readonly TokenService _tokenService;

        public AccessManagerController(UserService service, TokenService tokenService)
        {
            _service = service;
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
                var token = await _service.Login(entity);

                return Ok(new
                {
                    message = "Login realizado com sucesso.",
                    token = token
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
                await _service.Logout();
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
