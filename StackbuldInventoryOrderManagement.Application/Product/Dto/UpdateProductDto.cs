namespace StackbuldInventoryOrderManagement.Application.Product.Dto
{
    public class UpdateProductDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string? ImageUrl { get; set; }
        public string? Sku { get; set; }
        public bool IsActive { get; set; }
    }
}
