using FluentValidation;
using GoPlay_Core.Entities;
using System.Text.RegularExpressions;
using CpfCnpjLibrary;

public class UserEntityValidator : AbstractValidator<UserEntity>
{
    public UserEntityValidator()
    {
        // Senha
        RuleFor(x => x.PasswordHash)
            .NotEmpty()
            .MinimumLength(6)
            .Matches("[A-Z]").WithMessage("A senha deve conter ao menos uma letra maiúscula.")
            .Matches("[a-z]").WithMessage("A senha deve conter ao menos uma letra minúscula.")
            .Matches("[0-9]").WithMessage("A senha deve conter ao menos um número.")
            .Matches("[^a-zA-Z0-9]").WithMessage("A senha deve conter ao menos um caractere especial.");

        // E-mail
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress().WithMessage("Formato de e-mail inválido.");

        // CPF ou CNPJ
        RuleFor(x => x.CpfCnpj)
            .NotEmpty()
            .Must(IsValidCpfOrCnpj).WithMessage("CPF ou CNPJ inválido.");

        // Nome completo
        RuleFor(x => x.Name)
            .NotEmpty()
            .Matches("^[A-Za-zÀ-ÿ\\s]+$").WithMessage("Nome deve conter apenas letras e espaços.");

        // Login
        RuleFor(x => x.UserName)
            .NotEmpty()
            .Matches("^[a-zA-Z0-9_]+$").WithMessage("Login não deve conter espaços ou caracteres especiais.");

        // Telefone
        RuleFor(x => x.PhoneNumber)
            .Matches(@"^\(\d{2}\) \d{5}-\d{4}$").WithMessage("Formato de telefone inválido. Use (99) 99999-9999.");

        // Data de nascimento
        RuleFor(x => x.BirthDate)
            .LessThan(DateTime.Today).WithMessage("A data de nascimento deve ser anterior à data atual.");

        // Instagram (opcional)
        RuleFor(x => x.InstagramPage)
            .Must(insta => string.IsNullOrWhiteSpace(insta) || Regex.IsMatch(insta, @"^@[\w\.]+$|^https:\/\/(www\.)?instagram\.com\/[\w\.]+\/?$"))
            .WithMessage("Instagram deve ser uma URL válida ou começar com @.");
    }

    private bool IsValidCpfOrCnpj(string input)
    {
        var onlyDigits = new string(input.Where(char.IsDigit).ToArray());

        if (onlyDigits.Length == 11)
            return Cpf.Validar(onlyDigits);

        if (onlyDigits.Length == 14)
            return Cnpj.Validar(onlyDigits);

        return false;
    }
}
