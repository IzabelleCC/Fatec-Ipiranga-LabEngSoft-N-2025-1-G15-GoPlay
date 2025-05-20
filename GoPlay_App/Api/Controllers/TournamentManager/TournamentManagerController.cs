using GoPlay_App.Api.Controllers.TournamentManager.Models;
using GoPlay_Core.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace GoPlay_App.Api.Controllers.TournamentManager
{
    /// <summary>
    /// Controller para gerenciar torneios
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class TournamentManagerController : ControllerBase
    {
        private readonly ITournamentBusiness<TournamentEntity> _business;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Construtor do Controller
        /// </summary>
        /// <param name="business"></param>
        /// <param name="configuration"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public TournamentManagerController(
            ITournamentBusiness<TournamentEntity> business,
            IConfiguration configuration)
        {
            _business = business ?? throw new ArgumentNullException(nameof(business));
            _configuration = configuration;
        }

        /// <summary>
        /// Tratamento de Exceções
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        private IActionResult HandleException(Exception ex)
        {
            if (ex is NotFoundException)
                return NotFound(new { message = ex.Message });

            return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
        }

        /// <summary>
        /// Adiciona um novo torneio
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Busca todos os torneios
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllTournaments(CancellationToken cancellationToken)
        {
            try
            {
                var tournaments = await _business.GetAllTournaments(cancellationToken);

                if (tournaments == null || tournaments.Count == 0)
                    return NotFound(new { message = "Nenhum torneio encontrado." });

                return Ok(tournaments);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Busca um torneio pelo nome
        /// </summary>
        /// <param name="tournamentName"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet("GetByTournamentName/{tournamentName}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByTournamentName(string tournamentName, CancellationToken cancellationToken)
        {
            try
            {
                var tournament = await _business.GetAllByTournamentName(tournamentName, cancellationToken);

                if (tournament == null)
                    return NotFound(new { message = "Torneio não encontrado." });
                return Ok(tournament);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Busca um torneio pelo ID
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet("GetById/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            try
            {
                var tournament = await _business.GetTournamentById(id, cancellationToken);
                if (tournament == null)
                    return NotFound(new { message = "Torneio não encontrado." });
                return Ok(tournament);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Busca um torneio pelo ID do administrador
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet("GetByAdmUserId/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTournamentByAdmUserId(string id, CancellationToken cancellationToken)
        {
            try
            {
                var tournament = await _business.GetTournamentByAdmUserId(id, cancellationToken);
                if (tournament == null)
                    return NotFound(new { message = "Torneio não encontrado." });
                return Ok(tournament);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Edita um torneio
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update([FromBody] TournamentRequestBase<TournamentUpdateRequest> request, CancellationToken cancellationToken)
        {
            try
            {
                if (request?.Data == null)
                    return BadRequest(new { message = "Dados enviados inválidos." });

                var entity = request.Data.ToTournamentEntity();

                await _business.Update(entity, cancellationToken);

                return Ok(new { message = "Torneio atualizado com sucesso." });
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            try
            {
                var tournament = await _business.GetTournamentById(id, cancellationToken);

                if (tournament == null)
                    return NotFound(new { message = "Torneio não encontrado." });

                await _business.Delete(tournament, cancellationToken);

                return Ok(new { message = "Torneio deletado com sucesso." });
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
