using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using GameDock.Shared;
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

    // Styles
    private int _progressPercentage = 0;

    // Models
    private ElementReference _fileInput;
    private BuildMetadata _buildMetadata = new();

    private async Task HandleValidSubmit()
    {
        var requestUri = new Uri(Http.BaseAddress!, "/api/build/upload").ToString();

        await JS.InvokeVoidAsync("tusUpload", _fileInput, requestUri, _buildMetadata,
            DotNetObjectReference.Create(this));
    }

    [JSInvokable]
    public async Task OnSuccess()
    {
        await JS.InvokeVoidAsync("console.log", "success");
    }

    [JSInvokable]
    public async Task OnProgress(double bytesUploaded, double bytesTotal)
    {
        _progressPercentage = (int)(bytesUploaded / bytesTotal * 100);
        StateHasChanged();
    }

    [JSInvokable]
    public async Task OnError(string error)
    {
        await JS.InvokeVoidAsync("console.log", "error");
    }
}