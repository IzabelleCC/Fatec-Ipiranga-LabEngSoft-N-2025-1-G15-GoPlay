using System.Threading.Tasks;
using GoPlay_App.Api.Controllers.TournamentManager.Models;
using GoPlay_Core.Business.Interfaces;
using GoPlay_Core.Entities;
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
        private readonly ITournamentBusiness<TournamentEntity> _tournamentBusiness;
        private readonly IMatchGroupBusiness _matchGroupBusiness;
        private readonly IGameMatchBusiness _gameMatchBusiness;

        /// <summary>
        /// Construtor do Controller
        /// </summary>
        /// <param name="business"></param>
        /// <param name="configuration"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public TournamentManagerController(
            ITournamentBusiness<TournamentEntity> business, IMatchGroupBusiness matchGroupBusiness, IGameMatchBusiness gameMatchBusiness)
        {
            _tournamentBusiness = business ?? throw new ArgumentNullException(nameof(business));
            _matchGroupBusiness = matchGroupBusiness;
            _gameMatchBusiness = gameMatchBusiness;
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

                await _tournamentBusiness.Add(entity, cancellationToken);

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
                var tournaments = await _tournamentBusiness.GetAllTournaments(cancellationToken);

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
                var tournament = await _tournamentBusiness.GetAllByTournamentName(tournamentName, cancellationToken);

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
                var tournament = await _tournamentBusiness.GetTournamentById(id, cancellationToken);
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
                var tournament = await _tournamentBusiness.GetTournamentByAdmUserId(id, cancellationToken);
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

                await _tournamentBusiness.Update(entity, cancellationToken);

                return Ok(new { message = "Torneio atualizado com sucesso." });
            }
            catch (Exception ex)
            {
                return HandleException(ex);

            }
        }

        /// <summary>
        /// Deleta um torneio
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            try
            {
                var tournament = await _tournamentBusiness.GetTournamentById(id, cancellationToken);

                if (tournament == null)
                    return NotFound(new { message = "Torneio não encontrado." });

                await _tournamentBusiness.Delete(tournament, cancellationToken);

                return Ok(new { message = "Torneio deletado com sucesso." });
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Gera grupos para um torneio específico
        /// </summary>
        /// <param name="tournamentId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost("GenerateGroupMatches/{tournamentId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GenerateGroupMatches(int tournamentId, CancellationToken cancellationToken)
        {
            try
            {
                var matchGroup = await _matchGroupBusiness.GenerateMatchesForTournament(tournamentId, cancellationToken);

                if (matchGroup == null)
                    return NotFound(new { message = "Nenhum grupo de partidas encontrado." });

                return Ok(new
                {
                    message = "Partidas geradas com sucesso.",
                    data = matchGroup // Tipo: TournamentMatchesResultDto
                });
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Confirma presença da dupla ou do jogador em uma partida
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost("ConfirmAttendance")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ConfirmAttendance([FromBody] ConfirmAttendanceRequest request, CancellationToken cancellationToken)
        {
            try
            {
                if (request == null)
                    return BadRequest(new { message = "Dados enviados inválidos." });

                var result = await _matchGroupBusiness.ConfirmAttendance(request.RegistrationCategoryId, request.Latitude, request.Longitude, cancellationToken);

                if (!result)
                    return NotFound(new { message = "Confirmação de presença não encontrada." });
                return Ok(new { message = "Presença confirmada com sucesso." });
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Insere os resultados dos grupos e retorna os vencedores
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost("InsertGroupResults")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> InsertGroupResults([FromBody] List<GroupResultsRequest> request, CancellationToken cancellationToken)
        {
            try
            {
                if (request == null)
                    return BadRequest(new { message = "Dados enviados inválidos." });

                var results = request.Select(r => r.ToMatchGroupEntity()).ToList();

                await _matchGroupBusiness.InsertGroupResultsAndReturnWinners(results, cancellationToken);

                return Ok(new { message = "Resultados dos grupos inseridos com sucesso." });
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }


        [HttpPost("InsertEliminationResults")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> InsertEliminationResults([FromBody] EliminationResultsRequest request, CancellationToken cancellationToken)
        {
            try
            {
                if (request == null)
                    return BadRequest(new { message = "Dados enviados inválidos." });

                var results = request.ToGameMatchEntity();

                await _gameMatchBusiness.InsertEliminationResultsAndReturnWinners(results, cancellationToken);

                return Ok(new { message = "Resultados  inseridos com sucesso." });
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("Teste/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Teste(int id)
        {
            try
            {
                await _gameMatchBusiness.GenerateEliminationMatches(id, CancellationToken.None);
                return Ok(new { message = "Teste realizado com sucesso." });
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

    }
}
