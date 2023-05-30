using Duende.IdentityServer.EntityFramework.Entities;
using Duende.IdentityServer.EntityFramework.Extensions;
using Duende.IdentityServer.EntityFramework.Interfaces;
using Duende.IdentityServer.EntityFramework.Options;
using GameDock.Server.Infrastructure.Entities;
using GameDock.Server.Infrastructure.Entities.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace GameDock.Server.Infrastructure.Database;

public class InfoDbContext : IdentityDbContext<AppUser, AppRole, string>, IPersistedGrantDbContext
{
    private readonly IOptions<OperationalStoreOptions> _operationalStoreOptions;

    public InfoDbContext(DbContextOptions<InfoDbContext> options,
        IOptions<OperationalStoreOptions> operationalStoreOptions) : base(options)
    {
        _operationalStoreOptions = operationalStoreOptions;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(InfoDbContext).Assembly);
        modelBuilder.ConfigurePersistedGrantContext(_operationalStoreOptions.Value);
    }
    
    Task<int> IPersistedGrantDbContext.SaveChangesAsync() => base.SaveChangesAsync();

    public DbSet<Key> Keys { get; set; }
    public DbSet<PersistedGrant> PersistedGrants { get; set; }
    public DbSet<DeviceFlowCodes> DeviceFlowCodes { get; set; }

    public DbSet<BuildInfoEntity> BuildInfos { get; set; }
}