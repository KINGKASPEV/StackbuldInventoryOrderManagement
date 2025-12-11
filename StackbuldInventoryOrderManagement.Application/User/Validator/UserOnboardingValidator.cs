using FluentValidation;
using StackbuldInventoryOrderManagement.Application.User.Dto;
using StackbuldInventoryOrderManagement.Domain.Users;

namespace StackbuldInventoryOrderManagement.Application.User.Validator
{
    public class UserOnboardingValidator : AbstractValidator<UserOnboardingDto>
    {
        public UserOnboardingValidator()
        {
            RuleFor(x => x.UserType)
                .NotNull().WithMessage("UserType is required.")
                .Must(userType => userType == UserType.Customer)
                .WithMessage("UserType must be a Customer.");

            RuleFor(x => x.FullName)
                .NotEmpty()
                .WithMessage("FullName is required.");

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required.");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required.")
                .Length(11).WithMessage("Phone number must be 11 digits.")
                .Matches(@"^\d+$").WithMessage("Phone number must contain only digits.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long.")
                .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
                .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
                .Matches("[0-9]").WithMessage("Password must contain at least one number.")
                .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character.");
        }
    }
}
