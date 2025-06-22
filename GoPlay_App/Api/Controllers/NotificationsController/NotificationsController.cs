using GoPlay_App.Api.Controllers.NotificationsController.Models;
using GoPlay_Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

[ApiController]
[Route("api/[controller]")]
public class NotificationsController : ControllerBase
{
    private readonly FirebaseNotificationService _firebaseService;
    private readonly ILogger<NotificationsController> _logger;

    public NotificationsController(FirebaseNotificationService firebaseService, ILogger<NotificationsController> logger)
    {
        _firebaseService = firebaseService;
        _logger = logger;
    }

    [HttpPost("send")]
    public async Task<IActionResult> Send(string token, string title, string body)
    {
        _logger.LogInformation("Tentando enviar notificação para token: {Token}", token);

        await _firebaseService.SendNotificationAsync(token, title, body);

        _logger.LogInformation("Notificação enviada com sucesso para o token: {Token}", token);

        return Ok("Notificação enviada com sucesso");
    }

    [HttpPost("register")]
    public IActionResult RegisterToken([FromBody] RegisterTokenRequest request)
    {
        _logger.LogInformation("Token registrado para o usuário {UserId}: {Token}", request.UserId, request.Token);
        if (string.IsNullOrWhiteSpace(request.Token) || string.IsNullOrWhiteSpace(request.UserId))
        {
            _logger.LogWarning("Tentativa de registrar token inválido. Token: {Token}, UserId: {UserId}", request.Token, request.UserId);
            return BadRequest("Token e UserId são obrigatórios.");
        }

        _logger.LogInformation("Token registrado para o usuário {UserId}: {Token}", request.UserId, request.Token);

        // TODO: Salvar no repositório (banco)

        return Ok("Token registrado com sucesso.");
    }
}