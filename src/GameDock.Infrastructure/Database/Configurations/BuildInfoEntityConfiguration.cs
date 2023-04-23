using GameDock.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameDock.Infrastructure.Database.Configurations;

public class BuildInfoEntityConfiguration : IEntityTypeConfiguration<BuildInfoEntity>
{
    public void Configure(EntityTypeBuilder<BuildInfoEntity> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => new { x.Name, x.Version }).IsUnique();

        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        
        builder.Property(x => x.Name).IsRequired();
        builder.Property(x => x.Version).IsRequired();
        
        builder.Property(x => x.UpdatedAt)
            .ValueGeneratedOnAddOrUpdate();
        
        builder.Property(x => x.CreatedAt)
            .ValueGeneratedOnAdd()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
        
        builder.Property(x => x.Status)
            .HasColumnType("nvarchar(MAX)")
            .IsRequired();
    }
}