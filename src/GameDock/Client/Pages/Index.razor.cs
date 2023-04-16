using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace GameDock.Client.Pages;

public partial class Index : ComponentBase
{
    private string CurrentTime { get; set; }

    protected override async Task OnInitializedAsync()
    {
        while (true)
        {
            CurrentTime = DateTime.UtcNow.ToString("HH:mm:ss");
            StateHasChanged();
            await Task.Delay(500);
        }
    }
}