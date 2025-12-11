using FluentValidation;
using StackbuldInventoryOrderManagement.Application.Order.Dto;

namespace StackbuldInventoryOrderManagement.Application.Order.Validator
{
    public class OrderItemValidator : AbstractValidator<OrderItemDto>
    {
        public OrderItemValidator()
        {
            RuleFor(x => x.ProductId)
                .NotEmpty().WithMessage("Product ID is required.");

            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be greater than 0.")
                .LessThanOrEqualTo(1000).WithMessage("Quantity cannot exceed 1000 per order.");
        }
    }
}
