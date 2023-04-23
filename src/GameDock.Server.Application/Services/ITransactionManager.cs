namespace GameDock.Server.Application.Services;

public interface ITransactionManager
{
    Task InTransactionAsync(Func<CancellationToken, Task> exec, CancellationToken cancellationToken = default);
    Task<T> InTransactionAsync<T>(Func<CancellationToken, Task<T>> exec, CancellationToken cancellationToken = default);
}