using GoPlay_App.Api.Controllers.TournamentManager.Models;
using GoPlay_Core.Entities;
using GoPlay_Core.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace GoPlay_App.Api.Controllers.TournamentManager
{
    [Route("api/[controller]")]
    [ApiController]
    public class TournamentManagerController : ControllerBase
    {
        private readonly ITournamentBusiness<TournamentEntity> _business;
        private readonly IConfiguration _configuration;

        public TournamentManagerController(
            ITournamentBusiness<TournamentEntity> business,
            IConfiguration configuration)
        {
            _business = business ?? throw new ArgumentNullException(nameof(business));
            _configuration = configuration;
        }

        private IActionResult HandleException(Exception ex)
        {
            if (ex is NotFoundException)
                return NotFound(new { message = ex.Message });

            return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Add([FromBody] TournamentRequestBase<TournamentCreateRequest> request, CancellationToken cancellationToken)
        {
            try
            {
                if (request?.Data == null)
                    return BadRequest(new { message = "Dados enviados inválidos." });

                var entity = request.Data.ToTournamentEntity();

                await _business.Add(entity, cancellationToken);

                return Ok(new { message = "Torneio criado com sucesso." });
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }

        }
    }
}
