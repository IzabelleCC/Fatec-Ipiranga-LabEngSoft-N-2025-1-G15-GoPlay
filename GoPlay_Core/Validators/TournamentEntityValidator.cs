using FluentValidation;
using GoPlay_Core.Entities;
using GoPlay_Core.Repository.Interfaces;

namespace GoPlay_Core.Validators
{
    public class TournamentEntityValidator : AbstractValidator<TournamentEntity>
    {
        private readonly ITournamentRepository _tournamentRepository;

        public TournamentEntityValidator(ITournamentRepository tournamentRepository)
        {
            _tournamentRepository = tournamentRepository;

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("O nome do torneio é obrigatório.")
                .MustAsync(BeUniqueTournamentNameByDateAndLocation).WithMessage("Já existe um torneio ativo com o mesmo nome, data e local.");

            RuleFor(x => x.GamesStartDate)
                .NotEmpty().WithMessage("A data de início é obrigatória.")
                .Must(BeAFutureDate).WithMessage("A data de início deve ser no futuro.");

            RuleFor(x => x.GamesEndDate)
                .NotEmpty().WithMessage("A data de término é obrigatória.")
                .GreaterThan(x => x.GamesStartDate).WithMessage("A data de término deve ser posterior à data de início.");

            RuleFor(x => x.Location)
                .NotEmpty().WithMessage("O local é obrigatório.")
                .Matches(@"^[\p{L}\p{N}\s.,'-]+$").WithMessage("O local deve conter apenas letras, números e pontuação simples.");

            RuleFor(x => x.RegistrationFee)
                .GreaterThanOrEqualTo(0).WithMessage("O valor da inscrição deve ser um número positivo ou zero.");
        }

        private bool BeAFutureDate(DateTime date)
        {
            return date > DateTime.UtcNow;
        }

        private async Task<bool> BeUniqueTournamentNameByDateAndLocation(TournamentEntity tournament, string name, CancellationToken cancellationToken)
        {
            var existingTournament = await _tournamentRepository.GetByName(name);

            if (existingTournament == null)
                return true;
                        
            if (existingTournament.IsActive
                && existingTournament.Location == tournament.Location
                && existingTournament.GamesStartDate.Date == tournament.GamesStartDate.Date)
            {
                return false;
            }

            return true;
        }
    }
}
