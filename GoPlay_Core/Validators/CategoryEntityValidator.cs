using FluentValidation;
using GoPlay_Core.Entities;

namespace GoPlay_Core.Validators
{
    public class CategoryEntityValidator : AbstractValidator<CategoryEntity>
    {
        public CategoryEntityValidator()
        {
            RuleFor(x => x.CategoryType)
                .NotEmpty().WithMessage("O tipo da categoria é obrigatório.");

            RuleFor(x => x.PlayerLimit)
                .Must(BePositiveInteger).WithMessage("A quantidade de jogadores deve ser um número inteiro positivo.");
        }

        private bool BePositiveInteger(int value)
        {
            return value > 0;
        }
    }
}
