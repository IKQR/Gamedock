using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using GameDock.Shared.Requests;

namespace GameDock.Client.Services;

public class AuthService
{
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorage;

    public AuthService(IHttpClientFactory httpClientFactory, ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
        _httpClient = httpClientFactory.CreateClient("public");
    }

    public async Task<AuthResult> SignInAsync(string login, string password)
    {
        var request = new LoginRequest
        {
            Login = login,
            Password = password,
        };
        var result = await _httpClient.PostAsJsonAsync("/api/auth/token", request);

        var content = await result.Content.ReadAsStringAsync();

        if (!result.IsSuccessStatusCode)
        {
            return new AuthResult
            {
                Succeeded = false,
                Error = content,
            };
        }

        await _localStorage.SetTokenAsync(content);

        return new AuthResult
        {
            Succeeded = true,
            Token = content,
        };
    }
    
    public async Task SignOutAsync()=> await _localStorage.RemoveTokenAsync();
    
}

public class AuthResult
{
    public bool Succeeded { get; set; }
    public string Token { get; set; }
    public string Error { get; set; }
}