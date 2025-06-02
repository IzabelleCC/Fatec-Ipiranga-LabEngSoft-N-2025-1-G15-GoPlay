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
        private readonly IConfiguration _configuration;
        private readonly IPixBusiness _pixBusiness;

        public CategoryPlayerController(ICategoryPlayerBusiness business, IConfiguration configuration, IPixBusiness pixBusiness)
        {
            _business = business;
            _configuration = configuration;
            _pixBusiness = pixBusiness;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] CategoryRegistrationRequest request, CancellationToken cancellationToken)
        {
            if (request == null || request.CategoryId <= 0 || string.IsNullOrEmpty(request.FirstUserId))
                return BadRequest(new { message = "Dados enviados inválidos." });

            await _business.RegisterUserToCategory(request.CategoryId, request.FirstUserId, request.SecondUserId, cancellationToken);
            return Ok(new { message = "Usuário inscrito com sucesso na categoria." });
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
            var remoteIp = HttpContext.Connection.RemoteIpAddress?.ToString();

            Console.WriteLine($"IP remoto: {remoteIp}");

            if (remoteIp != "34.193.116.226")
                return StatusCode(403, new { message = $"IP não autorizado: {remoteIp}" });

            var hmac = HttpContext.Request.Query["hmac"].ToString();
            Console.WriteLine($"HMAC: -> {hmac}");
            if (hmac != _configuration["WEBHOOK:HMAC"])
                return StatusCode(403, new { message = "HMAC inválido." });

            try
            {
                Console.WriteLine("Processando payload do Webhook...");
                //var pixArray = payload.GetProperty("pix");
                //if (pixArray.GetArrayLength() == 0)
                //    return BadRequest(new { message = "Payload não contém dados." });

                //var pixItem = pixArray[0];
                //var txid = pixItem.GetProperty("txid").GetString();
                //var userId = pixItem.GetProperty("infoPagador").GetString();
                //var status = pixItem.TryGetProperty("status", out var st) ? st.GetString() : null;

                //if (string.IsNullOrWhiteSpace(txid) || string.IsNullOrWhiteSpace(userId))
                //    return BadRequest(new { message = "txid ou userId ausente." });

                //if (!string.Equals(status, "CONCLUIDA", StringComparison.OrdinalIgnoreCase))
                //    return Ok(new { message = $"Status '{status}' ignorado." });

                //await _pixBusiness.ConfirmPaymentByTxIdAsync(txid, userId, cancellationToken);
                return Ok(new { message = "Pagamento confirmado com sucesso." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("Webhook")]
        [HttpGet("Webhook/{*extra}")]
        public IActionResult WebhookValidation() 
        {
            Console.WriteLine("Webhook de validação recebido.");
            return Ok("Webhook de validação respondido com sucesso.");
        }

        [HttpPost("SetupWebhook")]
        public async Task<IActionResult> SetupWebhook(CancellationToken cancellationToken)
        {
            Console.WriteLine("Iniciando configuração do Webhook...");
            var chavePix = "goplay.fatec@gmail.com";
            var webhookUrl = "https://goplay-production.up.railway.app/api/CategoryPlayer/Webhook?hmac=GOPLAY2025";

            try
            {
                await _pixBusiness.RegisterWebhookAsync(chavePix, webhookUrl, cancellationToken);
                return Ok(new { message = "Webhook registrado com sucesso." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
