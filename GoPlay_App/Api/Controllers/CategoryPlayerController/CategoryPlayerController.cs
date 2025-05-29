using GoPlay_App.Api.Controllers.CategoryPlayerController.Models;
using GoPlay_Core.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GoPlay_App.Api.Controllers.CategoryPlayerController
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryPlayerController : ControllerBase
    {
        private readonly ICategoryPlayerBusiness _business;

        public CategoryPlayerController(ICategoryPlayerBusiness business)
        {
            _business = business;
        }

        /// <summary>
        /// Inscreve um jogador (ou dupla) em uma categoria.
        /// </summary>
        [HttpPost("Register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Register([FromBody] CategoryRegistrationRequest request, CancellationToken cancellationToken)
        {
            try
            {
                if (request == null || request.CategoryId <= 0 || string.IsNullOrEmpty(request.FirstUserId))
                    return BadRequest(new { message = "Dados enviados inválidos." });

                await _business.RegisterUserToCategory(request.CategoryId, request.FirstUserId, request.SecondUserId, cancellationToken);

                return Ok(new { message = "Usuário inscrito com sucesso na categoria." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Retorna todas as inscrições.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var result = await _business.GetAllAsync(cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Retorna uma inscrição por ID.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            var result = await _business.GetByIdAsync(id, cancellationToken);
            return result == null
                ? NotFound(new { message = "Inscrição não encontrada." })
                : Ok(result);
        }

        /// <summary>
        /// Retorna todas as inscrições por categoria.
        /// </summary>
        [HttpGet("ByCategory/{categoryId}")]
        public async Task<IActionResult> GetByCategory(int categoryId, CancellationToken cancellationToken)
        {
            var result = await _business.GetByCategoryIdAsync(categoryId, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Retorna todas as inscrições por usuário (como jogador 1 ou 2).
        /// </summary>
        [HttpGet("ByUser/{userId}")]
        public async Task<IActionResult> GetByUser(string userId, CancellationToken cancellationToken)
        {
            var result = await _business.GetByUserIdAsync(userId, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Atualiza os jogadores de uma inscrição existente.
        /// </summary>
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] CategoryPlayerUpdateRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var entity = await _business.GetByIdAsync(request.Id, cancellationToken);
                if (entity == null)
                    return NotFound(new { message = "Inscrição não encontrada." });

                request.ToEntity(entity);

                await _business.UpdatePlayersAsync(entity, cancellationToken);
                return Ok(new { message = "Inscrição atualizada com sucesso." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Exclui uma inscrição por ID.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            try
            {
                await _business.DeleteAsync(id, cancellationToken);
                return Ok(new { message = "Inscrição excluída com sucesso." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
