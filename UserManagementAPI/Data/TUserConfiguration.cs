using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserManagementAPI.Data;
using UserManagementAPI.Models;

namespace UserManagementAPI.Configurations;

public class TUserConfiguration : IEntityTypeConfiguration<TUser>
{
    public void Configure(EntityTypeBuilder<TUser> builder)
    {

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
               .IsRequired();

        builder.Property(u => u.Name)
               .HasMaxLength(100)
               .IsRequired();

        builder.Property(u => u.Email)
               .HasMaxLength(150)
               .IsRequired();

        builder.Property(u => u.Role)
               .HasMaxLength(50)
               .IsRequired();

        builder.HasIndex(u => u.Email)
               .IsUnique();
    }
}