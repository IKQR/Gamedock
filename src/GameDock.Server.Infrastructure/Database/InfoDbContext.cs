using GameDock.Server.Infrastructure.Entities;
using GameDock.Server.Infrastructure.Entities.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GameDock.Server.Infrastructure.Database;

public class InfoDbContext : IdentityDbContext<AppUser, AppRole, string>
{
    public DbSet<FleetInfoEntity> FleetInfos { get; set; }
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