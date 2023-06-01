using GameDock.Server.Domain.Enums;
using GameDock.Server.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace GameDock.Server.Infrastructure.Database.Configurations;

public class SessionInfoEntityConfiguration : IEntityTypeConfiguration<SessionInfoEntity>
{
    public void Configure(EntityTypeBuilder<SessionInfoEntity> builder)
    {
        builder.HasKey(x => x.ContainerId);

        builder.Property(x => x.ContainerId).IsRequired();
        builder.Property(x => x.ImageId).IsRequired(false).HasAnnotation("MinLength", 1);
        builder.Property(x => x.Ports).IsRequired().HasAnnotation("MinLength", 1).HasConversion(
            v => JsonConvert.SerializeObject(v),
            v => JsonConvert.DeserializeObject<int[]>(v)
        ).Metadata.SetValueComparer(new ValueComparer<int[]>(
            (c1, c2) => c1.SequenceEqual(c2),
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
            c => (int[])c.Clone()
        ));
        builder.Property(x => x.Status).IsRequired().HasConversion(
            v => v.ToString(),
            v => (SessionStatus)Enum.Parse(typeof(SessionStatus), v)
        );
        builder.Property(x => x.CreatedAt).IsRequired().HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(x => x.UpdatedAt).IsRequired(false).HasDefaultValueSql("CURRENT_TIMESTAMP");
    }
}