using GoPlay_Core.Business.Interfaces;
using GoPlay_Core.Entities;
using GoPlay_Core.Enum;
using GoPlay_Core.Repository.Interfaces;
using GoPlay_Core.Services.Gerencianet;
using GoPlay_Core.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Globalization;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;


namespace GoPlay_Core.Business
{
    public class PixBusiness : IPixBusiness
    {
        private readonly ICategoryPlayerBusiness _categoryPlayerBusiness;
        private readonly ICategoryPlayerRepository _categoryPlayerRepository;
        private readonly IUserRepository _userRepository;
        private readonly ITournamentRepository _turnamentRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IPixService _pixService;
        private readonly IConfiguration _configuration;
        private readonly GerencianetAuthenticator _authenticator;

        public PixBusiness(ICategoryPlayerBusiness categoryPlayerBusiness, ICategoryPlayerRepository categoryPlayerRepository, IPixService pixService, IConfiguration configuration, IUserRepository userRepository, ITournamentRepository turnamentRepository, ICategoryRepository categoryRepository, GerencianetAuthenticator authenticator)
        {
            _categoryPlayerBusiness = categoryPlayerBusiness;
            _categoryPlayerRepository = categoryPlayerRepository;
            _pixService = pixService;
            _configuration = configuration;
            _userRepository = userRepository;
            _turnamentRepository = turnamentRepository;
            _categoryRepository = categoryRepository;
            _authenticator = authenticator;
        }

        public async Task<string> GeneratePixForRegistration(int registrationId, string userId, CancellationToken cancellationToken)
        {
            var entity = await _categoryPlayerBusiness.GetByIdAsync(registrationId, cancellationToken);
            if (entity == null)
                throw new Exception("Inscrição não encontrada.");

            if (entity.FirstUserId != userId && entity.SecondUserId != userId)
                throw new Exception("Usuário não pertence a esta inscrição.");

            var pixRequestData = await CreatePixRequestData(registrationId, userId, cancellationToken);

            // Geração e persistência do TxId
            var guid = Guid.NewGuid().ToString("N");
            string txid = $"goplay{registrationId}{guid.Substring(0, 20)}";

            if (entity.FirstUserId == userId) entity.FirstUserTxId = txid;
            else entity.SecondUserTxId = txid;

            var response = await _pixService.GeneratePixAsync(pixRequestData, txid);

            if (response.Contains("error"))
            {
                throw new Exception($"Erro ao gerar cobrança Pix: {response}");
            }
            entity.RegisterStatus = RegisterStatus.PagamentoPendente;

            await _categoryPlayerBusiness.UpdatePlayersAsync(entity, cancellationToken);

            return response;
        }

        public async Task ConfirmPaymentByTxIdAsync(string txid, string userId, CancellationToken cancellationToken)
        {
            var registration = await _categoryPlayerRepository.GetByTxIdAsync(txid);

            if (registration == null)
                throw new KeyNotFoundException("Inscrição não encontrada com o TxId informado.");

            if (registration.FirstUserId == userId)
            {
                registration.FirstUserPaymentConfirmed = true;
            }
            else if (registration.SecondUserId == userId)
            {
                registration.SecondUserPaymentConfirmed = true;
            }
            else
            {
                throw new UnauthorizedAccessException("Usuário não pertence a esta inscrição.");
            }

            await _categoryPlayerBusiness.UpdatePlayersAsync(registration, cancellationToken);
        }

        public async Task<PixRequestData> CreatePixRequestData(int registrationId, string userId, CancellationToken cancellationToken)
        {
            var categoryPlayer = await _categoryPlayerRepository.GetByIdAsync(registrationId);
            if (categoryPlayer == null)
                throw new Exception("Inscrição não encontrada.");

            if (categoryPlayer.FirstUserId != userId && categoryPlayer.SecondUserId != userId)
                throw new Exception("Usuário não pertence a esta inscrição.");

            var category = await _categoryRepository.GetById(categoryPlayer.CategoryId);
            var user = await _userRepository.GetById(userId);
            var tournament = await _turnamentRepository.GetById(category.TournamentId);

            string nome = user.Name.ToUpper();
            string cpf = user.CpfCnpj;
            string valor = tournament.RegistrationFee.ToString("F2", CultureInfo.InvariantCulture);


            var pixKey = _configuration["Gerencianet:PixKey"] ?? throw new Exception("PixKey não configurado.");

            return new PixRequestData
            {
                calendario = new Calendario { expiracao = 3600 },
                devedor = new Devedor { nome = nome, cpf = cpf },
                valor = new Valor { original = valor },
                chave = pixKey,
                solicitacaoPagador = "Pagamento da inscrição GoPlay"
            };
        }

        public async Task RegisterWebhookAsync(string chavePix, string webhookUrl, CancellationToken cancellationToken)
        {
            var baseUrl = _configuration["Gerencianet:BaseUrl"];
            var (client, token) = await _authenticator.AuthenticateAsync();

            var request = new HttpRequestMessage(HttpMethod.Put, $"{baseUrl}/v2/webhook/{chavePix}")
            {
                Content = new StringContent(JsonSerializer.Serialize(new { webhookUrl }), Encoding.UTF8, "application/json")
            };

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.SendAsync(request, cancellationToken);
            var body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Erro ao registrar webhook: {body}");
        }

    }
}

