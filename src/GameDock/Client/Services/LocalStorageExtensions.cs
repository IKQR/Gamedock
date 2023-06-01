using System.Threading;
using System.Threading.Tasks;
using Blazored.LocalStorage;

namespace GameDock.Client.Services;

public static class LocalStorageExtensions
{
    public const string AuthTokenKey = "XToken";

    public static ValueTask<string> GetTokenAsync(this ILocalStorageService localStorage,
        CancellationToken cancellationToken = default) =>
        localStorage.GetItemAsStringAsync(AuthTokenKey, cancellationToken);


    public static ValueTask SetTokenAsync(this ILocalStorageService localStorage, string token,
        CancellationToken cancellationToken = default) =>
        localStorage.SetItemAsStringAsync(AuthTokenKey, token, cancellationToken);


    public static ValueTask RemoveTokenAsync(this ILocalStorageService localStorage,
        CancellationToken cancellationToken = default) =>
        localStorage.RemoveItemAsync(AuthTokenKey, cancellationToken);
}