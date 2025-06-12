using System.Text.Json;
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
        private readonly IPixBusiness _pixBusiness;
        private readonly IMatchGroupBusiness _matchGroupBusiness;

        public CategoryPlayerController(ICategoryPlayerBusiness business, IPixBusiness pixBusiness, IMatchGroupBusiness matchGroupBusiness)
        {
            _business = business;
            _pixBusiness = pixBusiness;
            _matchGroupBusiness = matchGroupBusiness;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] CategoryRegistrationRequest request, CancellationToken cancellationToken)
        {
            if (request == null || request.CategoryId <= 0 || string.IsNullOrEmpty(request.FirstUserId))
                return BadRequest(new { message = "Dados enviados inválidos." });

            var result = await _business.RegisterUserToCategory(request.CategoryId, request.FirstUserId, request.SecondUserId, cancellationToken);

            return Ok(new
            {
                message = "Usuário inscrito com sucesso na categoria.",
                categoryPlayerId = result.Id
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
            => Ok(await _business.GetAllAsync(cancellationToken));

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            var result = await _business.GetByIdAsync(id, cancellationToken);
            return result == null ? NotFound(new { message = "Inscrição não encontrada." }) : Ok(result);
        }

        [HttpGet("ByCategory/{categoryId}")]
        public async Task<IActionResult> GetByCategory(int categoryId, CancellationToken cancellationToken)
            => Ok(await _business.GetByCategoryIdAsync(categoryId, cancellationToken));

        [HttpGet("ByUser/{userId}")]
        public async Task<IActionResult> GetByUser(string userId, CancellationToken cancellationToken)
            => Ok(await _business.GetByUserIdAsync(userId, cancellationToken));

        [HttpGet("GetByUserIdReturnsFullInfo/{userId}")]
        public async Task<IActionResult> GetByUserIdReturnsFullInfo(string userId, CancellationToken cancellationToken)
        {
            var result = await _business.GetByUserIdAndReturnsFullInfoAsync(userId, cancellationToken);
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] CategoryPlayerUpdateRequest request, CancellationToken cancellationToken)
        {
            var entity = await _business.GetByIdAsync(request.Id, cancellationToken);
            if (entity == null)
                return NotFound(new { message = "Inscrição não encontrada." });

            request.ToEntity(entity);
            await _business.UpdatePlayersAsync(entity, cancellationToken);
            return Ok(new { message = "Inscrição atualizada com sucesso." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            await _business.DeleteAsync(id, cancellationToken);
            return Ok(new { message = "Inscrição excluída com sucesso." });
        }

        [HttpPost("GeneratePayment")]
        public async Task<IActionResult> GeneratePayment([FromQuery] int registrationId, [FromQuery] string userId, CancellationToken cancellationToken)
        {
            var entity = await _business.GetByIdAsync(registrationId, cancellationToken);
            if (entity == null)
                return NotFound(new { message = "Inscrição não encontrada." });

            if (entity.FirstUserId != userId && entity.SecondUserId != userId)
                return BadRequest(new { message = "Usuário não pertence a esta inscrição." });

            var pixResponse = await _pixBusiness.GeneratePixForRegistration(registrationId, userId, cancellationToken);
            return Ok(pixResponse);
        }

        [HttpPost("Webhook")]
        public async Task<IActionResult> WebhookPix([FromBody] JsonElement payload, CancellationToken cancellationToken)
        {
            Console.WriteLine("Webhook Pix recebido.");
            Console.WriteLine($"Payload: {payload}");
            try
            {
                Console.WriteLine("Processando payload do Webhook...");
                var pixArray = payload.GetProperty("pix");

                if (pixArray.GetArrayLength() == 0)
                    return BadRequest(new { message = "Payload não contém dados de pagamento." });

                var pixItem = pixArray[0];

                var txid = pixItem.GetProperty("txid").GetString();
                Console.WriteLine($"TxId recebido: {txid}");

                if (string.IsNullOrWhiteSpace(txid))
                    return BadRequest(new { message = "txid ausente no payload." });

                await _pixBusiness.ConfirmPaymentByTxIdAsync(txid, cancellationToken);

                return Ok(new { message = "Pagamento confirmado com sucesso." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("Webhook")]
        public IActionResult WebhookValidation()
        {
            Console.WriteLine("Webhook de validação recebido.");
            return Ok("Webhook de validação respondido com sucesso.");
        }

        [HttpGet("GetMatchGroupByCategoryId/{categoryId}")]
        public async Task<IActionResult> GetMatchGroupByCategoryId(int categoryId, CancellationToken cancellationToken)
        {
            var result = await _matchGroupBusiness.GetMatchGroupByCategoryId(categoryId, cancellationToken);
            if (result == null)
                return NotFound(new { message = "Nenhum grupo de partidas encontrado para esta categoria." });
            return Ok(result);
        }


    }
}
