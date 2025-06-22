using GoPlay_App.Api.Controllers.NotificationsController.Models;
using GoPlay_Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

[ApiController]
[Route("api/[controller]")]
public class NotificationsController : ControllerBase
{
    private readonly ExpoNotificationService _expoService;
    private readonly ILogger<NotificationsController> _logger;

    public NotificationsController(ExpoNotificationService expoService, ILogger<NotificationsController> logger)
    {
        _expoService = expoService;
        _logger = logger;
    }

    [HttpPost("send")]
    public async Task<IActionResult> Send(string token, string title, string body)
    {
        _logger.LogInformation("Tentando enviar notificação para token: {Token}", token);

        try
        {
            await _expoService.SendNotificationAsync(token, title, body);
            _logger.LogInformation("Notificação enviada com sucesso para o token: {Token}", token);
            return Ok("Notificação enviada com sucesso");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao enviar notificação para o token: {Token}", token);
            return StatusCode(500, $"Erro ao enviar notificação: {ex.Message}");
        }
    }

    [HttpPost("register")]
    public IActionResult RegisterToken([FromBody] RegisterTokenRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Token) || string.IsNullOrWhiteSpace(request.UserId))
        {
            _logger.LogWarning("Tentativa de registrar token inválido. Token: {Token}, UserId: {UserId}", request.Token, request.UserId);
            return BadRequest("Token e UserId são obrigatórios.");
        }

        _logger.LogInformation("Token registrado para o usuário {UserId}: {Token}", request.UserId, request.Token);

        // TODO: Salvar o token no banco (implemente conforme sua arquitetura)

        return Ok("Token registrado com sucesso.");
    }
}
