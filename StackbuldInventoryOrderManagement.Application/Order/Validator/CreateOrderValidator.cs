using FluentValidation;
using StackbuldInventoryOrderManagement.Application.Order.Dto;

namespace StackbuldInventoryOrderManagement.Application.Order.Validator
{
    public class CreateOrderValidator : AbstractValidator<CreateOrderDto>
    {
        public CreateOrderValidator()
        {
            RuleFor(x => x.OrderItems)
                .NotEmpty().WithMessage("Order must contain at least one item.")
                .Must(items => items != null && items.Count > 0)
                .WithMessage("Order must contain at least one item.");

            RuleForEach(x => x.OrderItems)
                .SetValidator(new OrderItemValidator());

            RuleFor(x => x.ShippingAddress)
                .MaximumLength(500).WithMessage("Shipping address must not exceed 500 characters.")
                .When(x => !string.IsNullOrEmpty(x.ShippingAddress));

            RuleFor(x => x.Notes)
                .MaximumLength(1000).WithMessage("Notes must not exceed 1000 characters.")
                .When(x => !string.IsNullOrEmpty(x.Notes));
        }
    }
}
