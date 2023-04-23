using GameDock.Server.Application.Services;

namespace GameDock.Server.Infrastructure.Database;

public class TransactionManager : ITransactionManager
{
    private readonly InfoDbContext _context;

    public TransactionManager(InfoDbContext context)
    {
        _context = context;
    }

    public async Task InTransactionAsync(Func<CancellationToken, Task> exec, CancellationToken cancellationToken = default)
    {
        var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            await exec(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<T> InTransactionAsync<T>(Func<CancellationToken, Task<T>> exec, CancellationToken cancellationToken = default)
    {
        var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var result = await exec(cancellationToken);
            
            await transaction.CommitAsync(cancellationToken);

            return result;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}