using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.SignalR.Client;

namespace GameDock.Client.Pages.Components;

public partial class ProjectUpload : ComponentBase, IDisposable
{
    [Inject]
    public HttpClient Client { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    private HubConnection _hubConnection;
    private bool? _uploadResult = null;
    private IList<string> _messages = new List<string>();
    private string _imageName = "";

    protected override async Task OnInitializedAsync()
    {
        // _hubConnection = new HubConnectionBuilder()
        //     .WithUrl(NavigationManager.ToAbsoluteUri("/imagehub"))
        //     .Build();
        //
        // _hubConnection.On<string>("BuildProgress", (message) =>
        // {
        //     InvokeAsync(() =>
        //     {
        //         _messages.Add(message);
        //         StateHasChanged();
        //     });
        // });

        await _hubConnection.StartAsync();
    }

    private async Task HandleFileSelected(InputFileChangeEventArgs e)
    {
        await using var fileStream = e.File.OpenReadStream();

        using var memoryStream = new MemoryStream();
        await fileStream.CopyToAsync(memoryStream);

        _imageName = await _hubConnection.InvokeAsync<string>("Build", e.File.Name, memoryStream.ToArray());
    }


    // private async Task HandleFileSelected(InputFileChangeEventArgs e)
    // {
    //     var file = e.File;
    //
    //     var request = new HttpRequestMessage(HttpMethod.Post, "api/file");
    //
    //     using var content = new MultipartFormDataContent();
    //     content.Add(new StreamContent(file.OpenReadStream()), "file", file.Name);
    //     request.Content = content;
    //
    //     try
    //     {
    //         var response = await Client.SendAsync(request);
    //         _uploadResult = response.IsSuccessStatusCode;
    //     }
    //     catch (Exception)
    //     {
    //         _uploadResult = false;
    //         throw;
    //     }
    // }

    public void Dispose()
    {
        _hubConnection?.DisposeAsync();
    }
}