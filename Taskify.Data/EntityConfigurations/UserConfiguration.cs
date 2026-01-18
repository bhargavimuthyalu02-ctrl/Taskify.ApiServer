using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Taskify.Model;

namespace Taskify.Data.EntityConfigurations
{
    internal class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.Id);
            builder.Property(u => u.UserEmail).IsRequired().HasMaxLength(200);
            builder.Property(u => u.Password).IsRequired().HasMaxLength(500);
            builder.Property(u => u.Role).IsRequired().HasMaxLength(50);
            builder.Property(u => u.CreatedAt).HasDefaultValueSql("GETUTCDATE()").ValueGeneratedOnAdd();

            builder.ToTable("Users");
        }
    }
}
