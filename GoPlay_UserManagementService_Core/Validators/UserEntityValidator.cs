using FluentValidation;
using GoPlay_UserManagementService_Core.Entities;

public class UserEntityValidator : AbstractValidator<UserEntity>
{
    public UserEntityValidator()
    {
        RuleFor(user => user.Name).NotEmpty().WithMessage("Name is required.");
        RuleFor(user => user.Email).EmailAddress().WithMessage("A valid email is required.");
        // Add other validation rules as needed
    }
}
