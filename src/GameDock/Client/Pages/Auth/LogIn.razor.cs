using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using GameDock.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace GameDock.Client.Pages.Auth;

public partial class LogIn : ComponentBase
{
    [Inject] AuthService AuthService { get; init; }
    [Inject] NavigationManager Navigation { get; init; }
    [Inject] AuthenticationStateProvider AuthenticationStateProvider { get; init; }

    private string _error = null!;
    private readonly LoginModel _loginModel = new();

    private async Task HandleLogin()
    {
        var result = await AuthService.SignInAsync(_loginModel.Login, _loginModel.Password);

        if (result.Succeeded)
        {
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            if (authState?.User?.Identity is { IsAuthenticated: true })
            {
                Navigation.NavigateTo("/builds");
            }
        }
        else
        {
            _error = result.Error;
        }
    }

    public record LoginModel
    {
        [Required] public string Login { get; set; }

        [Required] public string Password { get; set; }
    }
}