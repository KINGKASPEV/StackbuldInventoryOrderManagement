using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StackbuldInventoryOrderManagement.Domain.Audit;

namespace StackbuldInventoryOrderManagement.Persistence.Audit
{
    public class AuditTrailEntityTypeConfiguration : IEntityTypeConfiguration<AuditTrail>
    {
        public void Configure(EntityTypeBuilder<AuditTrail> builder)
        {
            builder.ToTable("AuditTrail");
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            builder.Property(x => x.LoggedInUser).HasMaxLength(450);
            builder.Property(x => x.ActionName).HasMaxLength(450);
            builder.Property(x => x.Module).HasMaxLength(150);
            builder.Property(x => x.ActionDescription).HasMaxLength(100);
            builder.Property(x => x.ActionTime).HasMaxLength(30);
            builder.Property(x => x.DateCreated).HasMaxLength(30);
            builder.Property(x => x.Origin).HasMaxLength(50);
            builder.Property(x => x.CreatedBy).HasMaxLength(450);
        }
    }
}
