using FluentValidation;
using UserManagementAPI.Data;

namespace UserManagementAPI.Models;

public class UserValidator : AbstractValidator<TUser>
{
    public UserValidator()
    {
        RuleFor(u => u.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name must be at most 100 characters.");

        RuleFor(u => u.Email)
            .NotEmpty().WithMessage("Email is required.")
            .MaximumLength(150).WithMessage("Email must be at most 150 characters.")
            .EmailAddress().WithMessage("Email must be a valid format.");

        RuleFor(u => u.Role)
            .NotEmpty().WithMessage("Role is required.")
            .MaximumLength(50).WithMessage("Role must be at most 50 characters.");
    }
}

