using FluentValidation;
using GoPlay_App.Api.Controllers.TournamentManager;
using GoPlay_Core.Models.Dto;
using GoPlay_Core.Repository.Interfaces;
using GoPlay_Core.Services;
using Microsoft.AspNetCore.Http;

namespace GoPlay_Core.Business
{
    public class TournamentBusiness : ITournamentBusiness<TournamentEntity>
    {
        private readonly ITournamentRepository _repository;
        private readonly IValidator<TournamentEntity> _validator;
        private readonly CloudinaryService _cloudinaryService;

        public TournamentBusiness(ITournamentRepository repository, IValidator<TournamentEntity> validator, CloudinaryService cloudinaryService)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _validator = validator ?? throw new ArgumentNullException(nameof(validator)); ;
            _cloudinaryService = cloudinaryService;
        }
        public async Task Add(TournamentEntity entity, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(entity, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            await _repository.Add(entity);
        }
        public async Task Delete(TournamentEntity entity, CancellationToken cancellationToken)
        {
            await _repository.Delete(entity);
        }
        public async Task<TournamentEntity> GetTournamentById(int tournamentId, CancellationToken cancellationToken)
        {
            var result = await _repository.GetById(tournamentId);
            if (result == null)
            {
                throw new InvalidOperationException("Torneio não encontrado.");
            }
            return result;
        }
        public async Task<List<TournamentEntity>> GetAllTournaments(CancellationToken cancellationToken)
        {
            var result = await _repository.GetAllTournaments();
            if (result == null)
            {
                throw new InvalidOperationException("Nenhum torneio encontrado.");
            }

            var tournaments = result
                .Where(t => t != null && t.IsActive == true)
                .ToList();

            return tournaments;
        }
        public async Task<List<TournamentEntity>> GetAllByTournamentName(string tournamentName, CancellationToken cancellationToken)
        {
            var result = await _repository.GetAllByName(tournamentName);

            if (result == null)
            {
                throw new InvalidOperationException("Nenhum torneio encontrado.");
            }

            var tournaments = result
                .Where(t => t != null && t.IsActive == true)
                .ToList();

            return tournaments;
        }
        public async Task<TournamentDetailsDto> GetByIdReturnDto(int tournamentId, CancellationToken cancellationToken)
        {
            var result = await _repository.GetById(tournamentId);

            if (result == null)
            {
                throw new InvalidOperationException("Nenhum torneio encontrado.");
            }

            var dto = new TournamentDetailsDto
            {
                Id = result.Id,
                Name = result.Name,
                Status = (int)result.Status,
                GamesStartDate = result.GamesStartDate,
                GamesEndDate = result.GamesEndDate,
                RegistrationDeadline = result.RegistrationDeadline,
                Categories = result.Categories.Select(category => new CategorySummaryDto
                {
                    Id = category.Id,
                    CategoryType = category.CategoryType,
                    IsDoubles = category.IsDoubles,
                    RegisterCount = category.CategoryPlayers?.Count ?? 0
                }).ToList()
            };

            return dto;
        }
        public async Task<List<TournamentEntity>> GetTournamentByAdmUserId(string id, CancellationToken cancellationToken)
        {
            var result = await _repository.GetTournamentByAdmUserId(id);
            if (result == null)
            {
                throw new InvalidOperationException("Nenhum torneio encontrado.");
            }
            var tournaments = result
                .Where(t => t != null && t.IsActive == true)
                .ToList();

            return tournaments;

        }
        public async Task Update(TournamentEntity entity, CancellationToken cancellationToken)
        {
            var existingTournament = await _repository.GetById(entity.Id);
            if (existingTournament == null)
            {
                throw new InvalidOperationException("Torneio não encontrado para atualização.");
            }

            await _repository.Update(entity);
        }

        public async Task<string?> UploadTournamentPictureAsync(int tournamentId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return null;

            var user = await _repository.GetById(tournamentId);
            if (user == null)
                return null;

            var imageUrl = await _cloudinaryService.UploadImageAsync(file);

            user.ProfilePictureUrl = imageUrl;

            await _repository.Update(user);

            return imageUrl;
        }
    }
}
