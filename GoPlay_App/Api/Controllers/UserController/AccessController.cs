using GoPlay_App.Api.Controllers.UserController.Models;
using GoPlay_UserManagementService_Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoPlay_App.Api.Controllers.UserController
{

    [Route("api/[controller]")]
    [ApiController]
    public class AccessController : ControllerBase
    {
        private readonly UserService _service;
        public AccessController(UserService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }


        /// <summary>
        /// Permite acesso a usuários autenticados
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Get()
        {
            return Ok("Acesso Permitido");
        }

        /// <summary>
        /// Realiza login de um usuário
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Login(UserRequestBase<UserLoginRequest> request, CancellationToken cancellationToken)
        {
            try
            {
                var entity = request.Data.ToLoginEntity();
                var token = await _service.Login(entity);

                return Ok(token);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
