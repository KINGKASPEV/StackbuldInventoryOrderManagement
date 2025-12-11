namespace StackbuldInventoryOrderManagement.Domain.Common
{
    public abstract class Entity
    {
        public Guid Id { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now.ToUniversalTime();
        public DateTime? DateModified { get; set; }
    }
}
