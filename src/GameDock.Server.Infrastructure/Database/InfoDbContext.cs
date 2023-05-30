using GameDock.Server.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameDock.Server.Infrastructure.Database;

public class InfoDbContext : DbContext
{
    public DbSet<BuildInfoEntity> BuildInfos { get; set; }

    public InfoDbContext(DbContextOptions<InfoDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(InfoDbContext).Assembly);
    }
}