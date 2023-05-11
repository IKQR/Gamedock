using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using GameDock.Shared.Requests;
using GameDock.Shared.Responses;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

namespace GameDock.Client.Components;

public partial class ArchiveUpload : ComponentBase
{
    [Inject]
    public HttpClient Http { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [Inject]
    public IJSRuntime JS { get; set; }

    [Parameter]
    public string NavigateOnSuccess { get; set; }

    private readonly UploadRequest _uploadRequest = new();
    private IBrowserFile _selectedFile;

    private int _progressPercentage = 0;

    private void HandleFileSelected(InputFileChangeEventArgs e)
    {
        _selectedFile = e.File;
    }

    private async Task HandleValidSubmit()
    {
        // Set buffer size to 512 KB.
        var bufferSize = 512 * 1024;
        var buffer = System.Buffers.ArrayPool<byte>.Shared.Rent(bufferSize);
        var totalBytesRead = 0L;
        var fileSize = _selectedFile.Size;

        var stream = _selectedFile.OpenReadStream(maxAllowedSize: long.MaxValue);

        try
        {
            int bytesRead;
            var chunkNumber = 0;
            var totalChunks = (int)Math.Ceiling((double)fileSize / bufferSize);

            while ((bytesRead = await stream.ReadAsync(buffer)) != 0)
            {
                totalBytesRead += bytesRead;
                _progressPercentage = (int)(100 * totalBytesRead / fileSize);
                await InvokeAsync(StateHasChanged);

                var chunkContent = new ByteArrayContent(buffer, 0, bytesRead);
                chunkNumber++;

                var formData = new MultipartFormDataContent
                {
                    { chunkContent, "archive", _selectedFile.Name },
                    { new StringContent(_uploadRequest.BuildName), "BuildName" },
                    { new StringContent(_uploadRequest.Version), "Version" },
                    { new StringContent(_uploadRequest.RuntimePah), "RuntimePah" },
                    { new StringContent(_uploadRequest.LaunchParameters ?? string.Empty), "LaunchParameters" },
                };

                formData.Headers.Add("Chunk-Number", chunkNumber.ToString());
                formData.Headers.Add("Total-Chunks", totalChunks.ToString());

                var response = await Http.PostAsync("/api/build", formData);

                if (!response.IsSuccessStatusCode) break;
                if (chunkNumber != totalChunks) continue;

                var content = await response.Content.ReadAsStreamAsync();
                var uploadResponse = await JsonSerializer.DeserializeAsync<UploadResponse>(content,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                    });

                NavigationManager.NavigateTo($"/builds/{uploadResponse.Id}");
            }
        }
        finally
        {
            System.Buffers.ArrayPool<byte>.Shared.Return(buffer);
        }
    }
}