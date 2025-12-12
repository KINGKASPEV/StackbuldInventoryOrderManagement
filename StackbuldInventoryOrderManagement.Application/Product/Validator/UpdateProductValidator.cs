using FluentValidation;
using StackbuldInventoryOrderManagement.Application.Product.Dto;

namespace StackbuldInventoryOrderManagement.Application.Product.Validator
{
    public class UpdateProductValidator : AbstractValidator<UpdateProductDto>
    {
        public UpdateProductValidator()
        {
            RuleFor(x => x.Name)
                .MaximumLength(200).WithMessage("Product name must not exceed 200 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters.");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be greater than 0.");

            RuleFor(x => x.StockQuantity)
                .GreaterThanOrEqualTo(0).WithMessage("Stock quantity cannot be negative.");

            RuleFor(x => x.Sku)
                .MaximumLength(50).WithMessage("SKU must not exceed 50 characters.")
                .When(x => !string.IsNullOrEmpty(x.Sku));
        }
    }
}
