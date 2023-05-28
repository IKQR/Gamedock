using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using GameDock.Shared;
using GameDock.Shared.Dto;
using GameDock.Shared.Responses;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using TusDotNetClient;

namespace GameDock.Client.Components;

public partial class ArchiveUpload : ComponentBase
{
    // Injects
    [Inject] public IJSRuntime JS { get; set; }
    [Inject] public HttpClient Http { get; set; }
    [Inject] public NavigationManager NavigationManager { get; set; }

    // Styles
    private int _progressPercentage = 0;

    // Models
    private ElementReference _fileInput;
    private BuildMetadata _buildMetadata = new();

    private void HandleFileSelection(ChangeEventArgs e)
    {
        _buildMetadata.FileName = Path.GetFileName(e.Value?.ToString());
    }
    
    private async Task HandleValidSubmit()
    {
        var requestUri = new Uri(Http.BaseAddress!, "/api/build/upload").ToString();

        await JS.InvokeVoidAsync("tusUpload", _fileInput, requestUri, _buildMetadata,
            DotNetObjectReference.Create(this));
    }

    [JSInvokable]
    public async Task OnSuccess()
    {
        var results =
            await Http.GetFromJsonAsync<BuildInfoDto[]>(
                $"api/build/info?name={_buildMetadata.BuildName}&version={_buildMetadata.Version}");
        
        NavigationManager.NavigateTo($"/builds/{results.Single().Id}");
    }

    [JSInvokable]
    public Task OnProgress(double bytesUploaded, double bytesTotal)
    {
        _progressPercentage = (int)(bytesUploaded / bytesTotal * 100);
        StateHasChanged();
        return Task.CompletedTask;
    }

    [JSInvokable]
    public async Task OnError(string error)
    {
        await JS.InvokeVoidAsync("console.log", "error");
    }
}