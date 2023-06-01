using GameDock.Server.Domain.Enums;
using GameDock.Server.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace GameDock.Server.Infrastructure.Database.Configurations;

public class FleetInfoEntityConfiguration : IEntityTypeConfiguration<FleetInfoEntity>
{
    public void Configure(EntityTypeBuilder<FleetInfoEntity> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.BuildId).IsRequired();
        builder.HasOne<BuildInfoEntity>()
            .WithMany()
            .HasForeignKey(x => x.BuildId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.Runtime).IsRequired()
            .HasAnnotation("MinLength", 1);
        
        builder.Property(x => x.ImageId)
            .IsRequired(false)
            .HasAnnotation("MinLength", 1);

        builder.Property(x => x.LaunchParameters)
            .IsRequired(false)
            .HasAnnotation("MinLength", 1);

        builder.Property(x => x.Ports)
            .HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<int[]>(v))
            .IsRequired()
            .HasAnnotation("MinLength", 1)
            .Metadata.SetValueComparer(new ValueComparer<int[]>(
                (c1, c2) => c1.SequenceEqual(c2),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => (int[])c.Clone()));

        builder.Property(x => x.Variables)
            .HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<Dictionary<string, string>>(v))
            .IsRequired(false)
            .Metadata.SetValueComparer(new ValueComparer<IDictionary<string, string>>(
                (d1, d2) => d1.OrderBy(kv => kv.Key).SequenceEqual(d2.OrderBy(kv => kv.Key)),
                d => d.Aggregate(0, (a, kv) => HashCode.Combine(a, kv.Key.GetHashCode(), kv.Value.GetHashCode())),
                d => new Dictionary<string, string>(d)));

        builder.Property(x => x.Status)
            .IsRequired()
            .HasConversion(
                v => v.ToString(),
                v => (FleetStatus)Enum.Parse(typeof(FleetStatus), v)
            );

        builder.Property(x => x.CreatedAt)
            .ValueGeneratedOnAdd()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(x => x.UpdatedAt)
            .ValueGeneratedOnAddOrUpdate()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
    }
}