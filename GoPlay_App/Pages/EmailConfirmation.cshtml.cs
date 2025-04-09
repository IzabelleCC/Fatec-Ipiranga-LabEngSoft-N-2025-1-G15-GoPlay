using GoPlay_Web.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;

public class EmailConfirmationModel : PageModel
{
    private readonly UserManagerApi _userManagerApi;

    public EmailConfirmationModel(UserManagerApi userManagerApi)
    {
        _userManagerApi = userManagerApi;
    }

    [BindProperty(SupportsGet = true)]
    public string? Token { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Email { get; set; }

    public bool IsSuccess { get; set; }
    public string SuccessMessage { get; set; } = "E-mail confirmado com sucesso!";
    public string ErrorMessage { get; set; } = "E-mail não confirmado.";

    public async Task OnGetAsync()
    {

        if (!string.IsNullOrWhiteSpace(Token) && !string.IsNullOrWhiteSpace(Email))
        {
            IsSuccess = await _userManagerApi.EmailConfirmation(Token, Email);

        }
    }
}
