using GoPlay_App.Api.Controllers.NotificationsController.Models;
using GoPlay_Core.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class NotificationsController : ControllerBase
{
    private readonly FirebaseNotificationService _firebaseService;

    public NotificationsController(FirebaseNotificationService firebaseService)
    {
        _firebaseService = firebaseService;
    }

    [HttpPost("send")]
    public async Task<IActionResult> Send(string token, string title, string body)
    {
        await _firebaseService.SendNotificationAsync(token, title, body);
        return Ok("Notificação enviada com sucesso");
    }

    [HttpPost("register")]
    public IActionResult RegisterToken([FromBody] RegisterTokenRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Token) || string.IsNullOrWhiteSpace(request.UserId))
        {
            return BadRequest("Token e UserId são obrigatórios.");
        }

        // Aqui você salvaria no banco. Exemplo fictício:
        Console.WriteLine($"Token recebido para o usuário {request.UserId}: {request.Token}");

        // TODO: Salvar no repositório (banco)

        return Ok("Token registrado com sucesso.");
    }

}
