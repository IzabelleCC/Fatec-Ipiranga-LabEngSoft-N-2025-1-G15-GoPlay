using FluentValidation;
using GoPlay_Core.Entities;
using System.Text.RegularExpressions;
using CpfCnpjLibrary;
using GoPlay_Core.Repository.Interfaces;

public class UserEntityValidator : AbstractValidator<UserEntity>
{
    private readonly IUserRepository _userRepository;

    public UserEntityValidator(IUserRepository userRepository)
    {
        _userRepository = userRepository;
        // Senha
        RuleFor(x => x.PasswordHash)
            .NotEmpty().WithMessage("A senha é obrigatória.")
            .MinimumLength(6).WithMessage("A senha deve ter no mínimo 6 caracteres.")
            .Matches("[A-Z]").WithMessage("A senha deve conter ao menos uma letra maiúscula.")
            .Matches("[a-z]").WithMessage("A senha deve conter ao menos uma letra minúscula.")
            .Matches("[0-9]").WithMessage("A senha deve conter ao menos um número.")
            .Matches("[^a-zA-Z0-9]").WithMessage("A senha deve conter ao menos um caractere especial.");

        // E-mail
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("O e-mail é obrigatório.")
            .EmailAddress().WithMessage("Formato de e-mail inválido.")
            .MustAsync(BeUniqueEmail).WithMessage("E-mail já está em uso.");

        // CPF ou CNPJ
        RuleFor(x => x.CpfCnpj)
            .NotEmpty().WithMessage("O CPF ou CNPJ é obrigatório.")
            .Must(IsValidCpfOrCnpj).WithMessage("CPF ou CNPJ inválido.")
            .MustAsync(BeUniqueCpfCnpj).WithMessage("CPF ou CNPJ já está em uso.");

        // Nome completo
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("O nome é obrigatório.")
            .Matches("^[A-Za-zÀ-ÿ\\s]+$").WithMessage("O nome deve conter apenas letras e espaços.");

        // Login
        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("O login é obrigatório.")
            .Matches("^[a-zA-Z0-9_]+$").WithMessage("O login não deve conter espaços ou caracteres especiais.")
            .MustAsync(BeUniqueUserName).WithMessage("Login já está em uso.");

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

    private async Task<bool> BeUniqueEmail(string email, CancellationToken cancellationToken)
    {
        var existingUser = await _userRepository.GetByEmail(email);
        return existingUser == null;
    }

    private async Task<bool> BeUniqueUserName(string userName, CancellationToken cancellationToken)
    {
        var existingUser = await _userRepository.GetByUserName(userName);
        return existingUser == null;
    }

    private async Task<bool> BeUniqueCpfCnpj(string cpfCnpj, CancellationToken cancellationToken)
    {
        var existingUser = await _userRepository.GetByCpfCnpj(cpfCnpj);
        return existingUser == null;
    }
}